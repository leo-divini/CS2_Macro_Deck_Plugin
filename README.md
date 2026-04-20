# CS2 GSI for Macro Deck

![CS2 GSI for Macro Deck icon](ExtensionIcon.png)

CS2 GSI for Macro Deck is a Macro Deck 2 plugin that receives Counter-Strike 2 Game State Integration payloads locally and publishes them as Macro Deck variables.

It listens only on `http://127.0.0.1:3333/`. CS2 sends data to the plugin, and Macro Deck buttons can show values such as map, score, HP, armor, money, weapon, ammo, bomb state, all players, grenades, and observer-only data when CS2 provides it.

Official Valve GSI documentation:

https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration

## Screenshots

Current screenshots to add before release:

- `docs/images/macrodeck-dashboard-live.png` - Macro Deck dashboard during a live round.
- `docs/images/macrodeck-dashboard-bomb.png` - Bomb planted / timer test.
- `docs/images/macrodeck-dashboard-observer.png` - Observer/spectator test with `allplayers` data.

## Status

This project is in early development. It builds, loads locally in Macro Deck, and has been tested with real CS2 payloads, but it still needs final release cleanup before Macro Deck Extension Store submission.

Known pre-release work:

- Decide whether token and port should stay hardcoded for `0.1.0` or become configurable.
- Add final screenshots/GIFs.
- Do final clean build and release tag.
- Submit to the Macro Deck Extension Store.

## Requirements

- Windows
- Macro Deck 2
- Counter-Strike 2
- .NET SDK 9 or newer for development builds
- Macro Deck installed at `C:\Program Files\Macro Deck\Macro Deck 2.dll` for local compilation

## Setup - Beginner

Use this if you only want it to work.

1. Install or copy the plugin into:

```text
%AppData%\Macro Deck\plugins\LeoM.Cs2Gsi
```

2. Create this file in the CS2 config folder:

```text
Counter-Strike Global Offensive\game\csgo\cfg\gamestate_integration_cs2md.cfg
```

3. Paste this config:

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
    "output"
    {
        "precision_time"     "3"
        "precision_position" "1"
        "precision_vector"   "3"
    }
    "data"
    {
        "provider"               "1"
        "map"                    "1"
        "map_round_wins"         "1"
        "round"                  "1"
        "player_id"              "1"
        "player_state"           "1"
        "player_weapons"         "1"
        "player_match_stats"     "1"
        "player_position"        "1"
        "bomb"                   "1"
        "phase_countdowns"       "1"
        "allplayers_id"          "1"
        "allplayers_state"       "1"
        "allplayers_match_stats" "1"
        "allplayers_weapons"     "1"
        "allplayers_position"    "1"
        "allgrenades"            "1"
    }
}
```

4. Restart CS2.
5. Restart Macro Deck.
6. Create a Macro Deck button and use a label like:

```text
{cs2md_map_name}
{cs2md_round_phase}
HP {cs2md_player_hp}
{cs2md_weapon_name}
{cs2md_weapon_ammo_clip}/{cs2md_weapon_ammo_clip_max}
```

## Setup - Medium

Use this when you want to verify the connection and troubleshoot.

1. Start Macro Deck.
2. Open this local debug URL:

```text
http://127.0.0.1:3333/state
```

3. Start CS2 and enter a match or training session.
4. Refresh `/state`.
5. Check these values:

```text
HasPayload = true
Provider.AppId = 730
Map.Name = de_inferno / de_mirage / ...
Player.Name = your CS2 name
Player.ActiveWeapon = weapon_...
```

6. To inspect the exact raw payload received from CS2:

```text
http://127.0.0.1:3333/raw
```

Useful Macro Deck status values:

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

## Setup - Advanced

Use this for observer dashboards, full match panels, or debugging all payload blocks.

Valve separates GSI blocks into normal player data and observer/spectator data. In Valve's official list, these blocks are under "must be spectating or observing":

```text
allgrenades
allplayers_id
allplayers_match_stats
allplayers_position
allplayers_state
allplayers_weapons
bomb
phase_countdowns
player_position
```

This plugin exposes variables for those blocks, but CS2 may leave them empty during normal gameplay. In real local tests, `bomb.state` and `bomb.timer` arrived during planted/defused events, while `bomb.position`, `bomb.carrier`, and `bomb.site` stayed empty. That matches Valve's observer/spectator limitation.

If you discover a CS2 mode or config where these fields behave differently, please open a PR with the raw `/raw` payload and a short note explaining the mode, map, and camera state.

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
Plugin.png
```

