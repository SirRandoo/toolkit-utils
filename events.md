---
title: Events
nav_order: 5
layout: default
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

### Surgery

Usage: `!buy surgery {part}`

This event allows a viewer to purchase and queue an item from
the shop for surgery. The part the surgery will queue on will
always be the part with the least overall health, including any
attached parts.

The price of this event will be added towards the final cost
of the event. If the part requested costs 5000 coins, and the
event was set to 500 coins, the final total the viewer will
be charged will be 5500 coins.
{: .info}

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

### Heal Any Pawn

Usage: `!buy healanypawn`

This event is essentially a healer mech serum for a random colonist.

### Revive All

Usage: `!buy reviveall`

This event is essentially a colony-wide resurrector mech serum.

### Replace Trait

Usage: `!buy replacetrait {old_trait} {new_trait}`

This event allows a viewer to join the `trait` event and the `removetrait` event.

The price of this event will always be `1` as its real cost is the price for removing
`old_trait` and adding `new_trait` combined.
{: .info}
