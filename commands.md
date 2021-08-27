---
layout: default
title: Commands
nav_order: 4
---

# Commands

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

## Colony Commands

|----------------+------------+--------------+---------------------------------------------------------|
| Name           | User Level | Usage        | Description                                             |
|:---------------|:-----------|:-------------|:--------------------------------------------------------|
| Colonist count | Anyone     | `!colonists` | Displays the current number of colonists in the colony. |
|----------------+------------+--------------+---------------------------------------------------------|
| Factions       | Anyone     | `!factions`  | Displays a list of factions in the game, as well as the colony's current goodwill with them. |
|----------------+------------+--------------+---------------------------------------------------------|
| Wealth         | Anyone     | `!wealth`    | Displays the wealth of the currently visible map.       |
|----------------+------------+--------------+---------------------------------------------------------|

## Pawn Commands

|--------------------+------------+---------------------------------------------------------------+-------------------|
| Name               | User Level | Usage                                                         | Description       |
|:-------------------|:-----------|:--------------------------------------------------------------|:------------------|
| Abandon pawn       | Anyone     | `!leave`                                                      | Forces your colonist to leave the colony. Depending on the settings enabled, the pawn will drop their inventory prior to leaving. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Insult pawn        | Anyone     | `!insult`<br/>`!insult <viewer>`                              | Insults the specified viewer, or a random pawn if unspecified. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn class         | Anyone     | `!mypawnclass`                                                | Displays a viewer's pawn's current A RimWorld of Magic class. This command is only usable when A RimWorld of Magic is enabled. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn body          | Anyone     | `!mypawnbody`                                                 | Displays all current hediffs a viewer's pawn has. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn gear          | Anyone     | `!mypawngear`                                                 | Displays a viewer's pawn's currently equipped gear, and armor rating. All displayed information is configurable in the mod's settings page. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn health        | Anyone     | `!mypawnhealth`<br/>`!mypawnhealth <capacity>`                | Displays an overview of viewer's pawn's health. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn kills         | Anyone     | `!mypawnkills`                                                | Displays the list of kills a viewer's pawn has killed this game. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn needs         | Anyone     | `!mypawnneeds`                                                | Displays a viewer's pawn's needs and their current satisfaction. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn relations     | Anyone     | `!mypawnrelations`<br/>`!mypawnrelations <viewer>`            | Displays a overview of the viewer's pawn's relationships. Depending on the settings enabled, this command may only show named relations, like spouse or rival. If `viewer` is specified, the command will instead show the opinion the viewer's pawn has of the other viewer's pawn, and vice versa. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn stats         | Anyone     | `!mypawnstats [stat_name [stat_name ...]]`                    | Displays a viewer's pawn's combat stats if no stats were specified, or only the stats specified. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Pawn work          | Anyone     | `!mypawnwork [WORK=PRIORITY [WORK=PRIORITY ...]]`             | Displays a viewer's pawn's work settings. Depending on the settings enabled, disabled work types would be hidden from the response, as well as sorted. Viewers can also change their pawn's work settings by specifying `WORK`, where `WORK` is the name of the work type, and `PRIORITY`, where priority is the number from 0-4. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Relink pawn        | Anyone     | `!fixmypawn`                                                  | Relinks a viewer to their pawn internally. This does **not** reconnect, nor otherwise connect, on Puppeteer. |
|--------------------+------------+---------------------------------------------------------------+-------------------|
| Set favorite color | Anyone     | `!setfavoritecolor <color>`<br/>`!setfavoritecolor <hexcode>` | Sets the viewer's pawn's favorite color to the color specified. This command requires Ideology to be installed and enabled before it'll be available for use. |
|--------------------+------------+---------------------------------------------------------------+-------------------|

## Misc Commands

|----------------+------------+---------------------------------------------------+--------------------|
| Name           | User Level | Usage                                             | Description        |
|:---------------|:-----------|:--------------------------------------------------|:-------------------|
| Price check    | Anyone     | `!price <category> <product> [quantity]`<br/>`!price <item> [quantity]` | Displays the price of the product in the specified category. If no category is specified, the `item` category will be used instead. |
|----------------+------------+---------------------------------------------------+--------------------|
| Relink pawns   | Moderator  | `!fixallpawns`                                    | Relinks all viewers to their pawns internally. This does **not** reconnect, nor connect, them on Puppeteer. |
|----------------+------------+---------------------------------------------------+--------------------|
| Toggle shop    | Moderator  | `!togglestore`                                    | Toggles viewer's ability to purchase anything from Toolkit's store. |
|----------------+------------+---------------------------------------------------+--------------------|
