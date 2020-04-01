---
layout: default
title: Command Handler
parent: Toolkit Tweaks
has_children: true
---

ToolkitUtils comes bundled with an optional command handler. It forces viewers to use
proper commands, like `!bal` instead of `!balloon`. Internally, this tweak only gets
utilized by ToolkitUtils to match commands. Toolkit doesn't natively support this
type of expansion, so Utils transforms any commands the handler parses into something
Toolkit can understand, and serves as a fix for shortcut commands being unsupported
natively.

Users have the option to disable the command handler should any unexpected bugs arise
via ToolkitUtils' settings menu. You can find the setting under the `General`
category.
