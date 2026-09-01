# Variabili di CS2 GSI per Macro Deck

Questa pagina spiega come usare le variabili di CS2 GSI per Macro Deck nei pulsanti di Macro Deck.

**🌐 Leggi in: [English](VARIABLES.md) · [Italiano](VARIABLES.it.md)**

## Formato dei Placeholder

I nomi delle variabili del plugin usano i punti:

```text
cs2md.player.hp
```

I placeholder dei pulsanti di Macro Deck usano trattini bassi:

```text
{cs2md_player_hp}
```

Esempi:

| Variabile plugin | Placeholder Macro Deck |
| --- | --- |
| `cs2md.map.name` | `{cs2md_map_name}` |
| `cs2md.round.phase` | `{cs2md_round_phase}` |
| `cs2md.player.hp` | `{cs2md_player_hp}` |
| `cs2md.weapon.name` | `{cs2md_weapon_name}` |
| `cs2md.bomb.state` | `{cs2md_bomb_state}` |

## Esempi Rapidi di Pulsanti

Stato live di base:

```text
{cs2md_map_name}
{cs2md_round_phase}
CT {cs2md_round_wins_ct} / T {cs2md_round_wins_t}
```

Giocatore:

```text
{cs2md_player_name}
{cs2md_player_team} {cs2md_player_activity}
HP {cs2md_player_hp}
AR {cs2md_player_armor}
```

Arma:

```text
{cs2md_weapon_name}
{cs2md_weapon_type}
{cs2md_weapon_state}
{cs2md_weapon_ammo_clip}/{cs2md_weapon_ammo_clip_max}
```

Bomba:

```text
BOMB {cs2md_bomb_state}
TIMER {cs2md_bomb_timer}
POS {cs2md_bomb_position}
```

## Connessione e Stato

| Variabile | Placeholder | Tipo | Significato |
| --- | --- | --- | --- |
| `cs2md.connected` | `{cs2md_connected}` | Bool | `true` dopo che è stato ricevuto un payload reale da CS2. |
| `cs2md.status` | `{cs2md_status}` | String | Stato del plugin/listener. |

Valori di stato:

| Valore | Significato |
| --- | --- |
| `starting` | Le variabili sono state inizializzate e il listener sta partendo. |
| `waiting_for_cs2` | Il listener è attivo, ma non è stato ancora ricevuto alcun payload reale da CS2. |
| `connected` | È stato ricevuto un payload reale da CS2 e le variabili sono state aggiornate. |
| `token_invalid` | CS2 ha inviato un payload con un token che non corrisponde a quello del plugin. |
| `port_in_use` | Il plugin non ha potuto agganciarsi alla porta configurata. |
| `listener_offline` | Il plugin sta interrogando `/state`, ma nessun listener è raggiungibile. |
| `restarting` | L'azione di reset sta riavviando il listener. |
| `error` | Si è verificato un errore imprevisto durante la pubblicazione dello stato. |

## Variabili Predefinite

Questi gruppi sono abilitati per impostazione predefinita.

### Provider

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.provider.name` | `{cs2md_provider_name}` | String |
| `cs2md.provider.appid` | `{cs2md_provider_appid}` | Intero |
| `cs2md.provider.version` | `{cs2md_provider_version}` | Intero |
| `cs2md.provider.steamid` | `{cs2md_provider_steamid}` | String |
| `cs2md.provider.timestamp` | `{cs2md_provider_timestamp}` | Intero |

### Mappa e Round

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.map.name` | `{cs2md_map_name}` | String |
| `cs2md.map.mode` | `{cs2md_map_mode}` | String |
| `cs2md.map.phase` | `{cs2md_map_phase}` | String |
| `cs2md.map.round` | `{cs2md_map_round}` | Intero |
| `cs2md.round.phase` | `{cs2md_round_phase}` | String |
| `cs2md.round.win_team` | `{cs2md_round_win_team}` | String |
| `cs2md.round.wins_ct` | `{cs2md_round_wins_ct}` | Intero |
| `cs2md.round.wins_t` | `{cs2md_round_wins_t}` | Intero |

