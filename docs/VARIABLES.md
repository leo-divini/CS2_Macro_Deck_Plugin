# CS2 GSI for Macro Deck Variables

This page explains how to use CS2 GSI for Macro Deck variables in Macro Deck buttons.

## Placeholder Format

Plugin variable names use dots:

```text
cs2md.player.hp
```

Macro Deck button placeholders use underscores:

```text
{cs2md_player_hp}
```

Examples:

| Plugin variable | Macro Deck placeholder |
| --- | --- |
| `cs2md.map.name` | `{cs2md_map_name}` |
| `cs2md.round.phase` | `{cs2md_round_phase}` |
| `cs2md.player.hp` | `{cs2md_player_hp}` |
| `cs2md.weapon.name` | `{cs2md_weapon_name}` |
| `cs2md.bomb.state` | `{cs2md_bomb_state}` |

## Quick Button Examples

Basic live state:

```text
{cs2md_map_name}
{cs2md_round_phase}
CT {cs2md_round_wins_ct} / T {cs2md_round_wins_t}
```

Player:

```text
{cs2md_player_name}
{cs2md_player_team} {cs2md_player_activity}
HP {cs2md_player_hp}
AR {cs2md_player_armor}
```

Weapon:

```text
{cs2md_weapon_name}
{cs2md_weapon_type}
{cs2md_weapon_state}
{cs2md_weapon_ammo_clip}/{cs2md_weapon_ammo_clip_max}
```

Bomb:

```text
BOMB {cs2md_bomb_state}
TIMER {cs2md_bomb_timer}
POS {cs2md_bomb_position}
```

## Connection And Status

| Variable | Placeholder | Type | Meaning |
| --- | --- | --- | --- |
| `cs2md.connected` | `{cs2md_connected}` | Bool | `true` after a real CS2 payload has been received. |
| `cs2md.status` | `{cs2md_status}` | String | Plugin/listener status. |

Status values:

| Value | Meaning |
| --- | --- |
| `starting` | Variables were initialized and the listener is starting. |
| `waiting_for_cs2` | The listener is running, but no real CS2 payload has been received yet. |
| `connected` | A real CS2 payload has been received and variables were updated. |
| `token_invalid` | CS2 sent a payload with a token that does not match the plugin token. |
| `port_in_use` | The plugin could not bind to the configured port. |
| `listener_offline` | The plugin is polling `/state`, but no listener is reachable. |
| `restarting` | The reset action is restarting the listener. |
| `error` | An unexpected error occurred while publishing state. |

## Default Variables

These groups are enabled by default.

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
| `cs2md.round.phase` | `{cs2md_round_phase}` | String |
| `cs2md.round.win_team` | `{cs2md_round_win_team}` | String |
| `cs2md.round.wins_ct` | `{cs2md_round_wins_ct}` | Integer |
| `cs2md.round.wins_t` | `{cs2md_round_wins_t}` | Integer |

### Player

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.player.steamid` | `{cs2md_player_steamid}` | String |
| `cs2md.player.name` | `{cs2md_player_name}` | String |
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

### Bomb

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.bomb.state` | `{cs2md_bomb_state}` | String |
| `cs2md.bomb.timer` | `{cs2md_bomb_timer}` | String |

## Optional Variables

Optional groups can be enabled in the plugin settings.

### Map Round Wins

| Pattern | Placeholder Example | Type |
| --- | --- | --- |
| `cs2md.rw.count` | `{cs2md_rw_count}` | Integer |
| `cs2md.rw.history` | `{cs2md_rw_history}` | String |
| `cs2md.rw.raw_json` | `{cs2md_rw_raw_json}` | String |
| `cs2md.rw.01` to `cs2md.rw.30` | `{cs2md_rw_01}` to `{cs2md_rw_30}` | String |

### Phase Countdowns

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.phase_countdowns.phase` | `{cs2md_phase_countdowns_phase}` | String |
| `cs2md.phase_countdowns.ends_in` | `{cs2md_phase_countdowns_ends_in}` | String |

### Player Position

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.player.position` | `{cs2md_player_position}` | String |
| `cs2md.player.forward` | `{cs2md_player_forward}` | String |

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

Replace `01` with `02` through `08` for other slots.

### Bomb Position / Carrier

| Variable | Placeholder | Type |
| --- | --- | --- |
| `cs2md.bomb.position` | `{cs2md_bomb_position}` | String |
| `cs2md.bomb.carrier` | `{cs2md_bomb_carrier}` | String |
| `cs2md.bomb.site` | `{cs2md_bomb_site}` | String |

`cs2md.bomb.site` is currently a compatibility alias for `bomb.position`; automatic A/B site detection is not implemented.

### Other Players

Observer/spectator payloads are exposed as `cs2md.ap01` through `cs2md.ap10`.

| Pattern | Placeholder Example | Type |
| --- | --- | --- |
| `cs2md.ap.count` | `{cs2md_ap_count}` | Integer |
| `cs2md.ap.raw_json` | `{cs2md_ap_raw_json}` | String |
| `cs2md.ap01.name` | `{cs2md_ap01_name}` | String |
| `cs2md.ap01.team` | `{cs2md_ap01_team}` | String |
| `cs2md.ap01.hp` | `{cs2md_ap01_hp}` | Integer |
| `cs2md.ap01.armor` | `{cs2md_ap01_armor}` | Integer |
| `cs2md.ap01.money` | `{cs2md_ap01_money}` | Integer |
| `cs2md.ap01.score` | `{cs2md_ap01_score}` | Integer |
| `cs2md.ap01.aw.name` | `{cs2md_ap01_aw_name}` | String |
| `cs2md.ap01.aw.ammo` | `{cs2md_ap01_aw_ammo}` | Integer |

Replace `01` with `02` through `10` for other players.

Each player also has weapon slot variables:

```text
cs2md.ap01.w01.name -> {cs2md_ap01_w01_name}
cs2md.ap01.w01.ammo -> {cs2md_ap01_w01_ammo}
```

Replace the first `01` for the player slot and the second `01` for that player's weapon slot.

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

Replace `01` with `02` through `16` for other grenade slots.

## Empty Values

Some variables are only available when CS2 sends that block.

Often available during normal gameplay:

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

If an optional variable is empty, the usual reason is that CS2 did not send that field in the current mode or camera state.
