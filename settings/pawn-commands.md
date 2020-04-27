---
layout: default
title: Pawn Commands
parent: Settings
---

## Pawn Commands

The pawn command settings refer to settings that modify how ToolkitUtils'
`!mypawn` commands function.

![Pawn Command Settings]({{- "/assets/cfg/pawn_commands.png" | relative_url -}})

The above image depicts the current list of pawn command settings that
can be found in-game in ToolkitUtils' settings menu. Below is an explanation
of each setting, and how they affect the mod.

### Temperature in Gear

Enabling this setting instructs the mod to migrate temperature information
into `!mypawngear` instead of `!mypawnbody`.

### Show Apparel in Gear

Enabling this setting instructs the mod to show the current apparel a pawn
is wearing in `!mypawngear`.

### Armor in Gear

Enabling this setting instructs the mod to show a pawn's current armor
protection ratings in `!mypawngear`.

### Weapon in Gear

Enabling this setting instructs the mod to show a pawn's currently equipped
weapon in `!mypawngear`.

### Queued Surgeries in Health

Enabling this setting instructs the mod to show a pawn's currently queued
surgeries in `!mypawnhealth`.

### Sort Priorities in Work

Enabling this setting instructs the mod to sort `!mypawnwork`'s output
according to the order they'd be executed.

### Filter Priorities in Work

Enabling this setting instructs the mod to filter disabled priorities from
`!mypawnwork`'s output.

### Leave Method

This setting changes how pawns will leave the colony when the `!leave`
command is used by a viewer. By default, the command uses a pseudo mental
break.

The possible leave methods are:

| Leave Method                  | Description                                           |
|:-----------------------------:|:------------------------------------------------------|
| Mental Break                  | The viewer's old pawn will be turned into a wild man. |
| Thanos                        | The viewer's old pawn will turn to ash.               |

### Drop Inventory When Leaving

Enabling this setting instructs the mod to drop a pawn's current inventory
when a viewer uses the `!leave` command. It's important to note that this
setting does ***not*** appear when the [leave method](#leave-method) is set
to `Thanos`.
