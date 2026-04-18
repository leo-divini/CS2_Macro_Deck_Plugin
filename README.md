# CS2 GSI for Macro Deck

CS2 GSI for Macro Deck is a Macro Deck 2 plugin for the Macro Deck Extension Store. It receives Counter-Strike 2 Game State Integration data locally and publishes it as Macro Deck variables.

The plugin listens on `http://127.0.0.1:3333/`, accepts CS2 GSI `POST` payloads, and exposes the latest parsed state at `http://127.0.0.1:3333/state` for local debugging.

## Status

This project is in early development. It currently builds and loads locally in Macro Deck, but it is not ready for Macro Deck Extension Store submission yet.

Current known blockers before store submission:

- Real CS2 match/training testing still needs to be completed.

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
- Warnings: `0`

The plugin project enables both Windows Forms and WPF so the local build aligns with the installed Macro Deck desktop assemblies.

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
        "player_match_stats" "1"
        "bomb"           "1"
    }
}
```

Restart CS2 after creating or editing the config file.

## Variables

The plugin publishes these Macro Deck variables:

Macro Deck label placeholders usually use underscores instead of dots. For example, the plugin variable `cs2md.player.hp` is used in button text as `{cs2md_player_hp}`.

| Variable / button text | Type | Meaning | Example |
| --- | --- | --- | --- |
| `cs2md.connected`<br>(`{cs2md_connected}`) | Bool | `true` only after a real CS2 GSI payload has been received. | `true` |
| `cs2md.status`<br>(`{cs2md_status}`) | String | Plugin connection status. See the status values below. | `connected` |
| `cs2md.provider.name`<br>(`{cs2md_provider_name}`) | String | GSI provider name from CS2. | `Counter-Strike: Global Offensive` |
| `cs2md.provider.appid`<br>(`{cs2md_provider_appid}`) | Integer | Steam app id, normally `730`. | `730` |
| `cs2md.provider.version`<br>(`{cs2md_provider_version}`) | Integer | CS2 provider version number. | `14141` |
| `cs2md.provider.steamid`<br>(`{cs2md_provider_steamid}`) | String | Steam ID reported by the provider block. | `7656119...` |
| `cs2md.provider.timestamp`<br>(`{cs2md_provider_timestamp}`) | Integer | Provider timestamp from the GSI payload. | `1776524798` |
| `cs2md.map.name`<br>(`{cs2md_map_name}`) | String | Current map name. | `de_mirage` |
| `cs2md.map.mode`<br>(`{cs2md_map_mode}`) | String | Current game mode. | `casual` |
| `cs2md.map.phase`<br>(`{cs2md_map_phase}`) | String | Current map phase. | `live` |
| `cs2md.map.round`<br>(`{cs2md_map_round}`) | Integer | Current map round number. | `8` |
| `cs2md.map.num_matches_to_win_series`<br>(`{cs2md_map_num_matches_to_win_series}`) | Integer | Number of matches needed to win the current series, when CS2 reports it. | `0` |
| `cs2md.map.ct.consecutive_round_losses`<br>(`{cs2md_map_ct_consecutive_round_losses}`) | Integer | CT consecutive round losses. | `1` |
| `cs2md.map.ct.timeouts_remaining`<br>(`{cs2md_map_ct_timeouts_remaining}`) | Integer | CT tactical timeouts remaining. | `1` |
| `cs2md.map.ct.matches_won_this_series`<br>(`{cs2md_map_ct_matches_won_this_series}`) | Integer | CT matches won in the current series. | `0` |
| `cs2md.map.t.consecutive_round_losses`<br>(`{cs2md_map_t_consecutive_round_losses}`) | Integer | T consecutive round losses. | `0` |
| `cs2md.map.t.timeouts_remaining`<br>(`{cs2md_map_t_timeouts_remaining}`) | Integer | T tactical timeouts remaining. | `1` |
| `cs2md.map.t.matches_won_this_series`<br>(`{cs2md_map_t_matches_won_this_series}`) | Integer | T matches won in the current series. | `0` |
| `cs2md.round.phase`<br>(`{cs2md_round_phase}`) | String | Current round phase. | `freezetime` |
| `cs2md.round.wins_ct`<br>(`{cs2md_round_wins_ct}`) | Integer | CT score. | `2` |
| `cs2md.round.wins_t`<br>(`{cs2md_round_wins_t}`) | Integer | T score. | `5` |
| `cs2md.player.steamid`<br>(`{cs2md_player_steamid}`) | String | Player Steam ID. | `7656119...` |
| `cs2md.player.name`<br>(`{cs2md_player_name}`) | String | Player name. | `LeoMos` |
| `cs2md.player.observer_slot`<br>(`{cs2md_player_observer_slot}`) | Integer | Player observer slot. | `1` |
| `cs2md.player.activity`<br>(`{cs2md_player_activity}`) | String | Player activity. | `playing` |
| `cs2md.player.hp`<br>(`{cs2md_player_hp}`) | Integer | Player health. | `100` |
| `cs2md.player.armor`<br>(`{cs2md_player_armor}`) | Integer | Player armor. | `97` |
| `cs2md.player.helmet`<br>(`{cs2md_player_helmet}`) | Bool | Whether the player has a helmet. | `true` |
| `cs2md.player.defusekit`<br>(`{cs2md_player_defusekit}`) | Bool | Whether the player has a defuse kit. | `true` |
| `cs2md.player.flashed`<br>(`{cs2md_player_flashed}`) | Integer | Flash effect value reported by CS2. | `0` |
| `cs2md.player.smoked`<br>(`{cs2md_player_smoked}`) | Integer | Smoke effect value reported by CS2. | `0` |
| `cs2md.player.burning`<br>(`{cs2md_player_burning}`) | Integer | Burning effect value reported by CS2. | `0` |
| `cs2md.player.alive`<br>(`{cs2md_player_alive}`) | Bool | Whether player health is above zero. | `true` |
| `cs2md.player.money`<br>(`{cs2md_player_money}`) | Integer | Player money. | `3050` |
| `cs2md.player.team`<br>(`{cs2md_player_team}`) | String | Player team, usually `CT` or `T`. | `T` |
| `cs2md.player.kills_round`<br>(`{cs2md_player_kills_round}`) | Integer | Kills in the current round. | `1` |
| `cs2md.player.headshot_kills_round`<br>(`{cs2md_player_headshot_kills_round}`) | Integer | Headshot kills in the current round. | `0` |
| `cs2md.player.kills_total`<br>(`{cs2md_player_kills_total}`) | Integer | Match kills. | `5` |
| `cs2md.player.assists`<br>(`{cs2md_player_assists}`) | Integer | Match assists. | `0` |
| `cs2md.player.deaths`<br>(`{cs2md_player_deaths}`) | Integer | Match deaths. | `1` |
| `cs2md.player.mvps`<br>(`{cs2md_player_mvps}`) | Integer | Match MVP count. | `0` |
| `cs2md.player.score`<br>(`{cs2md_player_score}`) | Integer | Match score. | `7` |
| `cs2md.player.equip_value`<br>(`{cs2md_player_equip_value}`) | Integer | Current equipment value. | `4100` |
| `cs2md.weapon.name`<br>(`{cs2md_weapon_name}`) | String | Current weapon name, including while reloading. | `weapon_galilar` |
| `cs2md.weapon.type`<br>(`{cs2md_weapon_type}`) | String | Current weapon type. | `Rifle` |
| `cs2md.weapon.paintkit`<br>(`{cs2md_weapon_paintkit}`) | String | Current weapon paint kit reported by CS2. | `default` |
| `cs2md.weapon.state`<br>(`{cs2md_weapon_state}`) | String | Current weapon state. | `active` / `reloading` |
| `cs2md.weapon.ammo_clip`<br>(`{cs2md_weapon_ammo_clip}`) | Integer | Ammo in the current weapon clip. | `24` |
| `cs2md.weapon.ammo_clip_max`<br>(`{cs2md_weapon_ammo_clip_max}`) | Integer | Maximum clip size for the current weapon. | `35` |
| `cs2md.weapon.ammo_reserve`<br>(`{cs2md_weapon_ammo_reserve}`) | Integer | Reserve ammo value reported by CS2. | `4` |
| `cs2md.bomb.state`<br>(`{cs2md_bomb_state}`) | String | Bomb state from CS2 GSI. | `planted` |
| `cs2md.bomb.site`<br>(`{cs2md_bomb_site}`) | String | Compatibility alias for bomb position. | `1210.5, -842.25, 64.0` |
| `cs2md.bomb.position`<br>(`{cs2md_bomb_position}`) | String | Raw bomb coordinate string from CS2 GSI, when CS2 sends the `bomb` block. | `1210.5, -842.25, 64.0` |
| `cs2md.bomb.timer`<br>(`{cs2md_bomb_timer}`) | String | Bomb countdown when available or locally estimated after plant. | `14s` |
| `cs2md.bomb.carrier`<br>(`{cs2md_bomb_carrier}`) | String | Bomb carrier/player field when CS2 sends it. | `7656119...` |

`cs2md.bomb.position`, `cs2md.bomb.site`, and `cs2md.bomb.carrier` depend on CS2 sending the `bomb` payload block. Valve documents that bomb position data is observer/spectator-only, so these values are often empty during normal player gameplay. `cs2md.bomb.site` is kept as a compatibility alias for now; automatic A/B site detection is not implemented yet.

Useful button label examples:

```text
STAT {cs2md_status}
CONN {cs2md_connected}
```

```text
{cs2md_map_name}
{cs2md_map_mode} {cs2md_map_phase}
R{cs2md_map_round}
CT {cs2md_round_wins_ct} / T {cs2md_round_wins_t}
```

```text
{cs2md_player_name}
{cs2md_player_team} {cs2md_player_activity}
HP {cs2md_player_hp}
AR {cs2md_player_armor}
```

```text
{cs2md_weapon_name}
{cs2md_weapon_type}
{cs2md_weapon_state}
{cs2md_weapon_ammo_clip}/{cs2md_weapon_ammo_reserve}
```

```text
BOMB {cs2md_bomb_state}
TIMER {cs2md_bomb_timer}
POS {cs2md_bomb_position}
```

`cs2md.status` uses these values:

| Value | Meaning |
| --- | --- |
| `starting` | Variables were initialized and the listener is starting. |
| `waiting_for_cs2` | The listener is running, but no real CS2 payload has been received yet. |
| `connected` | A real CS2 payload has been received and variables were updated. |
| `token_invalid` | CS2 sent a payload with a token that does not match the plugin token. |
| `port_in_use` | The plugin could not bind to port `3333`; another process is using it. |
| `listener_offline` | The plugin is polling `/state`, but no listener is reachable. |
| `restarting` | The reset action is restarting the listener. |
| `error` | An unexpected error occurred while publishing state. |

## Actions

The plugin provides these Macro Deck actions:

- `Refresh CS2 state`: reads the latest local `/state` response and updates Macro Deck variables.
- `Reset CS2 listener`: restarts the plugin's local listener and updates `cs2md.status`.

## Debugging

Open this URL locally while Macro Deck is running:

```text
http://127.0.0.1:3333/state
```

If the endpoint returns HTTP 200 but all values are empty or zero, the plugin is listening but CS2 has not sent useful GSI data yet. Start CS2, enter a training session or match, and reload `/state`.

To inspect the latest raw CS2 GSI payload received by the plugin:

```text
http://127.0.0.1:3333/raw
```

To run the optional console listener instead of the Macro Deck plugin:

```powershell
tools\run-listener.cmd
```

Close Macro Deck first, or free port `3333`, before running the debug listener.

## Troubleshooting

Port already in use:

- Close Macro Deck or the debug listener.
- Only one process can listen on `http://127.0.0.1:3333/`.
- `cs2md.status` is set to `port_in_use` when the plugin cannot bind to port `3333`.

`/state` is empty:

- Confirm CS2 was restarted after adding the GSI config.
- Enter a live match or training session.
- Confirm the config file is in the correct CS2 `cfg` folder.
- `cs2md.connected` remains `false` and `cs2md.status` remains `waiting_for_cs2` until a real CS2 GSI payload is received.

Token mismatch:

- The CS2 config token must match the plugin default token.
- Current default token: `cs2md_token_segreto`
- `cs2md.status` is set to `token_invalid` when the plugin receives a payload with the wrong token.

Macro Deck update check returns 404 for this plugin:

- This is expected for local development while the plugin is not published in the Extension Store.

## Privacy

The plugin listens only on `127.0.0.1` and receives local CS2 Game State Integration payloads. It does not send CS2 data to an external service.

## Icon

The icon is an original radar/HUD-style design. It does not use Counter-Strike, Valve, or Macro Deck logo assets.

The `GSI` lettering uses Oxanium, released under the SIL Open Font License. See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

## Roadmap Before Store Submission

- Add configurable token and port.
- Complete real CS2 testing.
- Tag the first release as `v0.1.0`.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
