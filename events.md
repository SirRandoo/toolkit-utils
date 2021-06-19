---
title: Events
nav_order: 5
layout: default
---

# Events

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

## Pawn Events

|------------------+---------------------------------------+--------------------------------------------|
| Name             | Usage                                 | Description                                |
|:-----------------+:--------------------------------------|:-------------------------------------------|
| Backpack         | `!buy backpack <item> [quantity]`     | Purchases and shoves the specified item in the viewer's pawn's backpack. |
|------------------+---------------------------------------+--------------------------------------------|
| Clear traits     | `!buy cleartraits`                    | Removes all eligible traits.               |
|------------------+---------------------------------------+--------------------------------------------|
| Equip weapon     | `!buy equip <item>`                   | Purchases and forces a viewer's pawn to equip the specified weapon. If the pawn already has a weapon equipped, it will be dropped on the floor. |
|------------------+---------------------------------------+--------------------------------------------|
| Full heal        | `!buy fullheal`                       | Heals as many hediffs as a viewer can afford. When the command heals a hediff, the viewer will be charged. This process will repeat until there are no more hediffs, or the viewer can't afford another heal. |
|------------------+---------------------------------------+--------------------------------------------|
| Heal any pawn    | `!buy healanypawn`                    | Heals one hediff on a random colonist.     |
|------------------+---------------------------------------+--------------------------------------------|
| Heal me          | `!buy healme`                         | Heals one hediff on the viewer's pawn.     |
|------------------+---------------------------------------+--------------------------------------------|
| Immortality      | `!buy immortality`                    | Gives a viewer's pawn the `immortal` hediff. Note: this event only gives you the base level, which is by default hidden until the pawn dies. |
|------------------+---------------------------------------+--------------------------------------------|
| Passion shuffle  | `!buy passionshuffle [skill]`         | Shuffles a viewer's pawn's passions. If `skill` is specified, the pawn will have a guaranteed passion in that skill. If the [Interests](https://steamcommunity.com/workshop/filedetails/?id=2089938084) mod is enabled, interests will also be shuffled. |
|------------------+---------------------------------------+--------------------------------------------|
| Random adulthood | `!buy randomadulthood`                | Replaces a viewer's pawn's adulthood with a random one. |
|------------------+---------------------------------------+--------------------------------------------|
| Random childhood | `!buy randomchildhood`                | Replaces a viewer's pawn's childhood with a random one. |
|------------------+---------------------------------------+--------------------------------------------|
| Replace trait    | `!buy replacetrait <old> <new>`       | Replaces the trait `old` with the trait `new`. |
|------------------+---------------------------------------+--------------------------------------------|
| Rescue me        | `!buy rescueme`                       | Spawns a prisoner rescue somewhere in the world with the viewer's pawn as the captive. This event only works for kidnapped pawns. |
|------------------+---------------------------------------+--------------------------------------------|
| Revive me        | `!buy reviveme`                       | Revives the viewer's pawn.                 |
|------------------+---------------------------------------+--------------------------------------------|
| Set traits       | `!buy settraits <trait> <trait> ...`  | Sets a viewer's pawn's traits to the list given if possible. |
|------------------+---------------------------------------+--------------------------------------------|
| Smite            | `!buy smite <viewer>`                 | Smites the specified viewer's pawn.        |
|------------------+---------------------------------------+--------------------------------------------|
| Surgery          | `!buy surgery <item> [part/quantity]` | Queues a surgery on the viewer's pawn. The part to be installed is specified through `item`, whereas the body part to install it on is specified through `part`. Additionally, `part` can be replaced with a number to queue multiple surgeries at once (`quantity`). |
|------------------+---------------------------------------+--------------------------------------------|
| Use              | `!buy use <item>`                     | Purchases and forces a viewer's pawn to use the specified item. |
|------------------+---------------------------------------+--------------------------------------------|
| Wear             | `!buy wear <item>`                    | Purchases and forces a viewer's pawn to wear the specified item. |
|------------------+---------------------------------------+--------------------------------------------|

## Colony Events

|-----------------+---------------------------------------+--------------------------------------------|
| Name            | Usage                                 | Description                                |
|:----------------+:--------------------------------------|:-------------------------------------------|
| Heal all        | `!buy healall`                        | Heals one hediff on every colonist.        |
|-----------------+---------------------------------------+--------------------------------------------|
| Sanctuary       | `!buy sanctuary`                      | Shrouds the colony in a mysterious light. While the light is over the colony, dead colonists are revived, healable hediffs are healed, hostile pawns are forced off the map or killed. |
|-----------------+---------------------------------------+--------------------------------------------|
| Revive all      | `!buy reviveall`                      | Revives all dead colonists.                |
|-----------------+---------------------------------------+--------------------------------------------|
