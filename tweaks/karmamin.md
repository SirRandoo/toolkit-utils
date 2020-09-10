---
layout: default
title: Karma Minimum
parent: Toolkit Tweaks
---

Twitch Toolkit has a concept of "karma", which is the mod's way of punishing bad viewers
when they purchase bad events, or rewarding them for purchasing good events. Karma does this
by affecting how _much_ coins a viewer will gain per interval. This means that the higher your
karma is, the higher your expected income is; the same can be said about the opposite. However,
should a streamer allow viewers to dip into negative karma, the minimum cap is hardcoded to -10.
While this works out fine for your average user, power users may want to lower this value even
_more_ to punish exceptionally bad viewers. Utils patches this by allowing the value to go as
far down as modern system can allow (-2 billion).