### Giocatore

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.player.steamid` | `{cs2md_player_steamid}` | String |
| `cs2md.player.name` | `{cs2md_player_name}` | String |
| `cs2md.player.activity` | `{cs2md_player_activity}` | String |
| `cs2md.player.team` | `{cs2md_player_team}` | String |
| `cs2md.player.hp` | `{cs2md_player_hp}` | Intero |
| `cs2md.player.armor` | `{cs2md_player_armor}` | Intero |
| `cs2md.player.helmet` | `{cs2md_player_helmet}` | Bool |
| `cs2md.player.defusekit` | `{cs2md_player_defusekit}` | Bool |
| `cs2md.player.flashed` | `{cs2md_player_flashed}` | Intero |
| `cs2md.player.smoked` | `{cs2md_player_smoked}` | Intero |
| `cs2md.player.burning` | `{cs2md_player_burning}` | Intero |
| `cs2md.player.alive` | `{cs2md_player_alive}` | Bool |
| `cs2md.player.money` | `{cs2md_player_money}` | Intero |
| `cs2md.player.kills_round` | `{cs2md_player_kills_round}` | Intero |
| `cs2md.player.headshot_kills_round` | `{cs2md_player_headshot_kills_round}` | Intero |
| `cs2md.player.kills_total` | `{cs2md_player_kills_total}` | Intero |
| `cs2md.player.assists` | `{cs2md_player_assists}` | Intero |
| `cs2md.player.deaths` | `{cs2md_player_deaths}` | Intero |
| `cs2md.player.mvps` | `{cs2md_player_mvps}` | Intero |
| `cs2md.player.score` | `{cs2md_player_score}` | Intero |
| `cs2md.player.equip_value` | `{cs2md_player_equip_value}` | Intero |

### Arma Corrente

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.weapon.name` | `{cs2md_weapon_name}` | String |
| `cs2md.weapon.type` | `{cs2md_weapon_type}` | String |
| `cs2md.weapon.paintkit` | `{cs2md_weapon_paintkit}` | String |
| `cs2md.weapon.state` | `{cs2md_weapon_state}` | String |
| `cs2md.weapon.ammo_clip` | `{cs2md_weapon_ammo_clip}` | Intero |
| `cs2md.weapon.ammo_clip_max` | `{cs2md_weapon_ammo_clip_max}` | Intero |
| `cs2md.weapon.ammo_reserve` | `{cs2md_weapon_ammo_reserve}` | Intero |

### Bomba

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.bomb.state` | `{cs2md_bomb_state}` | String |
| `cs2md.bomb.timer` | `{cs2md_bomb_timer}` | String |

## Variabili Opzionali

I gruppi opzionali possono essere abilitati nelle impostazioni del plugin.

### Vittorie di Round per Mappa

| Pattern | Esempio di Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.rw.count` | `{cs2md_rw_count}` | Intero |
| `cs2md.rw.history` | `{cs2md_rw_history}` | String |
| `cs2md.rw.raw_json` | `{cs2md_rw_raw_json}` | String |
| `cs2md.rw.01` fino a `cs2md.rw.30` | `{cs2md_rw_01}` fino a `{cs2md_rw_30}` | String |

### Countdown di Fase

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.phase_countdowns.phase` | `{cs2md_phase_countdowns_phase}` | String |
| `cs2md.phase_countdowns.ends_in` | `{cs2md_phase_countdowns_ends_in}` | String |

### Posizione del Giocatore

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.player.position` | `{cs2md_player_position}` | String |
| `cs2md.player.forward` | `{cs2md_player_forward}` | String |

### Slot Armi del Giocatore Corrente

L'inventario del giocatore corrente è esposto come `cs2md.pw01` fino a `cs2md.pw08`.

