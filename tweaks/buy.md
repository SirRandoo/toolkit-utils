---
layout: default
title: Buy Shortcut Patch
parent: Toolkit Tweaks
---

Twitch Toolkit has a concept of "shortcut" commands, mainly the `!levelskill` command.
What this means is that using these commands _without_ including `!buy` will work as if
you did. However, this functionality was hardcoded to _only_ level skill, so Utils changes
this to include every command that wants that type of behavior. All you'll need to do as a
developer is point your command's command driver to Twitch Toolkit's `Buy` driver, and the
mod will do the rest.
