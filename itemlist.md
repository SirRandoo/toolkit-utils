---
layout: default
title: ToolkitUtils' Item List
nav_order: 7
permalink: /itemlist
---

## ToolkitUtils and the Item List
{: .no_toc .text-delta }

With the addition of [trait](/tweaks/traits) and [race](/tweaks/races) support,
ToolkitUtils changed several aspects of Toolkit fundamentally. As a result, the
item list used by users no longer accurately reflects how some events will
function, like adding traits. To solve this problem, ToolkitUtils has its own
[item list](https://github.com/sirrandoo/itemlist).

### Table of Contents
1. TOC
{: toc }

### Set Up

Setting up Utils' item list is simliar to Toolkit's initial set up:

1. Fork the [item list](https://github.com/sirrandoo/itemlist) on Github.
![Fork the list on Github](/assets/itemlist-fork.png)

2. Go to your fork's settings
![Forked list's settings](/assets/itemlist-settings.png)

3. (OPTIONAL) If you do ***not*** have a personal site, you can rename the repository to `YOUR_GITHUB_USERNAME.github.io`
![Rename fork](/assets/itemlist-rename.png)

4. Scroll down to the Github pages section and ensure the `master` branch is used for Github pages.
![Fork pages](/assets/itemlist-pages.png)

### Uploading Toolkit Files

#### Items

1. Navigate to Toolkit's save directory
2. Open `StoreItems.json` in any editor
3. Copy its contents
4. On your item list, navigate to the `_data` directory
![Fork's _data directory](/assets/itemlist-data_directory.png)

5. Click your items file
![Fork's item file](/assets/itemlist-store_items_file.png)

6. Click the edit button
![Edit fork's item file](/assets/itemlist-store_items_file-edit.png)

7. Paste the contents from `StoreItems.json` into the editor
8. Commit the changes
![Commit fork's item file changes](/assets/itemlist-store_items_file-commit.png)

#### Events

Updating your events file is the same steps as your items file, but instead of
updating `StoreItems.json`, you'll be updating `StoreIncidents.json`. Please
refer to the [Items](#items) section for a walkthrough of how to upload your
Toolkit events.

#### Traits & Races

Like [Items](#items), you'll follow the same steps, except you'll be updating
the `ShopExt.json` file. It's important to note that you **must** enable the
setting [Shop File as Json](/settings/general#shop-file-as-json) before you'll
see the `ShopExt.json` file. If the setting is enabled and you *don't* see the
file, please open the trait or race config, then close it. Additionally, restarting
RimWorld will generate a json file for Utils.

#### Commands

You may have noticed that there is also a `commands.json` file in the `_data`
directory. By default, this file is prepopulated with the default commands a
viewer might expect to find in a general Toolkit+Utils playthrough. Currently,
Utils does *not* generate this file, so you'll have to manually modify this
file to change the command list.

#### Mod List

You may have *also* noticed that there is a `modlist.json` file in the `_data`
directory aswell. Utils doesn't generate this file yet either, so you'll have
to manually add and remove mods. Any json object you add should have a `name`
field and a `version` field. Any other fields are ignored by the list and will
require some modifications to be shown.

### Personalizing

You'll quickingly notice that the item list has some references to `SirRandoo`
in some areas, like the header. You can change this by editing the `_config.yml`
file in the root directory.

![Edit the _config.yml file](/assets/itemlist-personalize_config.png)

Here you have the option of editing `title` on line `21`, `description` on line
`23`, `email` on line `26`, `twitter_username` on line `29`, `github_username`
on line `30`, and `baseurl` on line `27`.

#### Base Url

If you named your item list repository `YOUR_GITHUB_USERNAME.github.io` you'll
want to replace this with an empty string (`''`). If you renamed your item list
to something *other* than `itemlist`, you'll want to put in that instead. If
your repository has spaces, you'll put dashes (`-`) in place of them. Generally,
this should be how Github names your repository, which you can find in your
url bar.

![Example url bar](/assets/itemlist-personalize_config-base_url.png)

### Social Icons

At the bottom of the item list you'll find a little Twitch icon that points to
`sirrandoo`'s profile. You can modify this, as well as add several others, in
the `_data\social.yml` file. There won't be a walkthrough for this as *everything*
in this file is personalizable. Just place your account handles in the relevant
lines and commit.

![Personalize social](/assets/itemlist-personalize_social.png)