| Pattern | Esempio di Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.pw.count` | `{cs2md_pw_count}` | Intero |
| `cs2md.pw.raw_json` | `{cs2md_pw_raw_json}` | String |
| `cs2md.pw01.slot` | `{cs2md_pw01_slot}` | String |
| `cs2md.pw01.name` | `{cs2md_pw01_name}` | String |
| `cs2md.pw01.type` | `{cs2md_pw01_type}` | String |
| `cs2md.pw01.paint` | `{cs2md_pw01_paint}` | String |
| `cs2md.pw01.state` | `{cs2md_pw01_state}` | String |
| `cs2md.pw01.ammo` | `{cs2md_pw01_ammo}` | Intero |
| `cs2md.pw01.ammo_max` | `{cs2md_pw01_ammo_max}` | Intero |
| `cs2md.pw01.reserve` | `{cs2md_pw01_reserve}` | Intero |

Sostituisci `01` con `02` fino a `08` per gli altri slot.

### Posizione / Carrier della Bomba

| Variabile | Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.bomb.position` | `{cs2md_bomb_position}` | String |
| `cs2md.bomb.carrier` | `{cs2md_bomb_carrier}` | String |
| `cs2md.bomb.site` | `{cs2md_bomb_site}` | String |

`cs2md.bomb.site` è attualmente un alias di compatibilità per `bomb.position`; il rilevamento automatico del sito A/B non è implementato.

### Altri Giocatori

I payload da osservatore/spettatore sono esposti come `cs2md.ap01` fino a `cs2md.ap10`.

| Pattern | Esempio di Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.ap.count` | `{cs2md_ap_count}` | Intero |
| `cs2md.ap.raw_json` | `{cs2md_ap_raw_json}` | String |
| `cs2md.ap01.name` | `{cs2md_ap01_name}` | String |
| `cs2md.ap01.team` | `{cs2md_ap01_team}` | String |
| `cs2md.ap01.hp` | `{cs2md_ap01_hp}` | Intero |
| `cs2md.ap01.armor` | `{cs2md_ap01_armor}` | Intero |
| `cs2md.ap01.money` | `{cs2md_ap01_money}` | Intero |
| `cs2md.ap01.score` | `{cs2md_ap01_score}` | Intero |
| `cs2md.ap01.aw.name` | `{cs2md_ap01_aw_name}` | String |
| `cs2md.ap01.aw.ammo` | `{cs2md_ap01_aw_ammo}` | Intero |

Sostituisci `01` con `02` fino a `10` per gli altri giocatori.

Ogni giocatore ha anche variabili per gli slot delle armi:

```text
cs2md.ap01.w01.name -> {cs2md_ap01_w01_name}
cs2md.ap01.w01.ammo -> {cs2md_ap01_w01_ammo}
```

Sostituisci il primo `01` per lo slot del giocatore e il secondo `01` per lo slot arma di quel giocatore.

### Granate

I payload delle granate da osservatore/spettatore sono esposti come `cs2md.g01` fino a `cs2md.g16`.

| Pattern | Esempio di Placeholder | Tipo |
| --- | --- | --- |
| `cs2md.g.count` | `{cs2md_g_count}` | Intero |
| `cs2md.g.raw_json` | `{cs2md_g_raw_json}` | String |
| `cs2md.g01.id` | `{cs2md_g01_id}` | String |
| `cs2md.g01.owner` | `{cs2md_g01_owner}` | String |
| `cs2md.g01.type` | `{cs2md_g01_type}` | String |
| `cs2md.g01.pos` | `{cs2md_g01_pos}` | String |
| `cs2md.g01.vel` | `{cs2md_g01_vel}` | String |
| `cs2md.g01.life` | `{cs2md_g01_life}` | String |
| `cs2md.g01.effect` | `{cs2md_g01_effect}` | String |
| `cs2md.g01.flames` | `{cs2md_g01_flames}` | Intero |
| `cs2md.g01.raw_json` | `{cs2md_g01_raw_json}` | String |

Sostituisci `01` con `02` fino a `16` per gli altri slot delle granate.

## Valori Vuoti

Alcune variabili sono disponibili solo quando CS2 invia quel blocco.

Spesso disponibili durante il gameplay normale:

- `provider`
- `map`
- `round`
- `player_id`
- `player_state`
- `player_weapons`
- `player_match_stats`

Spesso solo da osservatore/spettatore secondo Valve:

- `allplayers_*`
- `allgrenades`
- `bomb.position`
- `bomb.carrier`
- `phase_countdowns`
- `player.position`
- `player.forward`

Se una variabile opzionale è vuota, il motivo usuale è che CS2 non ha inviato quel campo nella modalità o nello stato della camera corrente.
