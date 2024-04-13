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
using ToolkitUtils.UX;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows;

/// <summary>
///     A dialog for drawing Utils' settings in a stateful, consistent
///     way.
/// </summary>
public class UtilsSettingsWindow() : ProxySettingsWindow(TkUtils.Instance)
{
    private const uint VisualExceptionsModId = 2538411704;
    private const int MagicModId = 1201382956;
    private const int HarModId = 839005762;

    private readonly TabWorker _tabWorker = new();

    private string _versionString = null!;
    private string _asapPurchasesDescription = null!;
    private string _asapPurchasesLabel = null!;
    private string _balanceGroupHeader = null!;
    private string _basketGroupHeader = null!;
    private string _broadcasterTypeDescription = null!;
    private string _broadcasterTypeLabel = null!;
    private FloatMenu _broadcasterUserTypeMenu = null!;

    private string _buildRateBuffer = TkSettings.StoreBuildRate.ToString();
    private bool _buildRateBufferValid = true;
    private string _buyItemBalanceDescription = null!;
    private string _buyItemBalanceLabel = null!;
    private string _buyItemGroupHeader = null!;
    private string _coinRateDescription = null!;
    private string _coinRateLabel = null!;
    private string _commandHandlerGroupHeader = null!;
    private string _commandParserDescription = null!;
    private string _commandParserLabel = null!;
    private string _commandPrefixDescription = null!;
    private string _commandPrefixLabel = null!;
    private string _commandRouterDescription = null!;
    private string _commandRouterLabel = null!;
    private Vector2 _commandTweakPos = Vector2.zero;

    private Vector2 _dataScrollPos = Vector2.zero;

    private string _decorateUtilsDescription = null!;
    private string _decorateUtilsLabel = null!;
    private string _dumpStyleDescription = null!;
    private string _dumpStyleLabel = null!;
    private FloatMenu _dumpStyleMenu = null!;
    private string _easterEggsDescription = null!;
    private string _easterEggsLabel = null!;
    private string _emojisDescription = null!;
    private string _emojisGroupHeader = null!;
    private string _emojisLabel = null!;

    private string _filesGroupHeader = null!;
    private string _gatewayGroupHeader = null!;
    private string _gatewayPuffDescription = null!;
    private string _gatewayPuffLabel = null!;
    private string _hairColorDescription = null!;
    private string _hairColorLabel = null!;
    private string _installedModsGroupHeader = null!;
    private string _itemSyntaxDescription = null!;
    private string _itemSyntaxLabel = null!;
    private string _lazyProcessGroupHeader = null!;
    private string _lookupGroupHeader = null!;
    private string _lookupLimitDescription = null!;
    private string _lookupLimitLabel = null!;
    private string _minifyDataDescription = null!;
    private string _minifyDataLabel = null!;
    private string _offloadShopDescription = null!;
    private string _offloadShopLabel = null!;
    private string _purchasePrefixDescription = null!;
    private string _purchasePrefixLabel = null!;
    private string _storeRateDescription = null!;
    private string _storeRateLabel = null!;
    private string _toolkitStyleDescription = null!;
    private string _toolkitStyleLabel = null!;
    private string _trueNeutralDescription = null!;
    private string _trueNeutralLabel = null!;
    private string _versionedModListDescription = null!;
    private string _versionedModListLabel = null!;
    private string _viewerGroupHeader = null!;

    /// <inheritdoc cref="Window.PreOpen" />
    public override void PreOpen()
    {
        foreach (ModItem mod in Data.Mods)
        {
            if (!string.Equals(mod.Name, TkUtils.Instance.Content?.Name))
            {
                continue;
            }

            _versionString = $"v{mod.Version}";

            break;
        }

        _tabWorker.AddTab(
            new TabItem { ContentDrawer = DrawGeneralSettings, Label = "TKUtils.General.Label".TranslateSimple(), Tooltip = "TKUtils.General.Tooltip".TranslateSimple() }
        );

        _tabWorker.AddTab(
            new TabItem { ContentDrawer = DrawDataSettings, Label = "TKUtils.Data.Label".TranslateSimple(), Tooltip = "TKUtils.Data.Tooltip".TranslateSimple() }
        );

        _tabWorker.AddTab(
            new TabItem
            {
                ContentDrawer = DrawCommandTweakSettings,
                Label = "TKUtils.CommandTweaks.Label".TranslateSimple(),
                Tooltip = "TKUtils.CommandTweaks.Tooltip".TranslateSimple()
            }
        );

        _tabWorker.AddTab(
            new TabItem
            {
                ContentDrawer = DrawModCompatSettings, Label = "TKUtils.ModCompat.Label".TranslateSimple(), Tooltip = "TKUtils.ModCompat.Tooltip".TranslateSimple()
            }
        );

        base.PreOpen();
    }