Current development builds compile the shared GSI code directly into `Cs2MacroDeck.Plugin.dll`, so the plugin output does not require `Cs2Gsi.Core.dll`.

Restart Macro Deck after copying the files.

Useful log lines:

```text
CS2 Macro Deck plugin enabled.
CS2 GSI server listening on http://127.0.0.1:3333/
```

## Variables

Macro Deck label placeholders use underscores instead of dots.

Example:

```text
Plugin variable: cs2md.player.hp
Button text:     {cs2md_player_hp}
```

Slot numbers are part of the variable name.

Example:

```text
Plugin variable: cs2md.ap01.name
Button text:     {cs2md_ap01_name}
```

### Connection

| Variable | Placeholder | Type | Meaning |
| --- | --- | --- | --- |
| `cs2md.connected` | `{cs2md_connected}` | Bool | `true` after a real CS2 payload has been received. |
| `cs2md.status` | `{cs2md_status}` | String | Plugin/listener status. |

### Provider

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.provider.name` | `{cs2md_provider_name}` | String |
| `cs2md.provider.appid` | `{cs2md_provider_appid}` | Integer |
| `cs2md.provider.version` | `{cs2md_provider_version}` | Integer |
| `cs2md.provider.steamid` | `{cs2md_provider_steamid}` | String |
| `cs2md.provider.timestamp` | `{cs2md_provider_timestamp}` | Integer |

### Map And Round

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.map.name` | `{cs2md_map_name}` | String |
| `cs2md.map.mode` | `{cs2md_map_mode}` | String |
| `cs2md.map.phase` | `{cs2md_map_phase}` | String |
| `cs2md.map.round` | `{cs2md_map_round}` | Integer |
| `cs2md.map.num_matches_to_win_series` | `{cs2md_map_num_matches_to_win_series}` | Integer |
| `cs2md.map.ct.consecutive_round_losses` | `{cs2md_map_ct_consecutive_round_losses}` | Integer |
| `cs2md.map.ct.timeouts_remaining` | `{cs2md_map_ct_timeouts_remaining}` | Integer |
| `cs2md.map.ct.matches_won_this_series` | `{cs2md_map_ct_matches_won_this_series}` | Integer |
| `cs2md.map.t.consecutive_round_losses` | `{cs2md_map_t_consecutive_round_losses}` | Integer |
| `cs2md.map.t.timeouts_remaining` | `{cs2md_map_t_timeouts_remaining}` | Integer |
| `cs2md.map.t.matches_won_this_series` | `{cs2md_map_t_matches_won_this_series}` | Integer |
| `cs2md.round.phase` | `{cs2md_round_phase}` | String |
| `cs2md.round.win_team` | `{cs2md_round_win_team}` | String |
| `cs2md.round.wins_ct` | `{cs2md_round_wins_ct}` | Integer |
| `cs2md.round.wins_t` | `{cs2md_round_wins_t}` | Integer |

### Map Round Wins

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.rw.count` | `{cs2md_rw_count}` | Integer |
| `cs2md.rw.history` | `{cs2md_rw_history}` | String |
| `cs2md.rw.raw_json` | `{cs2md_rw_raw_json}` | String |
| `cs2md.rw.01` to `.30` | `{cs2md_rw_01}` to `{cs2md_rw_30}` | String |

### Phase Countdowns

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.phase_countdowns.phase` | `{cs2md_phase_countdowns_phase}` | String |
| `cs2md.phase_countdowns.ends_in` | `{cs2md_phase_countdowns_ends_in}` | String |

