---
layout: default
title: Toolkit Tweaks
nav_order: 10
---

# Toolkit Tweaks

<details open markdown="block">
  <summary>
    Table of Contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

## Buy Shortcutting

Internally, Twitch Toolkit has a loose concept called "shortcutting". Shortcutting is when a command acts
as a shortcut to purchasing a similiarly named event, like `!levelskill` being the same as
`!buy levelskill`. However, this concept is only available to `!levelskill`, so Utils loosened the
restriction to allow all commands to act as a shortcut. The only requirement is that the command's handler
point to Twitch Toolkit's `buy` handler, then the mod will do the logic of shortcutting.

## Karma Minimum

By default, Twitch Toolkit only allows the minimum karma a viewer can have to be set to -10. With Utils
enabled, the mod will lower the limit down to -2 billion. This is useful for particularly brutal setups
where viewers are severely punished for trying to destroy the colony.

## Pawn Kind Support

Prior to ToolkitUtils, viewers were only allowed to purchase humans. Users that wanted to allow viewers to
purchase different races would have to use dev mode to spawn in these pawns, assign them to viewers, then
deduct the coins manually. That sounded tedious to do in more popular streams, so the `pawn` event was
overhauled to include support for "pawn kinds", as the mod likes to put it. This means that viewers can
now specify what kind of pawn they want when purchasing a pawn. This also means that users can customize
the pricing of each individual pawn kind through its relevant config dialog.

## Color Tag Removal

Prior to ToolkitUtils, Twitch Toolkit wouldn't remove color tags from trait names, so viewers had to also
type the color tag whenever they wanted to add or remove a trait. Aside from being an inconvenience to
viewers, this also cluttered chat, so Utils overhauled the trait events to always filter out tags.

## Individual Trait Prices

With the addition of ToolkitUtils and the overhaul of the trait events, the possibly opened up to allow
users to customize the prices of adding and removing traits. The price of adding and removing traits can
be customized through the relevant config dialog.