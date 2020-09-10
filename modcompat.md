---
layout: default
title: Mod Compatibility
nav_order: 4
---

ToolkitUtils includes a few mod-specific compatiblity code, like
its
[RimWorld of Magic](https://steamcommunity.com/sharedfiles/filedetails/?id=1201382956)
specific commands. While most mods will never
be supported, Utils aims to iron out some painful situations when
specific mods are included, like a necromancer's raised undead counting
as unnamed colonists. It'll never write compatiblity for *every* mod
in the workshop, current or future; it'll only include compatiblity for
major incompatiblities, like being able to swap classes with RimWorld of
Magic.

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

### RimWorld of Magic

As mention in the opening paragraph, ToolkitUtils includes some baked in
compatiblity for with RimWorld of Magic (TMagic). In addition, it adds some
restrictions on what viewers can and can't do with their class traits, outside
of the streamer disabling specific traits from being added or removed.

#### Pawn Class

One feature is the ability to see your pawn's current class, experience level,
resource (stamina/mana), and available ability points. If your pawn doesn't have
a class, the command will simply reply with a message stating so. Command features
aside, this command will *only* be visible when TMagic is active for obvious
reasons.

#### Class Restrictions

In an effort to limit potential damage caused by class swapping, Utils installed a
hard restriction on how classes work with TMagic. Adding and removing a class is
as simple as adding the trait, but once you have a proper class, not physically
adept or magically gifted, your pawn will not be able to pick a different class.
TMagic is simply not built to support this kind of functionality Twitch Toolkit
inadvertently exposed.

### Interests

[Interests](https://steamcommunity.com/workshop/filedetails/?id=2089938084) support
is also baked into Utils via its `!passionshuffle` command. When a viewer uses
passion shuffle while Interests is active, the command will also shuffle their
interests randomly. Interest placement is ***not*** affected by the viewer
specifying a skill they would like a passion in.

### Simple Sidearms

[Simple Sidearms](https://steamcommunity.com/sharedfiles/filedetails/?id=927155256)
support is solely centered around the `!mypawngear` command including all sidearms
a pawn is currently holding.

### Immortals

[Immortals](https://steamcommunity.com/sharedfiles/filedetails/?id=1984905966)
support is added via the `immortality` event; this event is only usable when
Immortals is active. When a viewer purchases this event, the viewer will be given
the immortality hediff, but it'll be given in the same manner pawns would normally
spawn with it as, i.e. a hidden hediff that'll only appear in the pawn's health tab
_after_ they've died and resurrected.

### Humanoid Alien Races

[Humanoid Alien Races](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762)
support is added via the `pawn`, `trait`, `removetrait`, and `replacetrait` events.

The pawn event allows viewers to specify what _kind_ of pawn they would like by
passing in the kind's name via `!buy pawn [kind]`. Kinds support is documented in
the [pawn tweak page]({{- "/tweaks/kinds" | relative_url -}}).

The trait events differ in that they follow trait restrictions set up in the kind's
def. A good example is the [Equium](https://steamcommunity.com/sharedfiles/filedetails/?id=1211945995)
mod that forbids "Sanguine" from being added to equiums since it's a "built-in"
trait. Prior to this, the events would take your coins and you'd be left wondering
why a trait isn't on your pawn.
