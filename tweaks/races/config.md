---
layout: default
title: Race Config
parent: Race Support for Colonists
grand_parent: Toolkit Tweaks
---

## Race Shop Config

The race shop config allows users to configure the price of each race,
and/or disable/enable races. It's important to note that the items listed
in this window do *not* correspond to the items listed in the item shop,
*but* ToolkitUtils will pull race prices from the item list.

### Races in the Item List

As mentioned above, the races in the item list do *not* correspond to
the races in the race list, *but* the race list will default to the
prices set in the item list. This is to ensure pre-existing prices get
migrated to where they should be. It's also important to note that
ToolkitUtils *does* disable races from appearing in the item list.

### Buy Pawn Event Config

Toolkit natively allows users to configure the price of events via the
incident editor. Should the cost be increased from `0`, ToolkitUtils
will update the price of **all** races to reflect the value inputted.
