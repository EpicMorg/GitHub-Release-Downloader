# Design decisions

Why the code looks the way it does. Each entry records a choice that is **not**
obvious from reading the source, so it does not get undone by accident.

Companion document: [STATE.md](STATE.md) — where the work currently stands.

---

## Project layout

```
src/
  GitHub Release Downloader.sln
  Core/   net10.0            shared logic, AOT-compatible, no UI
  GUI/    net10.0-windows    WinForms app
  TUI/    net10.0            console app `grd`, native AOT
```

**Why a separate Core.** The GUI and the console app need identical download
behaviour. Core holds the repository parser, the API models and the downloader;
neither front-end duplicates any of it.

**Why Core targets plain `net10.0`** (not `-windows`): the console app must build
for Linux and macOS.

**Why each project sits in its own folder.** The GUI project used to live at the
`src/` root, where its default globs swallowed every `.cs` under `src/**` —
including the other projects' sources, which produced duplicate type errors. The
old workaround was `<Compile Remove="Core/**" />` in the GUI project; giving every
project its own directory removed the need for it. Do not move a project back to
the `src/` root.

---

## Core

### Source-generated JSON, not reflection

`GitHubJsonContext` (a `JsonSerializerContext`) supplies metadata for
`GitHubRelease` and `List<GitHubRelease>`, and `GetJsonAsync` takes a
`JsonTypeInfo<T>`.

Reflection-based `System.Text.Json` — `ReadFromJsonAsync<T>()` — **does not
survive native AOT**. The trimmer removes the property metadata and deserialization
returns empty objects at runtime, usually without an error. If a new API field or
type is introduced, add a `[JsonSerializable]` attribute for it, otherwise the
console build breaks in a way the GUI build will not reveal.

`IsAotCompatible=true` on Core keeps the trim/AOT analyzers on, so this class of
mistake surfaces at build time.

### Skipping files by size

Default policy is *skip if present and the size matches*:

| On disk | Advertised size | Action |
|---|---|---|
| missing | — | download |
| present, size equal | `asset.size` | skip |
| present, size differs | `asset.size` | re-download |
| present | unknown | skip, and say so in the log |

The size is only unknown for **source archives** (`zipball_url` / `tarball_url`),
which GitHub serves without a length. There is nothing to compare against, so the
file on disk is trusted.

GitHub does not publish per-asset checksums, so size is the strongest cheap check
available. A hash would mean downloading the file to verify it, defeating the point.

### Downloads land in a `.part` file first

Bytes stream into `<file>.part` and are moved over the target only on success. An
aborted or cancelled download therefore never leaves a truncated file that the next
run would see as complete and skip.

### Folder layout

```
<target>/<owner>/<repo>/<tag>/<file>      default
<target>/<tag>/<file>                     GUI: checkbox off / CLI: --flat
```

The `<tag>` level stays in **both** modes. Without it, downloading every release
drops files from different versions into one directory, where same-named assets
overwrite each other.

Every path segment goes through `SanitizeSegment`, because tags legitimately
contain `/` (e.g. `release/1.2`) and would otherwise create unintended directories.

### Pre-releases and `/releases/latest`

`GET /releases/latest` **never returns a pre-release** — that is GitHub's
behaviour, not a bug. So "latest release only" + "include pre-releases" cannot use
that endpoint; the code falls back to the first entry of the full list instead.

Drafts are always excluded: they are invisible without repository write access.

### HTTP details

- A `User-Agent` header is **mandatory**; GitHub rejects requests without one.
- The token goes out as `Authorization: Bearer`. `HttpClient` drops that header on a
  cross-origin redirect, so it never reaches the CDN host that serves the asset
  bytes. Do not replace the handler with one that forwards headers across hosts.
- Unauthenticated API access is limited to **60 requests/hour** per IP, 5000 with a
  token. Downloading every release of a large repository can exhaust the anonymous
  budget, which is why the token field exists.
- A failure on one file is counted and reported; the rest of the batch continues.

---

## GUI

### Settings

Stored at `%APPDATA%\EpicMorg\GitHub Release Downloader\settings.json`: repository
URL, target path, all four checkboxes, the skip/overwrite choice and the token.
Written when the form closes **and** when a download starts, so a crash mid-download
does not lose the setup.

**The token is a DPAPI blob** (`ProtectedData.Protect`, scope `CurrentUser`) stored
as base64; everything else is plain JSON. Only the same Windows user on the same
machine can decrypt it. This protects against the file being copied elsewhere — not
against code running as that same user.

When the blob cannot be decrypted (different machine, reinstalled profile) the token
is silently treated as empty and the app asks for it again. A corrupt `settings.json`
falls back to defaults rather than blocking start-up.

`System.Security.Cryptography.ProtectedData` **needs no NuGet package** on .NET 10
for Windows — it is in the shared framework. Adding it produces warning `NU1510`.

### Cross-thread logging

Core awaits with `ConfigureAwait(false)`, so its log callback arrives on a thread
pool thread. `FrmMain.Log` therefore builds the line (timestamping at the moment of
the event) and hands it to the UI thread with `BeginInvoke` — not `Invoke`, which
would stall the downloading thread on every line.

`IProgress<DownloadProgress>` needs no such treatment: `Progress<T>` captures the UI
`SynchronizationContext` when constructed and marshals on its own.

`OnFormClosing` cancels an in-flight download, otherwise callbacks outlive the window
handle and `BeginInvoke` throws on a background thread where nothing catches it.

### Layout

Every control is anchored and the form has a `MinimumSize`; without the minimum, the
log box collapses and the buttons overlap when the window is shrunk.

---

## Console app (`grd`)

**No configuration file, by design.** Every parameter comes from the command line;
the only environment input is `GITHUB_TOKEN` as a fallback for `--token`. The GUI's
`settings.json` is deliberately not shared.

Defaults match the GUI: all stable releases, skip files whose size matches, create
`<owner>/<repo>` sub-folders.

Exit codes: `0` success, `1` bad arguments, `2` at least one file failed or the run
was cancelled.

`Ctrl+C` sets `e.Cancel = true` and cancels the token, so the run unwinds cleanly and
the `.part` file is not left looking like a finished download.

The argument parser is hand-written — no `System.CommandLine` — to keep the AOT
build free of an extra dependency.

---

## CI

### Why the matrix has one entry per OS *and* architecture

**ILCompiler cannot cross-compile.** `-r linux-arm64` on an x64 runner fails. Every
RID is therefore built on a runner of that same OS and architecture:

| RID | Runner |
|---|---|
| linux-x64 | `ubuntu-latest` |
| linux-arm64 | `ubuntu-24.04-arm` |
| win-x64 | `windows-latest` |
| win-arm64 | `windows-11-arm` |
| osx-x64 | `macos-13` |
| osx-arm64 | `macos-latest` |

Linux runners additionally need `clang` and `zlib1g-dev`; AOT linking fails without
them.

### The GUI is not AOT

**WinForms does not support native AOT.** The GUI ships as a self-contained
single-file build instead, Windows-only.

### Packaging

Windows artifacts are zipped, Linux/macOS use `tar.gz` — tar preserves the
executable bit, zip does not.

### Local AOT builds on Windows

If two .NET SDKs are installed, the **x86** SDK (`C:\Program Files (x86)\dotnet`) can
shadow the x64 one on `PATH`. Publishing `-r win-x64` from it is treated as
cross-compilation and fails with:

> Add a PackageReference for 'runtime.win-x64.Microsoft.DotNet.ILCompiler'

Use `C:\Program Files\dotnet\dotnet.exe` explicitly. Native AOT has no x86 target at
all.