    /// <inheritdoc cref="Window.PostOpen" />
    public override void PostOpen()
    {
        base.PostOpen();

        _dumpStyleMenu = new FloatMenu(
            new List<FloatMenuOption>
            {
                new("TKUtils.DumpStyle.SingleFile".TranslateSimple(), () => TkSettings.DumpStyle = nameof(DumpStyle.SingleFile)),
                new("TKUtils.DumpStyle.MultiFile".TranslateSimple(), () => TkSettings.DumpStyle = nameof(DumpStyle.MultiFile))
            }
        );

        _broadcasterUserTypeMenu = new FloatMenu(
            new List<FloatMenuOption>
            {
                new("TKUtils.BroadcasterUserType.Broadcaster".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Broadcaster)),
                new("TKUtils.BroadcasterUserType.Subscriber".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Subscriber)),
                new("TKUtils.BroadcasterUserType.Vip".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Vip)),
                new("TKUtils.BroadcasterUserType.Moderator".TranslateSimple(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.Moderator)),
                new("None".TranslateSimple().CapitalizeFirst(), () => TkSettings.BroadcasterCoinType = nameof(UserCoinType.None))
            }
        );
    }

    /// <inheritdoc cref="ProxySettingsWindow.GetTranslations" />
    protected override void GetTranslations()
    {
        _filesGroupHeader = "TKUtils.Data.Files".TranslateSimple();
        _dumpStyleLabel = "TKUtils.DumpStyle.Label".TranslateSimple();
        _dumpStyleDescription = "TKUtils.DumpStyle.Description".TranslateSimple();
        _minifyDataLabel = "TKUtils.MinifyData.Label".TranslateSimple();
        _minifyDataDescription = "TKUtils.MinifyData.Description".TranslateSimple();
        _offloadShopLabel = "TKUtils.OffloadShop.Label".TranslateSimple();
        _offloadShopDescription = "TKUtils.OffloadShop.Description".TranslateSimple();
        _asapPurchasesLabel = "TKUtils.DoPurchasesAsap.Label".TranslateSimple();
        _asapPurchasesDescription = "TKUtils.DoPurchasesAsap.Description".TranslateSimple();
        _trueNeutralLabel = "TKUtils.TrueNeutral.Label".TranslateSimple();
        _trueNeutralDescription = "TKUtils.TrueNeutral.Description".TranslateSimple();
        _broadcasterTypeLabel = "TKUtils.BroadcasterUserType.Label".TranslateSimple();
        _broadcasterTypeDescription = "TKUtils.BroadcasterUserType.Description".TranslateSimple();
        _lazyProcessGroupHeader = "TKUtils.Data.LazyProcess".TranslateSimple();
        _storeRateLabel = "TKUtils.StoreRate.Label".TranslateSimple();
        _storeRateDescription = "TKUtils.StoreRate.Description".TranslateSimple();

        _emojisGroupHeader = "TKUtils.General.Emojis".TranslateSimple();
        _emojisLabel = "TKUtils.Emojis.Label".TranslateSimple();
        _emojisDescription = "TKUtils.Emojis.Description".TranslateSimple();
        _viewerGroupHeader = "TKUtils.General.Viewer".TranslateSimple();
        _hairColorLabel = "TKUtils.HairColor.Label".TranslateSimple();
        _hairColorDescription = "TKUtils.HairColor.Description".TranslateSimple();
        _gatewayGroupHeader = "TKUtils.General.Gateway".TranslateSimple();
        _gatewayPuffLabel = "TKUtils.GatewayPuff.Label".TranslateSimple();
        _gatewayPuffDescription = "TKUtils.GatewayPuff.Description".TranslateSimple();
        _basketGroupHeader = "TKUtils.General.Basket".TranslateSimple();
        _easterEggsLabel = "TKUtils.EasterEggs.Label".TranslateSimple();
        _easterEggsDescription = "TKUtils.EasterEggs.Description".TranslateSimple();

        _balanceGroupHeader = "TKUtils.CommandTweaks.Balance".TranslateSimple();
        _coinRateLabel = "TKUtils.CoinRate.Label".TranslateSimple();
        _coinRateDescription = "TKUtils.CoinRate.Description".TranslateSimple();
        _commandHandlerGroupHeader = "TKUtils.CommandTweaks.Handler".TranslateSimple();
        _commandPrefixLabel = "TKUtils.CommandPrefix.Label".TranslateSimple();
        _commandPrefixDescription = "TKUtils.CommandPrefix.Description".TranslateSimple();
        _purchasePrefixLabel = "TKUtils.PurchasePrefix.Label".TranslateSimple();
        _purchasePrefixDescription = "TKUtils.PurchasePrefix.Description".TranslateSimple();
        _commandParserLabel = "TKUtils.CommandParser.Label".TranslateSimple();
        _commandParserDescription = "TKUtils.CommandParser.Description".TranslateSimple();
        _toolkitStyleLabel = "TKUtils.ToolkitStyleCommands.Label".TranslateSimple();
        _toolkitStyleDescription = "TKUtils.ToolkitStyleCommands.Description".TranslateSimple();
        _commandRouterLabel = "TKUtils.CommandRouter.Label".TranslateSimple();
        _commandRouterDescription = "TKUtils.CommandRouter.Description".TranslateSimple();
        _installedModsGroupHeader = "TKUtils.CommandTweaks.InstalledMods".TranslateSimple();
        _decorateUtilsLabel = "TKUtils.DecorateUtils.Label".TranslateSimple();
        _decorateUtilsDescription = "TKUtils.DecorateUtils.Description".TranslateSimple();
        _versionedModListLabel = "TKUtils.VersionedModList.Label".TranslateSimple();
        _versionedModListDescription = "TKUtils.VersionedModList.Description".TranslateSimple();
        _buyItemGroupHeader = "TKUtils.CommandTweaks.BuyItem".TranslateSimple();
        _buyItemBalanceLabel = "TKUtils.BuyItemBalance.Label".TranslateSimple();
        _buyItemBalanceDescription = "TKUtils.BuyItemBalance.Description".TranslateSimple();
        _itemSyntaxLabel = "TKUtils.BuyItemFullSyntax.Label".TranslateSimple();
        _itemSyntaxDescription = "TKUtils.BuyItemFullSyntax.Description".TranslateSimple();
        _lookupGroupHeader = "TKUtils.CommandTweaks.Lookup".TranslateSimple();
        _lookupLimitLabel = "TKUtils.LookupLimit.Label".TranslateSimple();
        _lookupLimitDescription = "TKUtils.LookupLimit.Description".TranslateSimple();
    }

    /// <inheritdoc cref="ProxySettingsWindow.DrawSettings" />
    protected override void DrawSettings(Rect region)
    {
        GUI.BeginGroup(region);

        var tabBarRect = new Rect(0f, 0f, region.width, Text.LineHeight * 2f);
        var tabPanelRect = new Rect(0f, tabBarRect.height, region.width, region.height - tabBarRect.height);

        GUI.BeginGroup(tabBarRect);
        _tabWorker.Draw(tabBarRect.AtZero(), paneled: true);

        LabelDrawer.Draw(tabBarRect, _versionString, Color.grey, TextAnchor.MiddleRight);
        GUI.EndGroup();

        GUI.BeginGroup(tabPanelRect);
        _tabWorker.SelectedTab?.Draw(tabPanelRect.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawGeneralSettings(Rect region)
    {
        var listing = new Listing_Standard();
        listing.Begin(region);

        listing.GroupHeader(_emojisGroupHeader, false);
        listing.CheckboxLabeled(_emojisLabel, ref TkSettings.Emojis);
        listing.DrawDescription(_emojisDescription);


        listing.GroupHeader(_viewerGroupHeader);
        listing.CheckboxLabeled(_hairColorLabel, ref TkSettings.HairColor);
        listing.DrawDescription(_hairColorDescription);

        listing.GroupHeader(_gatewayGroupHeader);
        listing.CheckboxLabeled(_gatewayPuffLabel, ref TkSettings.GatewayPuff);
        listing.DrawDescription(_gatewayPuffDescription);

        listing.GroupHeader(_basketGroupHeader);
        listing.CheckboxLabeled(_easterEggsLabel, ref TkSettings.EasterEggs);
        listing.DrawDescription(_easterEggsDescription);

        listing.End();
    }

    private void DrawDataSettings(Rect region)
    {
        var view = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * 32f);
        var listing = new Listing_Standard();
        GUI.BeginGroup(region);
        Widgets.BeginScrollView(region.AtZero(), ref _dataScrollPos, view);
        listing.Begin(view);

        listing.GroupHeader(_filesGroupHeader, false);

        (Rect dumpLabel, Rect dumpBtn) = listing.Split();
        LabelDrawer.Draw(dumpLabel, _dumpStyleLabel);
        listing.DrawDescription(_dumpStyleDescription);

        if (Widgets.ButtonText(dumpBtn, $"TKUtils.DumpStyle.{TkSettings.DumpStyle}".Translate()))
        {
            Find.WindowStack.Add(_dumpStyleMenu);
        }

        listing.CheckboxLabeled(_minifyDataLabel, ref TkSettings.MinifyData);
        listing.DrawDescription(_minifyDataDescription);

        listing.CheckboxLabeled(_offloadShopLabel, ref TkSettings.Offload);
        listing.DrawDescription(_offloadShopDescription);
        listing.DrawExperimentalNotice();

        listing.CheckboxLabeled(_asapPurchasesLabel, ref TkSettings.AsapPurchases);
        listing.DrawDescription(_asapPurchasesDescription);
        listing.DrawExperimentalNotice();

        listing.CheckboxLabeled(_trueNeutralLabel, ref TkSettings.TrueNeutral);
        listing.DrawDescription(_trueNeutralDescription);
        listing.DrawExperimentalNotice();

        (Rect coinTypeLabel, Rect coinTypeField) = listing.Split();
        LabelDrawer.Draw(coinTypeLabel, _broadcasterTypeLabel);
        listing.DrawDescription(_broadcasterTypeDescription);
        listing.DrawExperimentalNotice();

        if (Widgets.ButtonText(coinTypeField, $"TKUtils.BroadcasterUserType.{TkSettings.BroadcasterCoinType}".Translate()))
        {
            Find.WindowStack.Add(_broadcasterUserTypeMenu);
        }


        listing.GroupHeader(_lazyProcessGroupHeader);

        (Rect storeLabel, Rect storeField) = listing.Split();
        LabelDrawer.Draw(storeLabel, _storeRateLabel);
        listing.DrawDescription(_storeRateDescription);

        if (FieldDrawer.DrawNumberField(storeField, out int value, ref _buildRateBuffer, ref _buildRateBufferValid))
        {
            TkSettings.StoreBuildRate = value;
        }

        listing.End();
        Widgets.EndScrollView();
        GUI.EndGroup();
    }

    private void DrawCommandTweakSettings(Rect region)
    {
        var listing = new Listing_Standard();
        var viewPort = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * 48f);

        GUI.BeginGroup(region);
        Widgets.BeginScrollView(region.AtZero(), ref _commandTweakPos, viewPort);
        listing.Begin(viewPort);

        listing.GroupHeader(_balanceGroupHeader, false);
        listing.CheckboxLabeled(_coinRateLabel, ref TkSettings.ShowCoinRate);
        listing.DrawDescription(_coinRateDescription);


        listing.GroupHeader(_commandHandlerGroupHeader);

        if (TkSettings.Commands)
        {
            (Rect prefixLabel, Rect prefixField) = listing.Split();
            LabelDrawer.Draw(prefixLabel, _commandPrefixLabel);
            listing.DrawDescription(_commandPrefixDescription);
            TkSettings.Prefix = CommandHelper.ValidatePrefix(Widgets.TextField(prefixField, TkSettings.Prefix));

            (Rect buyPrefixLabel, Rect buyPrefixField) = listing.Split();
            LabelDrawer.Draw(buyPrefixLabel, _purchasePrefixLabel);
            listing.DrawDescription(_purchasePrefixDescription);
            TkSettings.BuyPrefix = CommandHelper.ValidatePrefix(Widgets.TextField(buyPrefixField, TkSettings.BuyPrefix));
        }

        listing.CheckboxLabeled(_commandParserLabel, ref TkSettings.Commands);
        listing.DrawDescription(_commandParserDescription);

        if (TkSettings.Commands)
        {
            listing.CheckboxLabeled(_toolkitStyleLabel, ref TkSettings.ToolkitStyleCommands);
            listing.DrawDescription(_toolkitStyleDescription);
        }

        bool commandRouter = TkSettings.CommandRouter;
        listing.CheckboxLabeled(_commandRouterLabel, ref commandRouter);

        if (commandRouter != TkSettings.CommandRouter)
        {
            RuntimeChecker.ValidateTicker();

            TkSettings.CommandRouter = commandRouter;
        }

        listing.DrawDescription(_commandRouterDescription);
        listing.DrawExperimentalNotice();

        listing.GroupHeader(_installedModsGroupHeader);
        listing.CheckboxLabeled(_decorateUtilsLabel, ref TkSettings.DecorateMods);
        listing.DrawDescription(_decorateUtilsDescription);

        listing.CheckboxLabeled(_versionedModListLabel, ref TkSettings.VersionedModList);
        listing.DrawDescription(_versionedModListDescription);


        listing.GroupHeader(_buyItemGroupHeader);
        listing.CheckboxLabeled(_buyItemBalanceLabel, ref TkSettings.BuyItemBalance);
        listing.DrawDescription(_buyItemBalanceDescription);
        listing.CheckboxLabeled(_itemSyntaxLabel, ref TkSettings.ForceFullItem);
        listing.DrawDescription(_itemSyntaxDescription);


        listing.GroupHeader(_lookupGroupHeader);

        (Rect limitLabel, Rect limitField) = listing.Split();
        var buffer = TkSettings.LookupLimit.ToString();

        LabelDrawer.Draw(limitLabel, _lookupLimitLabel);
        Widgets.TextFieldNumeric(limitField, ref TkSettings.LookupLimit, ref buffer);
        listing.DrawDescription(_lookupLimitDescription);

        GUI.EndGroup();
        Widgets.EndScrollView();
        listing.End();
    }

    private static void DrawModCompatSettings(Rect region)
    {
        var listing = new Listing_Standard();
        listing.Begin(region);

        listing.ModGroupHeader("Humanoid Alien Races", HarModId, false);
        listing.CheckboxLabeled("TKUtils.HAR.PawnKinds.Label".TranslateSimple(), ref TkSettings.PurchasePawnKinds);
        listing.DrawDescription("TKUtils.HAR.PawnKinds.Description".TranslateSimple());

        listing.ModGroupHeader("A RimWorld of Magic", MagicModId);
        listing.CheckboxLabeled("TKUtils.TMagic.Classes.Label".TranslateSimple(), ref TkSettings.ClassChanges);
        listing.DrawDescription("TKUtils.TMagic.Classes.Description".TranslateSimple());
        listing.DrawExperimentalNotice();

        if (TkSettings.ClassChanges)
        {
            listing.CheckboxLabeled("TKUtils.TMagic.ResetClass.Label".TranslateSimple(), ref TkSettings.ResetClass);
            listing.DrawDescription("TKUtils.TMagic.ResetClass.Description".TranslateSimple());
            listing.DrawExperimentalNotice();
        }

        listing.ModGroupHeader("Visual Exceptions", VisualExceptionsModId);
        listing.CheckboxLabeled("TKUtils.VisualExceptions.SendErrors.Label".TranslateSimple(), ref TkSettings.VisualExceptions);
        listing.DrawDescription("TKUtils.VisualExceptions.SendErrors.Description".TranslateSimple());
        listing.DrawExperimentalNotice();

        listing.End();
    }
}
