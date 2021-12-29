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

using System;
using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class UtilsSettingsWindow : FakeSettingsWindow
    {
        private readonly TabWorker tabWorker;
        private string asapPurchasesDescription;
        private string asapPurchasesLabel;
        private string balanceGroupHeader;
        private string basketGroupHeader;
        private string broadcasterTypeDescription;
        private string broadcasterTypeLabel;
        private FloatMenu broadcasterUserTypeMenu;

        private string buildRateBuffer = TkSettings.StoreBuildRate.ToString();
        private bool buildRateBufferValid = true;
        private string buyItemBalanceDescription;
        private string buyItemBalanceLabel;
        private string buyItemGroupHeader;
        private string coinRateDescription;
        private string coinRateLabel;
        private string commandHandlerGroupHeader;
        private string commandParserDescription;
        private string commandParserLabel;
        private string commandPrefixDescription;
        private string commandPrefixLabel;
        private string commandRouterDescription;
        private string commandRouterLabel;
        private Vector2 commandTweakPos = Vector2.zero;

        private Vector2 dataScrollPos = Vector2.zero;

        private float dataTabHeight;
        private string decorateUtilsDescription;
        private string decorateUtilsLabel;
        private string dumpStyleDescription;
        private string dumpStyleLabel;
        private FloatMenu dumpStyleMenu;
        private string easterEggsDescription;
        private string easterEggsLabel;
        private string emojisDescription;
        private string emojisGroupHeader;
        private string emojisLabel;

        private string filesGroupHeader;
        private string gatewayGroupHeader;
        private string gatewayPuffDescription;
        private string gatewayPuffLabel;
        private string hairColorDescription;
        private string hairColorLabel;
        private string installedModsGroupHeader;
        private string itemSyntaxDescription;
        private string itemSyntaxLabel;
        private string lazyProcessGroupHeader;
        private string lookupGroupHeader;
        private string lookupLimitDescription;
        private string lookupLimitLabel;
        private string minifyDataDescription;
        private string minifyDataLabel;
        private string offloadShopDescription;
        private string offloadShopLabel;
        private string purchasePrefixDescription;
        private string purchasePrefixLabel;
        private string storeRateDescription;
        private string storeRateLabel;
        private string toolkitStyleDescription;
        private string toolkitStyleLabel;
        private string trueNeutralDescription;
        private string trueNeutralLabel;
        private string versionedModListDescription;
        private string versionedModListLabel;
        private string viewerGroupHeader;

        public UtilsSettingsWindow() : base(TkUtils.Instance)
        {
            tabWorker = new TabWorker();
        }

        public override void PreOpen()
        {
            tabWorker.AddTab(new TabItem { ContentDrawer = DrawGeneralSettings, Label = "TKUtils.General.Label".TranslateSimple(), Tooltip = "TKUtils.General.Tooltip".TranslateSimple() });
            tabWorker.AddTab(new TabItem { ContentDrawer = DrawDataSettings, Label = "TKUtils.Data.Label".TranslateSimple(), Tooltip = "TKUtils.Data.Tooltip".TranslateSimple() });

            tabWorker.AddTab(
                new TabItem { ContentDrawer = DrawCommandTweakSettings, Label = "TKUtils.CommandTweaks.Label".TranslateSimple(), Tooltip = "TKUtils.CommandTweaks.Tooltip".TranslateSimple() }
            );

            tabWorker.AddTab(new TabItem { ContentDrawer = DrawModCompatSettings, Label = "TKUtils.ModCompat.Label".TranslateSimple(), Tooltip = "TKUtils.ModCompat.Tooltip".TranslateSimple() });

            base.PreOpen();
        }

        public override void PostOpen()
        {
            base.PostOpen();

            dumpStyleMenu = new FloatMenu(
                new List<FloatMenuOption>
                {
                    new FloatMenuOption("TKUtils.DumpStyle.SingleFile".TranslateSimple(), () => TkSettings.DumpStyle = nameof(DumpStyle.SingleFile)),
                    new FloatMenuOption("TKUtils.DumpStyle.MultiFile".TranslateSimple(), () => TkSettings.DumpStyle = nameof(DumpStyle.MultiFile))
                }
            );

            broadcasterUserTypeMenu = new FloatMenu(
                new List<FloatMenuOption>
                {
                    new FloatMenuOption("TKUtils.BroadcasterUserType.Broadcaster".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Broadcaster)),
                    new FloatMenuOption("TKUtils.BroadcasterUserType.Subscriber".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Subscriber)),
                    new FloatMenuOption("TKUtils.BroadcasterUserType.Vip".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Vip)),
                    new FloatMenuOption("TKUtils.BroadcasterUserType.Moderator".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Moderator)),
                    new FloatMenuOption("None".TranslateSimple().CapitalizeFirst(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.None))
                }
            );
        }

        protected override void GetTranslations()
        {
            filesGroupHeader = "TKUtils.Data.Files".TranslateSimple();
            dumpStyleLabel = "TKUtils.DumpStyle.Label".TranslateSimple();
            dumpStyleDescription = "TKUtils.DumpStyle.Description".TranslateSimple();
            minifyDataLabel = "TKUtils.MinifyData.Label".TranslateSimple();
            minifyDataDescription = "TKUtils.MinifyData.Description".TranslateSimple();
            offloadShopLabel = "TKUtils.OffloadShop.Label".TranslateSimple();
            offloadShopDescription = "TKUtils.OffloadShop.Description".TranslateSimple();
            asapPurchasesLabel = "TKUtils.DoPurchasesAsap.Label".TranslateSimple();
            asapPurchasesDescription = "TKUtils.DoPurchasesAsap.Description".TranslateSimple();
            trueNeutralLabel = "TKUtils.TrueNeutral.Label".TranslateSimple();
            trueNeutralDescription = "TKUtils.TrueNeutral.Description".TranslateSimple();
            broadcasterTypeLabel = "TKUtils.BroadcasterUserType.Label".TranslateSimple();
            broadcasterTypeDescription = "TKUtils.BroadcasterUserType.Description".TranslateSimple();
            lazyProcessGroupHeader = "TKUtils.Data.LazyProcess".TranslateSimple();
            storeRateLabel = "TKUtils.StoreRate.Label".TranslateSimple();
            storeRateDescription = "TKUtils.StoreRate.Description".TranslateSimple();

            emojisGroupHeader = "TKUtils.General.Emojis".TranslateSimple();
            emojisLabel = "TKUtils.Emojis.Label".TranslateSimple();
            emojisDescription = "TKUtils.Emojis.Description".TranslateSimple();
            viewerGroupHeader = "TKUtils.General.Viewers".TranslateSimple();
            hairColorLabel = "TKUtils.HairColor.Label".TranslateSimple();
            hairColorDescription = "TKUtils.HairColor.Description".TranslateSimple();
            gatewayGroupHeader = "TKUtils.General.Gateway".TranslateSimple();
            gatewayPuffLabel = "TKUtils.GatewayPuff.Label".TranslateSimple();
            gatewayPuffDescription = "TKUtils.GatewayPuff.Description".TranslateSimple();
            basketGroupHeader = "TKUtils.General.Basket".TranslateSimple();
            easterEggsLabel = "TKUtils.EasterEggs.Label".TranslateSimple();
            easterEggsDescription = "TKUtils.EasterEggs.Description".TranslateSimple();

            balanceGroupHeader = "TKUtils.CommandTweaks.Balance".TranslateSimple();
            coinRateLabel = "TKUtils.CoinRate.Label".TranslateSimple();
            coinRateDescription = "TKUtils.CoinRate.Description".TranslateSimple();
            commandHandlerGroupHeader = "TKUtils.CommandTweaks.Handler".TranslateSimple();
            commandPrefixLabel = "TKUtils.CommandPrefix.Label".TranslateSimple();
            commandPrefixDescription = "TKUtils.CommandPrefix.Description".TranslateSimple();
            purchasePrefixLabel = "TKUtils.PurchasePrefix.Label".TranslateSimple();
            purchasePrefixDescription = "TKUtils.PurchasePrefix.Description".TranslateSimple();
            commandParserLabel = "TKUtils.CommandParser.Label".TranslateSimple();
            commandParserDescription = "TKUtils.CommandParser.Description".TranslateSimple();
            toolkitStyleLabel = "TKUtils.ToolkitStyleCommands.Label".TranslateSimple();
            toolkitStyleDescription = "TKUtils.ToolkitStyleCommands.Description".TranslateSimple();
            commandRouterLabel = "TKUtils.CommandRouter.Label".TranslateSimple();
            commandRouterDescription = "TKUtils.CommandRouter.Description".TranslateSimple();
            installedModsGroupHeader = "TKUtils.CommandTweaks.InstalledMods".TranslateSimple();
            decorateUtilsLabel = "TKUtils.DecorateUtils.Label".TranslateSimple();
            decorateUtilsDescription = "TKUtils.DecorateUtils.Description".TranslateSimple();
            versionedModListLabel = "TKUtils.VersionedModList.Label".TranslateSimple();
            versionedModListDescription = "TKUtils.VersionedModList.Description".TranslateSimple();
            buyItemGroupHeader = "TKUtils.CommandTweaks.BuyItem".TranslateSimple();
            buyItemBalanceLabel = "TKUtils.BuyItemBalance.Label".TranslateSimple();
            buyItemBalanceDescription = "TKUtils.BuyItemBalance.Description".TranslateSimple();
            itemSyntaxLabel = "TKUtils.BuyItemFullSyntax.Label".TranslateSimple();
            itemSyntaxDescription = "TKUtils.BuyItemFullSyntax.Description".TranslateSimple();
            lookupGroupHeader = "TKUtils.CommandTweaks.Lookup".TranslateSimple();
            lookupLimitLabel = "TKUtils.LookupLimit.Label".TranslateSimple();
            lookupLimitDescription = "TKUtils.LookupLimit.Description".TranslateSimple();
        }

        protected override void DrawSettings(Rect region) { }

        private void DrawGeneralSettings(Rect region)
        {
            var listing = new Listing_Standard();
            listing.Begin(region);

            listing.DrawGroupHeader(emojisGroupHeader, false);
            listing.CheckboxLabeled(emojisLabel, ref TkSettings.Emojis);
            listing.DrawDescription(emojisDescription);


            listing.DrawGroupHeader(viewerGroupHeader);
            listing.CheckboxLabeled(hairColorLabel, ref TkSettings.HairColor);
            listing.DrawDescription(hairColorDescription);

            listing.DrawGroupHeader(gatewayGroupHeader);
            listing.CheckboxLabeled(gatewayPuffLabel, ref TkSettings.GatewayPuff);
            listing.DrawDescription(gatewayPuffDescription);

            listing.DrawGroupHeader(basketGroupHeader);
            listing.CheckboxLabeled(easterEggsLabel, ref TkSettings.EasterEggs);
            listing.DrawDescription(easterEggsDescription);

            listing.End();
        }

        private void DrawDataSettings(Rect region)
        {
            var view = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * dataTabHeight);
            var listing = new Listing_Standard();
            GUI.BeginGroup(region);
            Widgets.BeginScrollView(region.AtZero(), ref dataScrollPos, view);
            listing.Begin(view);

            listing.DrawGroupHeader(filesGroupHeader, false);

            (Rect dumpLabel, Rect dumpBtn) = listing.GetRectAsForm();
            SettingsHelper.DrawLabel(dumpLabel, dumpStyleLabel);
            listing.DrawDescription(dumpStyleDescription);

            if (Widgets.ButtonText(dumpBtn, $"TKUtils.DumpStyle.{TkSettings.DumpStyle}".Translate()))
            {
                Find.WindowStack.Add(dumpStyleMenu);
            }

            listing.CheckboxLabeled(minifyDataLabel, ref TkSettings.MinifyData);
            listing.DrawDescription(minifyDataDescription);

            listing.CheckboxLabeled(offloadShopLabel, ref TkSettings.Offload);
            listing.DrawDescription(offloadShopDescription);
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled(asapPurchasesLabel, ref TkSettings.AsapPurchases);
            listing.DrawDescription(asapPurchasesDescription);
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled(trueNeutralLabel, ref TkSettings.TrueNeutral);
            listing.DrawDescription(trueNeutralDescription);
            listing.DrawExperimentalNotice();

            (Rect coinTypeLabel, Rect coinTypeField) = listing.GetRectAsForm();
            SettingsHelper.DrawLabel(coinTypeLabel, broadcasterTypeLabel);
            listing.DrawDescription(broadcasterTypeDescription);
            listing.DrawExperimentalNotice();

            if (Widgets.ButtonText(coinTypeField, $"TKUtils.BroadcasterUserType.{TkSettings.BroadcasterCoinType}".Translate()))
            {
                Find.WindowStack.Add(broadcasterUserTypeMenu);
            }


            listing.DrawGroupHeader(lazyProcessGroupHeader);

            (Rect storeLabel, Rect storeField) = listing.GetRect(Text.LineHeight).ToForm();
            SettingsHelper.DrawLabel(storeLabel, storeRateLabel);
            listing.DrawDescription(storeRateDescription);
            SettingsHelper.DrawAugmentedNumberEntry(storeField, ref buildRateBuffer, ref TkSettings.StoreBuildRate, ref buildRateBufferValid);

            dataTabHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawCommandTweakSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * 48f);

            GUI.BeginGroup(region);
            Widgets.BeginScrollView(region.AtZero(), ref commandTweakPos, viewPort);
            listing.Begin(viewPort);

            listing.DrawGroupHeader(balanceGroupHeader, false);
            listing.CheckboxLabeled(coinRateLabel, ref TkSettings.ShowCoinRate);
            listing.DrawDescription(coinRateDescription);


            listing.DrawGroupHeader(commandHandlerGroupHeader);

            if (TkSettings.Commands)
            {
                (Rect prefixLabel, Rect prefixField) = listing.GetRectAsForm();
                SettingsHelper.DrawLabel(prefixLabel, commandPrefixLabel);
                listing.DrawDescription(commandPrefixDescription);
                TkSettings.Prefix = CommandHelper.ValidatePrefix(Widgets.TextField(prefixField, TkSettings.Prefix));

                (Rect buyPrefixLabel, Rect buyPrefixField) = listing.GetRectAsForm();
                SettingsHelper.DrawLabel(buyPrefixLabel, purchasePrefixLabel);
                listing.DrawDescription(purchasePrefixDescription);
                TkSettings.BuyPrefix = CommandHelper.ValidatePrefix(Widgets.TextField(buyPrefixField, TkSettings.BuyPrefix));
            }

            listing.CheckboxLabeled(commandParserLabel, ref TkSettings.Commands);
            listing.DrawDescription(commandParserDescription);

            if (TkSettings.Commands)
            {
                listing.CheckboxLabeled(toolkitStyleLabel, ref TkSettings.ToolkitStyleCommands);
                listing.DrawDescription(toolkitStyleDescription);
            }

            listing.CheckboxLabeled(commandRouterLabel, ref TkSettings.CommandRouter);
            listing.DrawDescription(commandRouterDescription.Translate());
            listing.DrawExperimentalNotice();


            listing.DrawGroupHeader(installedModsGroupHeader);
            listing.CheckboxLabeled(decorateUtilsLabel, ref TkSettings.DecorateMods);
            listing.DrawDescription(decorateUtilsDescription);

            listing.CheckboxLabeled(versionedModListLabel, ref TkSettings.VersionedModList);
            listing.DrawDescription(versionedModListDescription);


            listing.DrawGroupHeader(buyItemGroupHeader);
            listing.CheckboxLabeled(buyItemBalanceLabel, ref TkSettings.BuyItemBalance);
            listing.DrawDescription(buyItemBalanceDescription);
            listing.CheckboxLabeled(itemSyntaxLabel, ref TkSettings.ForceFullItem);
            listing.DrawDescription(itemSyntaxDescription);


            listing.DrawGroupHeader(lookupGroupHeader);

            (Rect limitLabel, Rect limitField) = listing.GetRectAsForm();
            var buffer = TkSettings.LookupLimit.ToString();

            SettingsHelper.DrawLabel(limitLabel, lookupLimitLabel);
            Widgets.TextFieldNumeric(limitField, ref TkSettings.LookupLimit, ref buffer);
            listing.DrawDescription(lookupLimitDescription);

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        private static void DrawModCompatSettings(Rect region)
        {
            var listing = new Listing_Standard();
            listing.Begin(region);

            listing.DrawModGroupHeader("Humanoid Alien Races", 839005762, false);
            listing.CheckboxLabeled("TKUtils.HAR.PawnKinds.Label".TranslateSimple(), ref TkSettings.PurchasePawnKinds);
            listing.DrawDescription("TKUtils.HAR.PawnKinds.Description".TranslateSimple());

            listing.DrawModGroupHeader("A RimWorld of Magic", 1201382956);
            listing.CheckboxLabeled("TKUtils.TMagic.Classes.Label".TranslateSimple(), ref TkSettings.ClassChanges);
            listing.DrawDescription("TKUtils.TMagic.Classes.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            if (TkSettings.ClassChanges)
            {
                listing.CheckboxLabeled("TKUtils.TMagic.ResetClass.Label".TranslateSimple(), ref TkSettings.ResetClass);
                listing.DrawDescription("TKUtils.TMagic.ResetClass.Description".TranslateSimple());
                listing.DrawExperimentalNotice();
            }

            listing.DrawModGroupHeader("Visual Exceptions", 2538411704);
            listing.CheckboxLabeled("TKUtils.VisualExceptions.SendErrors.Label".TranslateSimple(), ref TkSettings.VisualExceptions);
            listing.DrawDescription("TKUtils.VisualExceptions.SendErrors.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            listing.End();
        }
    }
}
