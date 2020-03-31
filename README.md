# ToolkitUtils
A collection of tweaks and commands for TwitchToolkit.


### What's included?

#### Pawn Commands

- `!mypawnbody`<br/>
    Allows a viewer to see information about their pawn's body.  Information included is
    the temperature range the pawn can currently tolerate, and the various afflictions
    the pawn is currently suffering from.  Afflictions are further broken down into whether
    or not they're causing the pawn to bleed out.
    <br/>
    **Note**: If the setting "Temperature in `!mypawngear`" is enabled, temperature 
                information will instead be outputted to `!mypawngear` instead of here.
- `!mypawnhealth [capacity]`<br/>
    Allows a viewer to see information about their pawn's health.  By default, if no `capacity`
    is supplied, the command will output a summary.
    <br/>
    <br/>
    The health summary includes information about the pawn's overall health reduced to a 
    percentage, whether or not they're currently downed, their current mood, whether or not
    the pawn is bleeding out, a percentage representing how effective a capacity is currently
    operating, and any queued surgeries.
    <br/>
    **Note**: If a pawn is currently downed, this information will be used instead of the pawn's mood.
    <br/>
    **Note**: If a pawn's race does not support a capacity, it will not be listed in the command.
                If no capacities are supported, the command will replace the capacity list with
                a message stating so.
    <br/>
    **Note**: If the streamer does not enable "Show queued surgeries in `!mypawnhealth`", no surgery
                information will be included in the command.
    <br/>
    <br/>
    If a viewer includes a `capacity`, the command will instead output the capacity's effective
    operational percentage, and a list of things currently effecting said percentage.
- `!mypawnneeds`<br/>
    Allows a viewer to see their pawn's current needs.
- `!mypawnwork`<br/>
    Allows a viewer to see their pawn's current work priority settings.
    <br/>
    **Note**: If the streamer does not enable "Sort work priorities in `!mypawnwork`", the 
                command will output the priorities in the order they were registered in
                RimWorld.  If the setting were to be enabled, they would be sorted in the
                order as seen in the `Work` tab.
    <br/>
    **Note**: If the streamer does not enable "Filter work priorities in `!mypawnwork`", the
                command will output *all* work priorities, even if they're disabled or incapable.
- `!mypawngear`<br/>
    Allows a viewer to see their pawn's current gear.  Information included is the pawn's
    temperature tolerance range, their armor protection values, and the weapon the pawn is
    currently holding.
    <br/>
    **Note**: If the setting "Temperature in `!mypawngear`" is disabled, temperature information
                will instead be outputted to `!mypawnbody`.
    <br/>
    **Note**: If the setting "Show armor values in `!mypawngear`" is disabled, armor protection
                values will not be shown.
    <br/>
    **Note**: If the setting "Show pawn's weapon in `!mypawngear`" is disabled, the command will
                not include the weapon the pawn is currently holding.
- `!fixmypawn`<br/>
    Allows a viewer to relink their pawn without intervention from the streamer.
    <br/>
    **Note**: This command does not support custom pawn names.  It will only look for a colonist 
                named after the viewer.
    <br/>
    **Note**: This command does not safely relink a pawn.  It will look for the ***first*** colonist 
                named after the viewer.
- `!leave`<br/>
    Allows a viewer to voluntarily leave the colony without the streamer having to banish them.
    <br/>
    <br/>
    By default the preferred method of leave is a "Run Wild" mental break.  Should the streamer choose
    to enable the alternative, they will be reduced to a pile of ash.
    <br/>
    **Note**: This command does **not** refund any coins spent on a bad pawn.
    <br/>
    **Note**: If the leave method is not `Mental Break`, the pawn's current inventory will turn to ash
                as well.

#### Colony Commands

- `!fixallpawns`<br/>
    Please refer to `!fixmypawn` for a general overview.
    <br/>
    **Note**: This command will relink ***every*** pawn, including already linked pawns.
- `!research [project]`<br/>
    Allows a viewer to query the colony's research projects.
    <br/>
    <br/>
    If no `project` is specified, this command will instead display the progress of the current
    research project.
    <br/>
    <br/>
    If a `project` is specified, this command will display the current progress towards that 
    project.  If the project has uncompleted prerequisites, the command will include them along
    with the progress of said prerequisite.
- `!factions`<br/>
    Allows a viewer to see the colony's current faction relations.
