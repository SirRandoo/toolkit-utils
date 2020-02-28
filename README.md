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


### Colony Commands

- !research {project}
  - Shows the percentage towards a specified research.
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
- Pawn Relinker
  - Ensures active viewer's pawns are linked to them.
  - *This feature is considered experimental, and may be removed at any moment.*