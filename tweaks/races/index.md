---
layout: default
title: Race Support for Colonists
parent: Toolkit Tweaks
has_children: true
---

## Race Support for Colonists

ToolkitUtils extends upon Toolkit's pawn purchase event by allowing viewers
to specify the race of their colonist. This is done by including the race
after `!buy pawn`. For example, if you were to want an
[Android](https://steamcommunity.com/sharedfiles/filedetails/?id=1541064015)
as a colonst, you'd need to use the command `!buy pawn ChjAndroid` for a
standard android.

Users can disable race support via the settings menu for ToolkitUtils. You
can find the setting under the `General` settings category.

### Backwards Compatibility

In earlier versions of ToolkitUtils, the race had to be specified with
`--race=?`. While this style isn't required anymore, the mod does still
recognize the double-dash style. In a more recent version, Utils allowed
users to use a dashless style (`race=?`) to make it easier to remember,
but as of version 2.0.5.0, the mod allowed users to simply specify the
race as shown above (`!buy pawn ChjAndroid`). It's important to note that
the mod *does* still support earlier styles.