- `!unstick`<br/>
    A carbon copy of Toolkit's `!unstick` command.

#### Purchasable Events

- `!buy reviveall` / `!reviveall`<br/>
    Allows a viewer to purchase a colony-wide revive.
- `!buy reviveme` / `!reviveme`<br/>
    Allows a viewer to purchase a personal revive.
- `!buy healme` / `!healme`<br/>
    Allows a viewer to purchase a personal healer mech serum.
- `!buy healall` / `!healall`<br/>
    Allows a viewer to purchase a colony-wide healer mech serum.
- `!buy passionshuffle {preferred}` / `!passionshuffle`<br/>
    Allows a viewer to shuffle their current passions.
    <br/>
    **Note**: If a preferred skill is specified, there will *always* be a minor passion in this 
                skill, but all others will be shuffled.
- `!buy surgery {part}` / `!surgery`<br/>
    Allows a viewer to purchase an installable part and immediately queue it for surgery.
    <br/>
    **Note**: This command does ***not*** include medicine.  This is a convenience event for
                ensuring parts get installed without the streamer having to keep track of who
                purchased what.
    **Note**: This command does not have checks to ensure downgrades don't replace upgrades, like
                bionic replacing an archotech. 

#### Toolkit Tweaks

- `!buy trait`<br/>
    This tweak modifies how `!buy trait` looks up traits in order to remove
    the `<color>` tag requirement in mods that add a colored traits.
    <br/>
    <br/>
    In addition, an optional tweak allows streamers to ignore sexuality traits 
    in regards towards the trait limit.  This allows viewers to freely include
    a sexuality trait without impacting their ideal trait lineup.
- `!buy removetrait`<br/>
    This tweak modifies how `!buy removetrait` looks up traits in order to
    remove the `<color>` tag requirement in mods that add a colored trait.
- `!buy pawn`<br/>
    This tweak modifies how Toolkit processes pawn purchases to allow for race
    support.  A race can be specified with the `race={race}` argument.
    <br/>
    **Note**: If this argument isn't included, the tweak will default to using
                `Human` as the pawn's race.
- `!lookup {category} {query}`<br/>
    This tweak modifies how Toolkit looks up products.
    <br/>
    <br/>
    With the addition of the race tweak above, this command has been modified
    to show the current list of races available to viewers for use in the
    `race={race}` argument.
    <br/>
    <br/>
    Additionally, this command now allows you to use a query that features spaces
    via quoting (`"my little query"`).
    <br/>
    **Note**: If you elect to use a quoted query, all products will be looked up
                according to that query.  This means that the tweak will not 
                modify your query.
- `!price {category} {query} [quantity]`<br/>
    This command is similar to `!lookup`, but shows the price of a product in the shop.
- `!installedmods`<br/>
    This tweak modifies how Toolkit outputs the mod list.  When ToolkitUtils 
    first loads, it'll cache the current mod list to allow for quicker responses.
    <br/>
    <br/>
    Optionally, this tweak allows a streamer to include mod versions in the
    command's output.  This functionality can be enabled via the
    "Show mod versions in `!installedmods`" setting.
- Command Parser<br/>
    This *optional* tweak modifies how Toolkit processes commands.  This forces
    viewers to use the commands as defined by mods via XML defs.  This means that
    `!bal` will ***always*** be `!bal`, not `!balloon`.
    <br/>
    **Note**: This tweak is **required** if you want the shortcut commands this
                mod provides for its shop events to function.
    <br/>
    **Note**: This tweak also allows you to specify a custom command prefix for 
                your channel via the mod's settings dialog.
    <br/>
    **Warning**: Using this tweak *can* negatively impact Toolkit in various ways.
                    Should an issue occur while this mod is enabled, it's worth 
                    disabling the parser and seeing if the issue persists.  If the
                    issue doesn't, you should [submit a bug report](https://github.com/sirrandoo/toolkit-utils/issues/new)
- Founders Support<br/>
    This tweak allows Toolkit to process the `Founders` badge added by Twitch.
- Configurable trait prices<br/>
    This tweak allows streamers to configure the prices of each individual trait in
    their current game.  This includes the price of adding a trait, and the price of
    removing a trait.
- Configurable race prices<br/>
    This tweak allows streamers to configure the prices of each individual race a 
    pawn can be.
