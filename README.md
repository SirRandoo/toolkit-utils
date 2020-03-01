# Toolkit Utils

A collection of commands for TwitchToolkit.



### Pawn Commands

- !mypawnbody
  - Shows information about a viewer's pawn's body, like diseases.
- !mypawnhealth {capacity}
  - Shows information about a viewer's pawn's health, like their manipulation.
  - If an optional *capacity* is provided, the command will show information about what's affecting the specified capacity.
    - A list of capacities can be seen when *capacity* is excluded.
- !mypawnneeds
  - Shows information about a viewer's pawn's needs.
- !mypawnwork
  - Shows information about a viewer's pawn's current work priorities.
- !mypawngear
  - Shows information about a viewer's pawn's current gear, armor rating, and optionally temperature tolerance.
- !fixmypawn
  - Attempts to relink a viewer's pawn.
  - Useful in cases where a mod nukes pawn instances.


### Colony Commands

- !research {project}
  - Shows the percentage towards a specified research.
  - If a research contains unfinished prerequisites, they will be appended to the response.
- !factions
  - Shows a list of visible factions, and their current goodwill towards the colony.


### Purchasable Events

- !buy reviveall
  - Revives every dead colonist.
- !buy revive
  - Revives the viewer's pawn.
- !buy heal
  - Emulates a healer mech serum, but cuts out the streamer having to tell a viewer's pawn to use it.


### Toolkit Tweaks

- !buy trait
  - The trait event now ignores sexuality traits, like bisexuality, when determining how many total traits a viewer's pawn currently has.
- !installedmods
  - The command now pulls from a pregenerated cache at start up.
  - Can be configured to include mod versions
- Command parser
  - Automatically captures & logs errors from any command the parser invokes.
  - Allows you to change the prefix the bot will look for.
- Founders support
  - Includes a patch to support the new `founders` badge.
  - This does **not** mean founders will be able to have special settings.
