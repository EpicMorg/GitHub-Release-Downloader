# Project state

Snapshot for picking the work up on another machine.
Last updated: **2026-09-21**. Branch: **`develop`**.

Companion document: [DECISIONS.md](DECISIONS.md) — why things are built the way
they are. Read it before changing the downloader, the JSON handling or the CI matrix.

---

## What the thing does

Downloads release assets from a GitHub repository into
`<target>/<owner>/<repo>/<tag>/<file>`. Two front-ends over one shared core: a
WinForms app and a native-AOT console tool.

## Where things live

| Path | What |
|---|---|
| `src/Core/` | `RepoReference`, `ReleaseDownloader`, API models, `GitHubJsonContext` |
| `src/GUI/` | `FrmMain`, `AppSettings` (settings.json + DPAPI token) |
| `src/TUI/` | `grd` — `Program.cs`, `CommandLineOptions.cs` |
| `.github/workflows/release.yml` | build + publish on `v*` tags |

## Build

```
dotnet build "src/GitHub Release Downloader.sln"
```

Clean as of the last run: 3 projects, 0 warnings, 0 errors.

Native AOT locally (Windows) — use the **x64** SDK explicitly, see DECISIONS.md:

```
"C:\Program Files\dotnet\dotnet.exe" publish "src/TUI/GitHub Release Downloader.Cli.csproj" -c Release -r win-x64 -o out
```

---

## Done and verified

- Repository input: `owner/repo`, `https://github.com/owner/repo/...` (trailing path
  ignored), `api.github.com/repos/...`, `git@github.com:owner/repo.git`, with or
  without `.git`.
- Latest / all releases, pre-release and source-archive toggles, drafts always skipped.
- Skip-by-size and re-download on mismatch. **Verified end to end** against
  `sharkdp/fd` with the AOT binary: 22 files downloaded, second run skipped all 22,
  a deliberately truncated file was detected and re-fetched.
- `.part` staging, per-file failure isolation, cancellation.
- GUI: tabbed form (Main / Settings), anchored layout, progress bar, log,
  settings persistence, DPAPI-protected token.
- CLI: full argument set, `--help`, `GITHUB_TOKEN` fallback, exit codes.
- AOT: trim and AOT analyzers report **0 warnings**; the published `win-x64` binary
  runs (~5 MB).

## Not verified

- **The workflow has never run** — it cannot be tested before it is pushed.
- **`win-arm64`, `linux-*`, `osx-*` builds** — never compiled; only `win-x64` was
  published locally.
- Private repositories and the token path — the token code is exercised only by the
  unauthenticated flow so far.
- No automated tests exist at all.

---

## Open items

### Next up
- **GUI visual polish.** Explicitly deferred; the layout works but has not been
  designed. This is the reason the branch is still `develop`.

### Housekeeping
- **Git index is inconsistent.** Folders were moved outside of git, so old paths show
  as deleted, `src/GUI/` and `src/TUI/` are untracked, and some files are staged
  under a `src/Cli/` path that no longer exists. `git add -A` makes git see the
  renames. Builds are unaffected.
- **TUI project filename.** The folder is `TUI/` but the project is still
  `GitHub Release Downloader.Cli.csproj` (solution entry: `…Cli`). Cosmetic;
  `AssemblyName` is `grd` either way.

### CI decisions to revisit
- **`softprops/action-gh-release@v2`** is a third-party action. For a release
  pipeline it should be pinned to a commit SHA, or replaced with `gh release create`
  to drop the dependency entirely.
- **`windows-11-arm`** runners are free for public repositories only. If this repo
  goes private, drop that matrix entry.
- **`macos-13`** is the last x64 macOS runner and is being retired by GitHub. When it
  goes, `osx-x64` goes with it.
- **Tags are not branch-checked.** Any `v*` tag triggers a release regardless of the
  branch it points at. Add a `git branch --contains` guard if releases must come from
  `master` only.

### Possible features
- Per-repository asset filters (include/exclude by name or glob).
- Resuming an interrupted download via HTTP range requests; right now a cancelled
  file restarts from zero.
- Showing the API rate-limit budget in the GUI before a large run.

---

## Release process

```
git tag v1.0.0
git push origin v1.0.0
```

Builds the GUI for `win-x64`/`win-arm64` and `grd` for six platforms, attaches the
archives plus `SHA256SUMS.txt` to a generated release. A tag containing `-`
(`v1.0.0-rc1`) is marked as a pre-release. `workflow_dispatch` builds and uploads
artifacts without publishing anything.
