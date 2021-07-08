---
title: Toolkit Editor
nav_order: 6
layout: default
---

As part of the v6 update to Utils, the mod introduced "Toolkit Editor" -- easy way of modifying
things in bulk, saving presets, and loading presets. Below is a brief overview of the editor's
menu.

![Toolkit Editor menu]({{- "/assets/editor/editor.png" | relative_url -}})

# Navigating the menu

## Store tabs

The various stores you can modify are shown in the top-left as tabs. Clicking on the store you
want to modify will appropriately update the display to the selected store. The state other
stores were in prior to switching is preserved so you can always come back to it, unless you
close the menu.

## Presets

Presets, or partials as the mod likes to call them, are a way of saving and loading sections of
your store. The sections that are saved are affected by the selectors you have active.

### Saving a Preset

Saving a preset can be done by clicking the
![Save preset icon]({{- "/assets/editor/save-preset.png" | relative_url -}}) icon. The menu below
should appear:

![Save preset dialog]({{- "/assets/editor/save-preset-dialog.png" | relative_url -}})

In the dialog above, you can see it asks for 2 types of information: a name for the preset, and
a description of the preset. Clicking `Cancel` will abort the operation, and clicking `Confirm`
will save the store section to disk.

### Loading a Preset

Loading a preset can be done by clicking the
![Load preset icon]({{- "/assets/editor/load-preset.png" | relative_url -}}) icon. The menu below
should appear:

![Load preset dialog]({{- "/assets/editor/load-preset-dialog.png" | relative_url -}})

In the dialog above, you can see all the presets you have saved. To the right there are two icons:
![Load preset icon]({{- "/assets/editor/load-preset-2.png" | relative_url -}}) and 
![Delete preset icon]({{- "/assets/editor/delete-preset.png" | relative_url -}}). To the left is
the name the preset is saved under, and its description can be read by hovering over it.

The ![Load preset icon]({{- "/assets/editor/load-preset-2.png" | relative_url -}}) icon tells the
mod to load the section saved in the preset.

The ![Delete preset icon]({{- "/assets/editor/delete-preset.png" | relative_url -}}) icon tells
the mod to delete the preset.

## Selectors

Selectors are the sole means of specifying _what_ you want to modify. Selectors can be added by
clicking `Add selector` in the left column of the editor. Once clicked, a list of available
selectors will appear. Clicking any will display it under the `Add selector` button. Changing,
adding, or removing any selector will automatically update the display in the bottom half of the
editor. There won't be a comprehensive list of selectors the mod has available since the list
can easily get out of hand, however they try to be self-descriptive.

## Mutators

Mutators are the sole means of modifying what you have selected. Mutators can be added by clicking
`Add mutator` in the right column of the editor. Once clicked, a list of available mutators will
appear. Clicking any will display it under the `Add mutator` button. Unlike selectors, mutators do
no automatically change what you have selected. Changes are only made once you've clicked `Apply`
in the far right of the menu. There won't be a comprehensive list of mutators the mod has available
since the list can easily get out of hand, however they try to be self-descriptive.
