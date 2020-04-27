---
title: Events
nav_order: 5
layout: default
---

## Events

1. TOC
{:toc}

### Surgery

Usage: `!buy surgery {part}`

This evnet allows a viewer to purchase and queue an item from
the shop for surgery. It's important to note that this command
does not make any assumptions as to where a part should go. If
a colonist is missing an arm, it may end up queuing the surgery
for the other arm. It's also possible the command may queue the
surgery for a part that's an upgrade from the part specified.

It's also important to note that the cost of this event *does*
get added towards the final price a viewer will spend on a surgery.
If this event costs `500` coins, and a viewer requests an
`archotechleg`, the viewer will pay for the combined total of
the event **and** the archotech leg. It's considered a
"convenience fee."

### Passion Shuffle

Usage: `!buy passionshuffle [skill]`

This event allows a viewer to shuffle their pawn's current passions.
If a viewer specifies a skill, it'll have a guaranteed minor passion.

### Heal

Usage: `!buy healme`

This event is essentially a personal use healer mech serum.

### Revive

Usage: `!buy reviveme`

This event is essentially a personal resurrector mech serum.

### Heal All

Usage: `!buy healall`

This event is essentially a colony-wide healer mech serum.

### Revive All

Usage: `!buy reviveall`

This event is essentially a colony-wide resurrector mech serum.
