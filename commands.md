---
layout: default
title: Commands
nav_order: 4
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

### Price

Usage: `!price {category} {query} [quantity]`

This command is simliar to `!lookup`, except that it *only* displays
one result. The result includes shop information, like the cost to
purchase. If a specific quantity is specified, it'll show the amount of
coins required to purchase the specified amount of the specified item.

This command does not perform the same lookup  as `!lookup`. The query
provided *must* match the result from `!lookup` or the command will not
display anything.
{: .warn}

### Factions

Usage: `!factions`

This command displays the colony's current faction relations.

### Research

Usage: `!research [project]` OR `!research [itemname]`

This command displays the colony's status on research projects. By
default, this command displays the current research project, but
you can see the progress on a specific project by specifying the
name of the project, or an item from said project. If a research
has prerequisites, the command will include the current progress
towards those prerequisites.

### Fix Pawns

Usage: `!fixmypawn` OR `!fixallpawns`

These commands are used to relink viewers to their pawns. There
exists two versions of this command, `!fixmypawn` and `!fixallpawns`.
The former relinks the person using the command to their pawn,
while the latter relinks *every* viewer to their pawns. 

These commands do ***not*** work well when there's multiple colonists
with the same name. If there happens to be, it'll link the viewer to
the *first* colonist named after a viewer from left to right.
{: .warn}

### Leave

Usage: `!leave`

This command allows a viewer to leave the colony without having to
wait until their pawn is killed. For information about the leave
command's settings, please refer to its [settings]({{- "/settings/pawn-commands#leave-method" | relative_url -}})
documentation.

This command does ***not*** refund any coins spent on a pawn.
{: .warn}

### Pawn Gear

Usage: `!mypawngear`

This command displays information about a pawn's gear. For information
about the gear command's settings, please refer to the [pawn commands]({{- "/settings/pawn-commands" | relative_url -}})
documentation.

Information displayed includes:

- The current comfortable temperature range a colonist can withstand
- The pawn's current armor rating values
- The current weapon a pawn is holding
- A list of currently worn apparel

### Pawn Body

Usage: `!mypawnbody`

This command displays information about a pawn's body. Informaton displayed
includes:

- The current comfortable temperature range a colonist can withstand
- An list of health conditions the colonist is suffering from
- Whether or not a colonist is currently bleeding

### Pawn Work

Usage: `!mypawnwork`

This command displays information about a pawn's current work priorities.
For information
about the gear command's settings, please refer to the [pawn commands]({{- "/settings/pawn-commands" | relative_url -}})
documentation.

### Pawn Needs

Usage: `!mypawnneeds`

This command displays information about a pawn's current needs in the form
of a percentage value.

### Pawn Health

Usage: `!mypawnhealth [capacity]`

This command displays information about a pawn's current health. If no
capacity is given, the command will display a list of capacities and
their effective working percentage. If a capacity is given, the command
will display what is currently affecting said capacity. The information
displayed when no capacity is given includes:

- An overall health percentage
- Whether or not the colonist is downed
- Their mood in the form of an emoticon
  - This is replaced with a colonist is downed
- Whether or not the colonist is bleeding out
  - If a colonist is about to bleed out, it'll display the time remaining
- A list of capacities and their effective working percentage
- [Queued surgeries]({{- "/settings/pawn-commands" | relative_url -}})
  - Disabled by default

### Pawn Kills

Usage: `!mypawnkills`

This command displays the current number of kills your pawn has accumulated.

### Pawn Class

Usage: `!mypawnclass`

This command displays a brief overview of your [RimWorld of Magic](https://steamcommunity.com/sharedfiles/filedetails/?id=1201382956) class.
The information display includes:

- Your current class
- Your current level
- Your current mana
- Your current maxmimum mana
- Your current mana generation rate
- Your current experience
- The amount of experience needed to level up

### Insult

Usage: `!insult [viewer]`

This command instructs your pawn to insult the viewer specified.
If no viewer is specified, your pawn will select a random viewer.

This command does not guarantee you'll successfully insult someone.
{: .warn}

This command will remove a small amount of karma from the user even
if the insult job failed.
{: .danger}

### Pawn Stats

Usage: `!mypawnstats {category}`

This command displays the stats registered to the stat category
specified. A list of base game categories include:

- Basics
- Combat
- Misc
- Social

This command's output can be configured in ToolkitUtils' settings menu.
{: .info}
