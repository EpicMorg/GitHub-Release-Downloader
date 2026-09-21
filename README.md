# GitHub Release Downloader

Download every asset of a GitHub release — or of every release a repository ever
published — into a tidy folder tree, and keep it up to date without re-fetching what
you already have.

Ships as two front-ends over one shared core:

- **GUI** — a Windows desktop app.
- **`grd`** — a console tool, compiled ahead of time to a single native binary for
  Windows, Linux and macOS.

Neither needs the .NET runtime installed.

## Features

- Point it at `owner/repo`, a `github.com` URL, an SSH remote or an API URL — all
  work.
- Grab the latest release or every release; include pre-releases and source archives
  on demand. Drafts are always skipped.
- **Re-runs are cheap.** A file already on disk whose size matches the one GitHub
  advertises is skipped; one whose size differs is fetched again. Point it at the
  same folder tomorrow and it downloads only what changed.
- Interrupted downloads never masquerade as complete — bytes land in a `.part` file
  and are moved into place only on success.
- One failed asset does not abort the batch.
- Optional personal access token for private repositories and a higher API rate limit.

## Install

Grab the archive for your platform from the
[Releases](https://github.com/EpicMorg/GitHub-Release-Downloader/releases) page and
unpack it. Windows builds are `.zip`, Linux and macOS builds are `.tar.gz`.
`SHA256SUMS.txt` accompanies every release.

| Artifact | Platforms |
|---|---|
| `GitHub-Release-Downloader-gui-*` | `win-x64`, `win-arm64` |
| `grd-*` | `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, `osx-arm64` |

## Using the GUI

**Main** tab — paste the repository, choose the target folder, press **Download**.
Progress and a log appear below; the button turns into **Cancel** while a run is in
flight.

**Settings** tab — personal access token, what to download (latest only,
pre-releases, source archives, whether to create `owner/repo` sub-folders) and what to
do about files already on disk.

Settings are remembered between runs in
`%APPDATA%\EpicMorg\GitHub Release Downloader\settings.json`. The token is encrypted
with Windows DPAPI and bound to your user account, so copying that file to another
machine will not expose it.

## Using `grd`

```
grd <owner/repo | github url> [options]
```

| Option | Meaning |
|---|---|
| `-o`, `--output <dir>` | Where to download to (default: current directory) |
| `-t`, `--token <pat>` | GitHub token; falls back to the `GITHUB_TOKEN` variable |
| `-l`, `--latest` | Only the latest release (default: every release) |
| `-p`, `--pre` | Include pre-releases |
| `-s`, `--sources` | Also grab the source archives |
| `--overwrite` | Always overwrite; the default skips files whose size matches |
| `--flat` | Drop the `<owner>/<repo>` folders, keep only `<tag>` |
| `-q`, `--quiet` | Only report errors and the final summary |
| `-h`, `--help` | Show help |

Exit codes: `0` — everything downloaded or skipped, `1` — bad arguments, `2` — at
least one file failed or the run was cancelled.

The console tool keeps no configuration of its own; every parameter comes from the
command line.

```sh
# every stable release of a repository
grd sharkdp/fd -o ~/downloads

# just the newest one, pre-releases included, straight from a browser URL
grd https://github.com/sharkdp/fd/releases -l -p -o ~/downloads

# private repository, no owner/repo folders
GITHUB_TOKEN=ghp_… grd myorg/internal-tool --flat -o ./vendor
```

## Where files end up

```
<target>/<owner>/<repo>/<tag>/<file>
```

With the sub-folder checkbox off (or `--flat` on the command line):

```
<target>/<tag>/<file>
```

The `<tag>` level is always there — without it, assets from different releases would
overwrite one another.

## Tokens and rate limits

Anonymous requests to the GitHub API are capped at **60 per hour per IP**; a token
raises that to 5000 and is required for private repositories. Pulling every release
of a busy repository can exhaust the anonymous budget, and the tool will say so
plainly when it happens.

A [fine-grained token](https://github.com/settings/tokens) with read-only access to
the repositories you care about is enough. The token is sent to `api.github.com`
only — it is dropped before following the redirect to the CDN that serves the asset
bytes.

## Building from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
dotnet build "src/GitHub Release Downloader.sln"
```

To produce a native `grd` binary for the machine you are on:

```sh
dotnet publish "src/TUI/GitHub Release Downloader.Cli.csproj" -c Release -r <rid> -o out
```

Native AOT cannot cross-compile: build each RID on a machine of that same OS and
architecture. Linux additionally needs `clang` and `zlib1g-dev`.

| Project | Target | Notes |
|---|---|---|
| `src/Core/` | `net10.0` | Shared download logic, AOT-compatible, no UI |
| `src/GUI/` | `net10.0-windows` | WinForms app |
| `src/TUI/` | `net10.0` | `grd`, published with native AOT |

## Documentation

- [docs/DECISIONS.md](docs/DECISIONS.md) — why the code is shaped the way it is.
  Worth reading before changing the downloader, the JSON handling or the CI matrix.
- [docs/STATE.md](docs/STATE.md) — current state of the work and open items.

## License

[MIT](LICENSE.md) © EpicMorg