### Current Player

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.player.steamid` | `{cs2md_player_steamid}` | String |
| `cs2md.player.name` | `{cs2md_player_name}` | String |
| `cs2md.player.observer_slot` | `{cs2md_player_observer_slot}` | Integer |
| `cs2md.player.activity` | `{cs2md_player_activity}` | String |
| `cs2md.player.team` | `{cs2md_player_team}` | String |
| `cs2md.player.hp` | `{cs2md_player_hp}` | Integer |
| `cs2md.player.armor` | `{cs2md_player_armor}` | Integer |
| `cs2md.player.helmet` | `{cs2md_player_helmet}` | Bool |
| `cs2md.player.defusekit` | `{cs2md_player_defusekit}` | Bool |
| `cs2md.player.flashed` | `{cs2md_player_flashed}` | Integer |
| `cs2md.player.smoked` | `{cs2md_player_smoked}` | Integer |
| `cs2md.player.burning` | `{cs2md_player_burning}` | Integer |
| `cs2md.player.alive` | `{cs2md_player_alive}` | Bool |
| `cs2md.player.money` | `{cs2md_player_money}` | Integer |
| `cs2md.player.kills_round` | `{cs2md_player_kills_round}` | Integer |
| `cs2md.player.headshot_kills_round` | `{cs2md_player_headshot_kills_round}` | Integer |
| `cs2md.player.kills_total` | `{cs2md_player_kills_total}` | Integer |
| `cs2md.player.assists` | `{cs2md_player_assists}` | Integer |
| `cs2md.player.deaths` | `{cs2md_player_deaths}` | Integer |
| `cs2md.player.mvps` | `{cs2md_player_mvps}` | Integer |
| `cs2md.player.score` | `{cs2md_player_score}` | Integer |
| `cs2md.player.equip_value` | `{cs2md_player_equip_value}` | Integer |
| `cs2md.player.position` | `{cs2md_player_position}` | String |
| `cs2md.player.forward` | `{cs2md_player_forward}` | String |

### Current Weapon

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.weapon.name` | `{cs2md_weapon_name}` | String |
| `cs2md.weapon.type` | `{cs2md_weapon_type}` | String |
| `cs2md.weapon.paintkit` | `{cs2md_weapon_paintkit}` | String |
| `cs2md.weapon.state` | `{cs2md_weapon_state}` | String |
| `cs2md.weapon.ammo_clip` | `{cs2md_weapon_ammo_clip}` | Integer |
| `cs2md.weapon.ammo_clip_max` | `{cs2md_weapon_ammo_clip_max}` | Integer |
| `cs2md.weapon.ammo_reserve` | `{cs2md_weapon_ammo_reserve}` | Integer |

The plugin keeps the weapon visible while `weapon.state` is `active` or `reloading`.

### Current Player Weapon Slots

Current-player inventory is exposed as `cs2md.pw01` through `cs2md.pw08`.

| Pattern | Placeholder Example | Type |
| --- | --- | --- |
| `cs2md.pw.count` | `{cs2md_pw_count}` | Integer |
| `cs2md.pw.raw_json` | `{cs2md_pw_raw_json}` | String |
| `cs2md.pw01.slot` | `{cs2md_pw01_slot}` | String |
| `cs2md.pw01.name` | `{cs2md_pw01_name}` | String |
| `cs2md.pw01.type` | `{cs2md_pw01_type}` | String |
| `cs2md.pw01.paint` | `{cs2md_pw01_paint}` | String |
| `cs2md.pw01.state` | `{cs2md_pw01_state}` | String |
| `cs2md.pw01.ammo` | `{cs2md_pw01_ammo}` | Integer |
| `cs2md.pw01.ammo_max` | `{cs2md_pw01_ammo_max}` | Integer |
| `cs2md.pw01.reserve` | `{cs2md_pw01_reserve}` | Integer |

Replace `01` with `02` through `08` for the other slots.

