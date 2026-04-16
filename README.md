# CS2 GSI for Macro Deck

CS2 GSI for Macro Deck is a Macro Deck 2 plugin for the Macro Deck Extension Store. It receives Counter-Strike 2 Game State Integration data locally and publishes it as Macro Deck variables.

The plugin listens on `http://127.0.0.1:3333/`, accepts CS2 GSI `POST` payloads, and exposes the latest parsed state at `http://127.0.0.1:3333/state` for local debugging.

## Status

This project is in early development. It currently builds and loads locally in Macro Deck, but it is not ready for Macro Deck Extension Store submission yet.

Current known blockers before store submission:

- The local build emits warning `MSB3277` for `WindowsBase` because the project references the installed `Macro Deck 2.dll` directly.
- An extension icon and final release documentation still need to be completed.

## Requirements

- Windows
- Macro Deck 2
- Counter-Strike 2
- .NET SDK 9 or newer for development builds
- Macro Deck installed at `C:\Program Files\Macro Deck\Macro Deck 2.dll` for local compilation

## Projects

- `Cs2MacroDeck.Plugin`: Macro Deck plugin.
- `Cs2Gsi.Core`: CS2 GSI models, parser, defaults, and shared HTTP server.
- `Cs2Gsi.Listener`: optional console/debug listener for development.
- `tools`: helper launch files for the debug listener.

For normal use, run Macro Deck and let the plugin receive CS2 data directly. Do not run `Cs2Gsi.Listener` at the same time unless you are explicitly debugging, because both try to use port `3333`.

## Build

From the repository root:

```powershell
dotnet build Cs2MacroDeck.slnx
```

Expected current result:

- Errors: `0`
- Known warning: `MSB3277 WindowsBase`

The warning is caused by the direct reference to the locally installed Macro Deck assembly and is not currently known to block local plugin loading.

## Local Installation

Build the solution, then copy the plugin build output to Macro Deck's plugin folder:

```text
%AppData%\Macro Deck\plugins\LeoM.Cs2Gsi
```

The local plugin folder must contain at least:

```text
Cs2MacroDeck.Plugin.dll
Cs2MacroDeck.Plugin.deps.json
ExtensionManifest.json
```

Current development builds compile the shared GSI code directly into `Cs2MacroDeck.Plugin.dll`, so the plugin output does not require `Cs2Gsi.Core.dll`.

Restart Macro Deck after copying the files.

Useful log lines:

```text
CS2 Macro Deck plugin enabled.
CS2 GSI server listening on http://127.0.0.1:3333/
```

## CS2 Game State Integration Config

Create this file in the CS2 config folder:

```text
Counter-Strike Global Offensive\game\csgo\cfg\gamestate_integration_cs2md.cfg
```

Example config:

```text
"CS2 Macro Deck GSI"
{
    "uri"       "http://127.0.0.1:3333/"
    "timeout"   "5.0"
    "buffer"    "0.1"
    "throttle"  "0.5"
    "heartbeat" "10.0"
    "auth"
    {
        "token" "cs2md_token_segreto"
    }
    "data"
    {
        "provider"       "1"
        "map"            "1"
        "round"          "1"
        "player_id"      "1"
        "player_state"   "1"
        "player_weapons" "1"
        "bomb"           "1"
    }
}
```

Restart CS2 after creating or editing the config file.

## Variables

The plugin publishes these Macro Deck variables:

```text
cs2md.connected
cs2md.status
cs2md.map.name
cs2md.map.mode
cs2md.map.round
cs2md.round.phase
cs2md.round.wins_ct
cs2md.round.wins_t
cs2md.player.hp
cs2md.player.armor
cs2md.player.helmet
cs2md.player.alive
cs2md.player.money
cs2md.player.team
cs2md.player.kills_round
cs2md.player.kills_total
cs2md.player.assists
cs2md.player.deaths
cs2md.weapon.name
cs2md.weapon.type
cs2md.weapon.ammo_clip
cs2md.weapon.ammo_reserve
cs2md.bomb.state
cs2md.bomb.site
cs2md.bomb.timer
```

## Debugging

Open this URL locally while Macro Deck is running:

```text
http://127.0.0.1:3333/state
```

If the endpoint returns HTTP 200 but all values are empty or zero, the plugin is listening but CS2 has not sent useful GSI data yet. Start CS2, enter a training session or match, and reload `/state`.

To run the optional console listener instead of the Macro Deck plugin:

```powershell
tools\run-listener.cmd
```

Close Macro Deck first, or free port `3333`, before running the debug listener.

## Troubleshooting

Port already in use:

- Close Macro Deck or the debug listener.
- Only one process can listen on `http://127.0.0.1:3333/`.

`/state` is empty:

- Confirm CS2 was restarted after adding the GSI config.
- Enter a live match or training session.
- Confirm the config file is in the correct CS2 `cfg` folder.

Token mismatch:

- The CS2 config token must match the plugin default token.
- Current default token: `cs2md_token_segreto`

Macro Deck update check returns 404 for this plugin:

- This is expected for local development while the plugin is not published in the Extension Store.

## Privacy

The plugin listens only on `127.0.0.1` and receives local CS2 Game State Integration payloads. It does not send CS2 data to an external service.

## Roadmap Before Store Submission

- Add `Plugin.png` and `ExtensionIcon.png`.
- Add configurable token and port.
- Improve connection states such as `waiting_for_cs2`, `token_invalid`, and `port_in_use`.
- Complete real CS2 testing.
- Tag the first release as `v0.1.0`.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
