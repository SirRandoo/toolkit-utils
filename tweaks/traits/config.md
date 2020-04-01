---
layout: default
title: Trait Config
parent: Trait Tweaks
grand_parent: Toolkit Tweaks
---

## Trait Shop Config

The trait shop config allows users to specify the amount of coins a
viewer must pay for adding a trait, the amount to remove a trait,
whether or not a trait *can* be removed, and whether or not a trait
*can* be added.

### Trait Event Config

Toolkit natively allows users to configure the price of events via
the incident editor. Should the cost be increased from `0`, ToolkitUtils
will update the price of **all** traits to reflect the value inputted.
If you were to change the `trait` event's cost to `1500`, all traits will
cost `1500` coins to add to a pawn. Likewise, changing the `removetrait`
event's cost to `1500` would change the removal price of all traits to
`1500` coins.
