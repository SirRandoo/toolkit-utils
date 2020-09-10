---
layout: default
title: ToolkitUtils' Item List
nav_order: 7
permalink: /itemlist
---

## ToolkitUtils and the Item List
{: .no_toc .text-delta }

With the addition of [trait]({{- "/tweaks/traits" | relative_url -}})
and [race]({{- "/tweaks/races" | relative_url -}}) support, ToolkitUtils
changed several aspects of Toolkit fundamentally. As a result, the item
list used by users no longer accurately reflects how some events will function,
like adding traits. To solve this problem, ToolkitUtils has its own
[item list](https://github.com/sirrandoo/itemlist).

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

### Set Up

Setting up Utils' item list is simliar to Toolkit's initial set up:

1. Fork the [item list](https://github.com/sirrandoo/itemlist) on Github.
![Fork the list on Github]({{- "/assets/itemlist/fork.png" | relative_url -}})

2. Go to your fork's settings
![Forked list's settings]({{- "/assets/itemlist/settings.png" | relative_url -}})

3. (OPTIONAL) If you do ***not*** have a personal site, you can rename the repository to `YOUR_GITHUB_USERNAME.github.io`
![Rename fork]({{- "/assets/itemlist/rename.png" | relative_url -}})

4. Scroll down to the Github pages section and ensure the `master` branch is used for Github pages.
![Fork pages]({{- "/assets/itemlist/pages.png" | relative_url -}})

### Uploading Toolkit Files

#### Items

1. Navigate to Toolkit's save directory
2. Open `StoreItems.json` in any editor
3. Copy its contents
4. On your item list, navigate to the `_data` directory
![Fork's _data directory]({{- "/assets/itemlist/data_directory.png" | relative_url -}})

5. Click your items file
![Fork's item file]({{- "/assets/itemlist/store_items_file.png" | relative_url -}})

6. Click the edit button
![Edit fork's item file]({{- "/assets/itemlist/store_items_file-edit.png" | relative_url -}})

7. Paste the contents from `StoreItems.json` into the editor
8. Commit the changes
![Commit fork's item file changes]({{- "/assets/itemlist/store_items_file-commit.png" | relative_url -}})

#### Events

Updating your events file is the same steps as your items file, but instead of
updating `StoreItems.json`, you'll be updating `StoreIncidents.json`. Please
refer to the [Items](#items) section for a walkthrough of how to upload your
Toolkit events.

#### Traits & Races

Like [Items](#items), you'll follow the same steps, except you'll be updating
the `ShopExt.json` file.

#### Commands

As with the other two, you'll follow the same steps on the `commands.json` file
in your Toolkit directory.

#### Mod List

Same as before, but now it's on the `modlist.json` file. This file includes mod
names, versions, authors, and the steam id of the mod. Utils' item list will
automatically generate links to the mod's workshop page if a steam id was found,
or a link with the mod's name as a search query.

### Personalizing

You'll quickingly notice that the item list has some references to `SirRandoo`
in some areas, like the header. You can change this by editing the `_config.yml`
file in the root directory.

![Edit the _config.yml file]({{- "/assets/itemlist/personalize_config.png" | relative_url -}})

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

![Example url bar]({{- "/assets/itemlist/personalize_config-base_url.png" | relative_url -}})

Not adjusting the base url to fit the criteria above **will** negatively impact
how the site looks, or outright stop certain pages from being visible.
{: .warn}

### Social Icons

At the bottom of the item list you'll find a little Twitch icon that points to
`sirrandoo`'s profile. You can modify this, as well as add several others, in
the `_data\social.yml` file. There won't be a walkthrough for this as *everything*
in this file is personalizable. Just place your account handles in the relevant
lines and commit.

![Personalize social]({{- "/assets/itemlist/personalize_social.png" | relative_url -}})
