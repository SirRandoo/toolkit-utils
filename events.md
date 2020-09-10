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

Usage: `!buy surgery {part} [quantity]` OR `!buy surgery {part} [body part]`

This event allows a viewer to purchase and queue an item from
the shop for surgery. The part the surgery will queue on will
always be the part with the least overall health, including any
attached parts.

This event's cost will be the same as the item being queued.
For example, if a viewer wants an archotech leg to be installed,
which hypothetically cost 3500 coins, the viewer will have
to pay 3500 coins to queue the surgery.
{: .info}

### Full Heal

Usage: `!buy fullheal`

This event mimics `heal`, but continues to heal all your injuries so long as you can afford
them. What this means is that for every part healed, it'll remove whatever amount this event
costs from your current balance.

For example, if you have 5 healable injuries, and this event
costs 500 coins, you'll be charged 2500 coins when the command is finished. If you only have
enough for 3 out of the 5 injuries, you'll only be charged 1500 coins.
{: .info}

### Heal All

Usage: `!buy healall`

This event is essentially a colony-wide healer mech serum.

### Heal

Usage: `!buy healme`

This event is essentially a personal use healer mech serum.

### Heal Any Pawn

Usage: `!buy healanypawn`

This event is essentially a healer mech serum for a random colonist.

### Immortality

Usage: `!buy immortality`

This event grants the `immortality` hediff to the viewer's pawn upon purchase.
While the hediff is applied, it'll remain hidden until the pawn dies and subsequently resurrects.

This event is only purchasable when the Immortals mod is active.
{: .warn}

### Passion Shuffle

Usage: `!buy passionshuffle [skill]`

This event allows a viewer to shuffle their pawn's current passions.
If a viewer specifies a skill, it'll have a guaranteed minor passion.

### Replace Trait

Usage: `!buy replacetrait {old_trait} {new_trait}`

This event allows a viewer to join the `trait` event and the `removetrait` event.

The price of this event will always be `1` as its real cost is the price for removing
`old_trait` and adding `new_trait` combined.
{: .info}

### Rescue Me

Usage: `!buy rescueme`

This event creates a prisoner rescue quest with a kidnapped viewer's pawn as the captive.
The pawn chosen will be the oldest pawn in the game's kidnapped list.

### Revive All

Usage: `!buy reviveall`

This event is essentially a colony-wide resurrector mech serum.

### Revive

Usage: `!buy reviveme`

This event is essentially a personal resurrector mech serum.
