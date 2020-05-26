---
layout: default
title: Traits
parent: Toolkit Tweaks
---

This tweak fundamentally changes how Toolkit handles trait events.
{: .warn}

Without ToolkitUtils installed, all traits would cost the same
amount of coins to add and remove. Utils allows users to set the
cost to add and remove traits *per* trait. With this change, Utils
forcibly sets all trait events to cost 1 coin.

If you manage to get past Utils' checks, you may encounter Toolkit's
cost handler if a trait is lower than the price the event is set to.
Toolkit's cost handler is responsible for ensuring viewers have the
appropriate amount of coins before an event's code will run. Setting
a trait event to something other than 1 can mean that traits *below*
the event's cost will *only* be usable once they have, at least, the
event's cost.
{: .danger}
