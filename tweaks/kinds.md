---
layout: default
title: Kind Support for Colonists
parent: Toolkit Tweaks
---

This tweak fundamentally changes how Toolkit handles the pawn event.
{: .warn}

Without ToolkitUtils installed, you'd only be able to purchase human
colonists. Utils allows users to purchase kinds *other* than human.

If you manage to get past Utils' checks, you may encounter Toolkit's
cost handler if a kind is lower than the price the event is set to.
Toolkit's cost handler is responsible for ensuring viewers have the
appropriate amount of coins before an event's code will run. Setting
the pawn event to something other than 1 can mean that kinds *below*
the event's cost will *only* be usable once they have, at least, the
event's cost.
{: .danger}
