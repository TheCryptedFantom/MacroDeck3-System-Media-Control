# System Media Control

A [Macro Deck 3](https://macro-deck.app) plugin that controls whatever's currently playing on your PC —
Spotify, Foobar2000, a browser tab, or any other app that reports to Windows' built-in media controls —
without needing a separate integration per app.

## Why

Macro Deck's existing music integrations (YouTube Music, Spotify, Foobar2000) each talk to one specific
app's own API. This plugin instead hooks into the operating system's media session layer
([System Media Transport Controls](https://learn.microsoft.com/uwp/api/windows.media.control)), the same
system that powers Windows 11's media flyout and hardware media keys. Any app that registers with it is
controllable here, with no per-app code required.

## Features

- Automatically detects every app currently reporting a media session
- Lets you pick which one to control when more than one is active at once
- Track title, artist, album and artwork shown in the Music Player widget
- Play, Pause, Next, Previous, Seek, and Refresh

## Platform support

**Windows only.** System Media Transport Controls is a Windows-specific API; there's no equivalent code
path for Linux or macOS in this plugin. It has not been tested on anything earlier than Windows 11.

## Not supported

- **Volume** — SMTC has no volume API for third-party control. Use the app itself or the Windows volume
  mixer.
- **Shuffle / Repeat** — not all apps expose these consistently through SMTC, and testing showed
  unreliable behavior (e.g. multi-second delays) even where they were technically available. Left out
  for now rather than shipped half-working.

## Installation

Grab the latest release from the [Releases](../../releases) page (once published) or the Macro Deck
Store, and install it through Macro Deck's plugin manager.

## Building from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```powershell
dotnet build
dotnet test
```

To run it against a disposable local host for development:

```powershell
macrodeck-plugin run --project src\CryptedFantom.SystemMedia --stub-host
```

## License

All Rights Reserved — see [LICENSE](LICENSE). This source is provided for viewing as part of the Macro
Deck plugin publication process; it is not licensed for redistribution or modification.

## Author

[Trenton Roach](https://github.com/TheCryptedFantom) ([@cryptedfantom](https://youtube.com/@cryptedfantom))