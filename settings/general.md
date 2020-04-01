---
layout: default
title: General
parent: Settings
---

## General

The general settings refer to settings that don't have a specific category,
are too few to group into a new category, or simply fit best in general.

![General Settings](/assets/settings-general.png)

The above image depicts the current list of general settings that can be
found in-game in ToolkitUtils' settings menu. Below is an explanation of
each setting, and how they affect the mod.

### Mod Versions in Installed Mods

Enabling this setting changes how the `!installedmods` command outputs the
mod list. By default, the mod only outputs mod names, with the exception of
Toolkit's version. When this setting is enabled, the command will instead
output `{mod_name} (v{mod_version})`.

### Emoticons

This setting determines how commands that use emoticons output information.
By default, the mod attempts to use emoticons whenever possible to minimize
the total length of command responses. It's understandable to want to disable
them if viewers have trouble viewing some of the more newer emoticons, that's
why ToolkitUtils allows viewers to use the `--text` flag to swap to a
text-only version of the command it's used on.

### Star Utils in Installed Mods

This setting just adds a little star next to ToolkitUtils in `!installedmods`
to help locate it better. If [emoticons](#emoticons) is disabled, this will
instead use an asterisk (`*`).

### Sexuality Traits

This setting changes how the mod handles sexuality traits. Prior to this
setting, sexuality traits would count towards the trait limit, which made
building dream pawns a little limited. When this setting is enabled, the mod
will allow viewers to add a sexuality trait without it counting towards
the trait limit. It's imporant to note that this change *only* supports
vanilla RimWorld's traits. Modded content will still count towards the limit.

### Pawn Races

This setting allows viewers to specify a race for their pawn. For more
information about pawn races, please refer to the [race tweak](/tweaks/races)
documentation.

### Lookup Results

This setting allows users to change how many maximum results will appear in
the `!lookup` command's output. It's recommended to keep this at a reasonable
value since this does *can* lead to text blocks.

### Shop File as Json

This setting instructs the mod to output ToolkitUtils' shop file as an
additional json file. Enabling this setting does **not** swap how Utils
stores its shop information. Modifying the json file will **not** affect
items in Utils' shop. This file exists as a means to Utils' shop into an
item list.