### Bomb

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.bomb.state` | `{cs2md_bomb_state}` | String |
| `cs2md.bomb.timer` | `{cs2md_bomb_timer}` | String |
| `cs2md.bomb.position` | `{cs2md_bomb_position}` | String |
| `cs2md.bomb.carrier` | `{cs2md_bomb_carrier}` | String |
| `cs2md.bomb.site` | `{cs2md_bomb_site}` | String |

`cs2md.bomb.site` is currently a compatibility alias for `bomb.position`; automatic A/B site detection is not implemented yet.

### All Players

Observer/spectator payloads are exposed as `cs2md.ap01` through `cs2md.ap10`. Short names are used because Macro Deck truncates long variable names in the picker.

| Pattern | Placeholder Example | Type |
| --- | --- | --- |
| `cs2md.ap.count` | `{cs2md_ap_count}` | Integer |
| `cs2md.ap.raw_json` | `{cs2md_ap_raw_json}` | String |
| `cs2md.ap01.steamid` | `{cs2md_ap01_steamid}` | String |
| `cs2md.ap01.name` | `{cs2md_ap01_name}` | String |
| `cs2md.ap01.slot` | `{cs2md_ap01_slot}` | Integer |
| `cs2md.ap01.activity` | `{cs2md_ap01_activity}` | String |
| `cs2md.ap01.team` | `{cs2md_ap01_team}` | String |
| `cs2md.ap01.hp` | `{cs2md_ap01_hp}` | Integer |
| `cs2md.ap01.armor` | `{cs2md_ap01_armor}` | Integer |
| `cs2md.ap01.helmet` | `{cs2md_ap01_helmet}` | Bool |
| `cs2md.ap01.defusekit` | `{cs2md_ap01_defusekit}` | Bool |
| `cs2md.ap01.flashed` | `{cs2md_ap01_flashed}` | Integer |
| `cs2md.ap01.smoked` | `{cs2md_ap01_smoked}` | Integer |
| `cs2md.ap01.burning` | `{cs2md_ap01_burning}` | Integer |
| `cs2md.ap01.alive` | `{cs2md_ap01_alive}` | Bool |
| `cs2md.ap01.money` | `{cs2md_ap01_money}` | Integer |
| `cs2md.ap01.kr` | `{cs2md_ap01_kr}` | Integer |
| `cs2md.ap01.hsr` | `{cs2md_ap01_hsr}` | Integer |
| `cs2md.ap01.kt` | `{cs2md_ap01_kt}` | Integer |
| `cs2md.ap01.assists` | `{cs2md_ap01_assists}` | Integer |
| `cs2md.ap01.deaths` | `{cs2md_ap01_deaths}` | Integer |
| `cs2md.ap01.mvps` | `{cs2md_ap01_mvps}` | Integer |
| `cs2md.ap01.score` | `{cs2md_ap01_score}` | Integer |
| `cs2md.ap01.equip` | `{cs2md_ap01_equip}` | Integer |
| `cs2md.ap01.pos` | `{cs2md_ap01_pos}` | String |
| `cs2md.ap01.forward` | `{cs2md_ap01_forward}` | String |
| `cs2md.ap01.aw.name` | `{cs2md_ap01_aw_name}` | String |
| `cs2md.ap01.aw.type` | `{cs2md_ap01_aw_type}` | String |
| `cs2md.ap01.aw.paint` | `{cs2md_ap01_aw_paint}` | String |
| `cs2md.ap01.aw.state` | `{cs2md_ap01_aw_state}` | String |
| `cs2md.ap01.aw.ammo` | `{cs2md_ap01_aw_ammo}` | Integer |
| `cs2md.ap01.aw.ammo_max` | `{cs2md_ap01_aw_ammo_max}` | Integer |
| `cs2md.ap01.aw.reserve` | `{cs2md_ap01_aw_reserve}` | Integer |
| `cs2md.ap01.w.count` | `{cs2md_ap01_w_count}` | Integer |
| `cs2md.ap01.w.raw_json` | `{cs2md_ap01_w_raw_json}` | String |
| `cs2md.ap01.w01.slot` | `{cs2md_ap01_w01_slot}` | String |
| `cs2md.ap01.w01.name` | `{cs2md_ap01_w01_name}` | String |
| `cs2md.ap01.w01.type` | `{cs2md_ap01_w01_type}` | String |
| `cs2md.ap01.w01.paint` | `{cs2md_ap01_w01_paint}` | String |
| `cs2md.ap01.w01.state` | `{cs2md_ap01_w01_state}` | String |
| `cs2md.ap01.w01.ammo` | `{cs2md_ap01_w01_ammo}` | Integer |
| `cs2md.ap01.w01.ammo_max` | `{cs2md_ap01_w01_ammo_max}` | Integer |
| `cs2md.ap01.w01.reserve` | `{cs2md_ap01_w01_reserve}` | Integer |

Replace the first `01` with `02` through `10` for the other players. Replace the second weapon `01` with `02` through `08` for each player's weapon slots.

### Grenades

Observer/spectator grenade payloads are exposed as `cs2md.g01` through `cs2md.g16`.

| Pattern | Placeholder Example | Type |
| --- | --- | --- |
| `cs2md.g.count` | `{cs2md_g_count}` | Integer |
| `cs2md.g.raw_json` | `{cs2md_g_raw_json}` | String |
| `cs2md.g01.id` | `{cs2md_g01_id}` | String |
| `cs2md.g01.owner` | `{cs2md_g01_owner}` | String |
| `cs2md.g01.type` | `{cs2md_g01_type}` | String |
| `cs2md.g01.pos` | `{cs2md_g01_pos}` | String |
| `cs2md.g01.vel` | `{cs2md_g01_vel}` | String |
| `cs2md.g01.life` | `{cs2md_g01_life}` | String |
| `cs2md.g01.effect` | `{cs2md_g01_effect}` | String |
| `cs2md.g01.flames` | `{cs2md_g01_flames}` | Integer |
| `cs2md.g01.raw_json` | `{cs2md_g01_raw_json}` | String |

Replace `01` with `02` through `16` for the other grenade slots.

## Button Label Examples

Basic live button:

```text
{cs2md_map_name}
{cs2md_round_phase}
CT {cs2md_round_wins_ct} / T {cs2md_round_wins_t}
```

Player button:

```text
{cs2md_player_name}
{cs2md_player_team} {cs2md_player_activity}
HP {cs2md_player_hp}
AR {cs2md_player_armor}
```

Weapon button:

```text
{cs2md_weapon_name}
{cs2md_weapon_type}
{cs2md_weapon_state}
{cs2md_weapon_ammo_clip}/{cs2md_weapon_ammo_clip_max}
```

Bomb button:

```text
BOMB {cs2md_bomb_state}
TIMER {cs2md_bomb_timer}
POS {cs2md_bomb_position}
```

Observer player slot:

```text
P1 {cs2md_ap01_name}
{cs2md_ap01_team}
HP {cs2md_ap01_hp}
{cs2md_ap01_aw_name}
```

Grenade slot:

```text
Nade {cs2md_g01_type}
owner {cs2md_g01_owner}
{cs2md_g01_pos}
```

## What May Be Empty

Some variables are only available when CS2 actually sends that block.

Usually available during normal player gameplay:

- `provider`
- `map`
- `round`
- `player_id`
- `player_state`
- `player_weapons`
- `player_match_stats`

Often observer/spectator-only according to Valve:

- `allplayers_*`
- `allgrenades`
- `bomb.position`
- `bomb.carrier`
- `phase_countdowns`
- `player.position`
- `player.forward`

The plugin still exposes these variables, but empty values usually mean CS2 did not send that field.

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

## Projects

- `Cs2MacroDeck.Plugin`: Macro Deck plugin.
- `Cs2Gsi.Core`: CS2 GSI models, parser, defaults, and shared HTTP server.
- `Cs2Gsi.Listener`: optional console/debug listener for development.
- `tools`: helper launch files for the debug listener.

For normal use, run Macro Deck and let the plugin receive CS2 data directly. Do not run `Cs2Gsi.Listener` at the same time unless you are explicitly debugging, because both try to use port `3333`.

## Roadmap Before Store Submission

- Add configurable token and port, or explicitly freeze the defaults for `0.1.0`.
- Add final Macro Deck screenshots/GIFs.
- Tag the first release as `v0.1.0`.
- Submit to the Macro Deck Extension Store.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
