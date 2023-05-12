// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using JetBrains.Annotations;
using ToolkitUtils.Api;
using Verse;

namespace ToolkitUtils
{
    /// <summary>
    ///     A class for housing the various settings within the mod.
    /// </summary>
    [StaticConstructorOnStartup]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class TkSettings : ModSettings
    {
        /// <summary>
        ///     When enabled, viewers that purchase items or events that have a
        ///     "neutral" karma type rating will not receive any karma for said
        ///     purchase.
        /// </summary>
        public static bool TrueNeutral;

        /// <summary>
        ///     When enabled, viewers are required to use the extended item
        ///     syntax in order to purchase items.
        /// </summary>
        /// <remarks>
        ///     The extended item syntax is roughly as follows:
        ///     ITEM[MATERIAL,QUALITY]
        ///     <br/>
        ///     <br/>
        ///     In the above example, the placeholder "ITEM" is the item a viewer
        ///     wants to purchase, the placeholder "MATERIAL" is the material of
        ///     the item, if it can have a material, and the placeholder
        ///     "QUALITY" is the quality of the item, if it can have a quality.
        ///     <br/>
        ///     <br/>
        ///     Note: the placeholders "MATERIAL" and "QUALITY" are
        ///     interchangeable, and optional depending on the item being
        ///     purchased.
        /// </remarks>
        public static bool ForceFullItem;

        /// <summary>
        ///     Whether the mod should use its command handler instead of Twitch
        ///     Toolkit's.
        /// </summary>
        /// <remarks>
        ///     Utils' command handler fixes up some of the oddities within
        ///     Twitch Toolkit's, more notably its case-sensitiveness, as well as
        ///     its lack of support for "shortcut" commands. Shortcut commands
        ///     are commands that reference the
        ///     <see cref="TwitchToolkit.Commands.ViewerCommands.Buy"/> as its
        ///     command handler, but call an event with the same name. An example
        ///     of this within Twitch Toolkit would be the "levelskill" command.
        ///     <br/>
        ///     <br/>
        ///     In addition to the above, it allows users to change the prefix of
        ///     their commands, in case there's a conflict between an existing
        ///     bot within their channel, as well as opens the concept of
        ///     "shortcut" commands to every item and event available within the
        ///     mod through the <see cref="BuyPrefix"/> setting.
        /// </remarks>
        public static bool Commands = true;

        /// <summary>
        ///     The prefix of all the commands within Twitch Toolkit. Messages
        ///     that do not start with this value will be ignored by the command
        ///     handler.
        /// </summary>
        /// <remarks>
        ///     The associated code for this setting is only active when the
        ///     <see cref="Commands"/> setting is enabled.
        /// </remarks>
        public static string Prefix = "!";

        /// <summary>
        ///     The prefix for the global "shortcut" code within the mod.
        ///     Messages that do not start with this value will be ignored by the
        ///     command handler.
        /// </summary>
        /// <remarks>
        ///     The associated code for this setting is only active when the
        ///     <see cref="Commands"/> setting is enabled.
        /// </remarks>
        public static string BuyPrefix = "$";

        /// <summary>
        ///     Whether the mod's command handler will check for a command match
        ///     by comparing a viewer's input to the beginning of the command's
        ///     name, instead of a direct match.
        /// </summary>
        /// <remarks>
        ///     The associated code for this setting is only active when the
        ///     <see cref="Commands"/> setting is enabled.
        /// </remarks>
        public static bool ToolkitStyleCommands = true;

        /// <summary>
        ///     Whether the mod will remove cosmetic whitespace from its data
        ///     files when saving.
        /// </summary>
        public static bool MinifyData;

        /// <summary>
        ///     Whether the mod will add a little star next to mods from
        ///     SirRandoo.
        /// </summary>
        public static bool DecorateMods;

        /// <summary>
        ///     Whether the mod's commands and events will use unicode symbols
        ///     and emojis instead of plain text responses.
        /// </summary>
        /// <remarks>
        ///     If disabled, certain information may be omitted from responses to
        ///     limit the amount of text being sent to chat, as well as no
        ///     suitable text-based alternative being available.
        /// </remarks>
        public static bool Emojis = true;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnWork"/> command
        ///     will filter work priorities disabled for the given pawn.
        /// </summary>
        public static bool FilterWorkPriorities;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnGear"/> command
        ///     will include a given pawn's current apparel.
        /// </summary>
        public static bool ShowApparel;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnGear"/> command
        ///     will include a given pawn's armor rating.
        /// </summary>
        public static bool ShowArmor = true;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnHealth"/>
        ///     command will include any queued surgeries for a given pawn.
        /// </summary>
        public static bool ShowSurgeries = true;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnGear"/> command
        ///     will include a given pawn's currently equipped weapon, as well as
        ///     sidearms if the relevant mod is installed.
        /// </summary>
        public static bool ShowWeapon = true;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnWork"/> command
        ///     will sort the priorities to the order displayed in the in-game
        ///     work tab.
        /// </summary>
        public static bool SortWorkPriorities;

        /// <summary>
        ///     Whether viewers are allowed to purchase pawns other than human.
        /// </summary>
        public static bool PurchasePawnKinds = true;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnGear"/> command
        ///     will include a given pawn's temperature range instead of being
        ///     included in the <see cref="ToolkitUtils.Commands.PawnBody"/>
        ///     command.
        /// </summary>
        public static bool TempInGear;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.PawnLeave"/> command
        ///     will drop the viewer's pawn inventory, gear, and weapons prior to
        ///     leaving.
        /// </summary>
        /// <remarks>
        ///     If the current leave method is
        ///     <see cref="Api.LeaveMethod.Thanos"/>, this setting's
        ///     associated code will never run; the items will disappear along
        ///     with the pawn.
        /// </remarks>
        public static bool DropInventory;

        /// <summary>
        ///     The current coin tier for the broadcaster.
        /// </summary>
        public static string BroadcasterCoinType = nameof(UserTypes.Broadcaster);

        /// <summary>
        ///     The current leave method for
        ///     <see cref="ToolkitUtils.Commands.PawnLeave"/>.
        /// </summary>
        public static string LeaveMethod = nameof(Api.LeaveMethod.Voluntarily);

        /// <summary>
        ///     The current dump method for the mod.
        /// </summary>
        public static string DumpStyle = nameof(ToolkitUtils.DumpStyle.SingleFile);

        /// <summary>
        ///     The maximum number of entries that will be displayed in
        ///     lookup-style commands, mainly
        ///     <see cref="ToolkitUtils.Commands.Lookup"/>.
        /// </summary>
        public static int LookupLimit = 10;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.InstalledMods"/>
        ///     command will include the respective mod's version, if one could
        ///     be found.
        /// </summary>
        public static bool VersionedModList;

        /// <summary>
        ///     Whether the <see cref="ToolkitUtils.Commands.Balance"/> command
        ///     will include the amount of coins a viewer is earning every reward
        ///     cycle.
        /// </summary>
        public static bool ShowCoinRate = true;

        /// <summary>
        ///     Whether Twitch Toolkit will change a viewer's pawn's hair color
        ///     to match their chat color.
        /// </summary>
        /// <remarks>
        ///     If a viewer has not set their chat color, their hair will not be
        ///     changed.
        /// </remarks>
        public static bool HairColor = true;

        /// <summary>
        ///     The minimum opinion a pawn must have with a given viewer's pawn
        ///     in order to be displayed within the
        ///     <see cref="ToolkitUtils.Commands.PawnRelations"/> command.
        /// </summary>
        /// <remarks>
        ///     This value is compared to the absolute value of a pawn's opinion
        ///     of a given viewer's pawn. If the opinion rating is <c>-40</c>,
        ///     and this value is <c>20</c>, it would be compared as if it were
        ///     <c>40 > 20</c>.
        /// </remarks>
        public static int OpinionMinimum;

        /// <summary>
        ///     Whether the mod will complete purchases as they come in,
        ///     regardless of whether the game is paused.
        /// </summary>
        /// <remarks>
        ///     The different internally is whether purchases are completed on an
        ///     "update" or on a "tick", with the latter requiring the game be
        ///     unpaused.
        /// </remarks>
        public static bool AsapPurchases;

        /// <summary>
        ///     The amount of items the item store will validate per tick.
        /// </summary>
        /// <remarks>
        ///     The associated code for this setting runs once when the item
        ///     store is first open. It's job is to act as a final validator to
        ///     ensure the items available to viewers are up-to-date.
        /// </remarks>
        public static int StoreBuildRate = 60;

        /// <summary>
        ///     Whether purchases can be made.
        /// </summary>
        public static bool StoreState = true;

        /// <summary>
        ///     Whether the mod will save its data on a separate thread.
        /// </summary>
        public static bool Offload;

        /// <summary>
        ///     Whether a viewer's balance will be displayed in the confirmation
        ///     message when purchasing items.
        /// </summary>
        public static bool BuyItemBalance;

        /// <summary>
        ///     Whether viewers are allowed to change their classes through the
        ///     trait events.
        /// </summary>
        public static bool ClassChanges;

        /// <summary>
        ///     Whether the trait events will reset the viewer's pawn's class
        ///     data when changing classes.
        /// </summary>
        public static bool ResetClass;

        /// <summary>
        ///     Whether the mod will defer exception raised through the command
        ///     and event systems to the mod "VisualExceptions".
        /// </summary>
        public static bool VisualExceptions;

        /// <summary>
        ///     Whether the "!mypawnrelations" command will only show
        ///     relationships with a concrete def associated with them.
        /// </summary>
        /// <remarks>
        ///     Internally there are several "inferred" relationships, like
        ///     acquaintances, that have no def associated with them. Enabling
        ///     this setting only allows relationships with a def backing.
        /// </remarks>
        public static bool MinimalRelations = true;

        /// <summary>
        ///     Whether a small puff of smoke will be displayed when something
        ///     spawns through the gateway.
        /// </summary>
        public static bool GatewayPuff = true;

        /// <summary>
        ///     Whether the mod's easter eggs can occur. Disabling this removes
        ///     the random rats that spawn from the gateway, as well as disable
        ///     user-specific easter eggs.
        /// </summary>
        public static bool EasterEggs = true;

        /// <summary>
        ///     Whether the mod's command router is enabled. The mod's command
        ///     router is responsible for executing commands on threads separate
        ///     from TwitchLib's.
        /// </summary>
        public static bool CommandRouter = true;

        /// <summary>
        ///     Whether commands and events that accept custom colors can accept
        ///     a transparency value.
        /// </summary>
        public static bool TransparentColors;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Commands, "commands", true);
            Scribe_Values.Look(ref Prefix, "prefix", "!");
            Scribe_Values.Look(ref BuyPrefix, "buyPrefix", "$");
            Scribe_Values.Look(ref ToolkitStyleCommands, "toolkitStyleCommands", true);
            Scribe_Values.Look(ref DecorateMods, "decorateUtils");
            Scribe_Values.Look(ref ForceFullItem, "forceFullItemSyntax");
            Scribe_Values.Look(ref Emojis, "emojis", true);
            Scribe_Values.Look(ref FilterWorkPriorities, "filterWork");
            Scribe_Values.Look(ref ShowApparel, "apparel");
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowWeapon, "weapon", true);
            Scribe_Values.Look(ref SortWorkPriorities, "sortWork");
            Scribe_Values.Look(ref PurchasePawnKinds, "race", true);
            Scribe_Values.Look(ref TempInGear, "tempInGear");
            Scribe_Values.Look(ref DropInventory, "dropInventory");
            Scribe_Values.Look(ref LeaveMethod, "leaveMethod", nameof(Api.LeaveMethod.Voluntarily));
            Scribe_Values.Look(ref BroadcasterCoinType, "broadcasterCoinType", nameof(UserTypes.Broadcaster));
            Scribe_Values.Look(ref LookupLimit, "lookupLimit", 10);
            Scribe_Values.Look(ref AsapPurchases, "asapPurchases");
            Scribe_Values.Look(ref VersionedModList, "versionedModList");
            Scribe_Values.Look(ref ShowCoinRate, "balanceCoinRate", true);
            Scribe_Values.Look(ref TrueNeutral, "trueNeutral");
            Scribe_Values.Look(ref HairColor, "hairColor", true);
            Scribe_Values.Look(ref OpinionMinimum, "minimumOpinion", 20);
            Scribe_Values.Look(ref StoreBuildRate, "storeBuildRate", 60);
            Scribe_Values.Look(ref Offload, "offload");
            Scribe_Values.Look(ref BuyItemBalance, "buyItemBalance");
            Scribe_Values.Look(ref ClassChanges, "classChanges");
            Scribe_Values.Look(ref ResetClass, "resetClass");
            Scribe_Values.Look(ref MinimalRelations, "minimalRelations", true);
            Scribe_Values.Look(ref GatewayPuff, "gatewayPuff", true);
            Scribe_Values.Look(ref EasterEggs, "easterEggs", true);
            Scribe_Values.Look(ref TransparentColors, "allowTransparentColors");
        }
    }
}
