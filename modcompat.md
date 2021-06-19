---
layout: default
title: Mod Compatibility
nav_order: 4
---

# Mod Compatibility

Utils expands upon Twitch Toolkit in a number of ways, including adding some varying level of support for
many popular mods. While most of the support extends or outright adds new content to the mod, a few are
only for ensuring a smooth experience while playing with those mods.

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

## RimWorld of Magic

The [A RimWorld of Magic](https://steamcommunity.com/sharedfiles/filedetails/?id=1201382956)
specific compatibility code includes a command for showing your pawn's class, and extending the trait
commands to better handle class changes. While they do work majority of scenarios, the feature itself is
still considered experimental as RimWorld of Magic's mod author could change any of the underlying code
the compatiblity code was built on, and as a result may be unpredictable between mod updates.

### Viewer Class

The relevant command (`!mypawnclass`) allows a viewer to check their current class' stats. The command
doesn't provide a way to check abilities, nor will the mod ever provide a way to adjust your abilities from
chat. The information displayed in this command includes the pawn's class name, current level, current
experience, experience cap for their current level, current stamina/mana, current stamina/mana gain, and any ability points available.

### Class Traits

This feature changes how the trait commands handle class traits. By default, the mod is set to forbid
class changes, and as a result this feature is effectively disabled. When class changes *are* allowed,
adding a class trait will forbid you from having multiple classes, and removing a class trait will perform
the same action as RimWorld of Magic's class removal gem.

## Interests

The [Interests](https://steamcommunity.com/workshop/filedetails/?id=2089938084) specific compatibility code
changes how passion events handle interests. Prior to this, the passion events would remove interests when
they modified a skill. With the patch, the passion events forbid or ignore modifying skills with interests.

## Simple Sidearms

The [Simple Sidearms](https://steamcommunity.com/sharedfiles/filedetails/?id=927155256) specific
compatibility code changes how the `!mypawngear` command displays information. With the patch, the command
will also include any equipped sidearms in the command's list of weapons.

## Immortals

The [Immortals](https://steamcommunity.com/sharedfiles/filedetails/?id=1984905966) specific compatibility
code adds a new event, called `immortality`, that allows viewers to purchase the mod's immortality. The
event doesn't include a way of increasing or decreasing the level of immortality a given pawn has -- it
only allows you to purchase the base level.

## Humanoid Alien Races

The [Humanoid Alien Races](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762) specific
compatibility code changes how the trait events handle race-specific settings the mod author has definied.
The patch only allows viewers to purchase traits that would otherwise be forbidden, or disable removing
traits that are forcibly active.