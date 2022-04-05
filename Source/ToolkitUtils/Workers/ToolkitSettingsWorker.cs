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
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit;
using TwitchToolkit.Settings;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public static class ToolkitSettingsWorker
    {
        private static readonly float LineHeight = Mathf.FloorToInt(Text.SmallFontHeight * 1.25f);
        private static TabWorker _tabWorker;
        private static TabItem _coinTabItem;
        private static TabItem _cooldownTabItem;
        private static TabItem _karmaTabItem;
        private static TabItem _patchesTabItem;
        private static TabItem _storeTabItem;
        private static TabItem _storytellerTabItem;
        private static TabItem _viewerTabItem;
        private static string _startingBalanceBuffer;
        private static string _coinIntervalBuffer;
        private static string _coinAmountBuffer;
        private static string _minimumPurchaseBuffer;
        private static string _halfCoinsBuffer;
        private static string _noCoinsBuffer;
        private static string _voteTimeBuffer;
        private static string _voteOptionsBuffer;
        private static string _eventCooldownBuffer;
        private static string _maxBadEventsBuffer;
        private static string _maxGoodEventsBuffer;
        private static string _maxNeutralEventsBuffer;
        private static string _maxItemEventsBuffer;
        private static string _queueCostBuffer;
        private static string _subCoinBuffer;
        private static string _subMultBuffer;
        private static string _subVotesBuffer;
        private static string _vipCoinBuffer;
        private static string _vipMultBuffer;
        private static string _vipVotesBuffer;
        private static string _modCoinBuffer;
        private static string _modMultBuffer;
        private static string _modVotesBuffer;
        private static string _startKarmaBuffer;
        private static string _karmaCapBuffer;
        private static string _minKarmaBuffer;
        private static string _minGiftingKarmaBuffer;
        private static string _minGiftKarmaBuffer;
        private static string _tOneGoodKarmaBuffer;
        private static string _tOneNeutralKarmaBuffer;
        private static string _tOneBadKarmaBuffer;
        private static string _tTwoGoodKarmaBuffer;
        private static string _tTwoNeutralKarmaBuffer;
        private static string _tTwoBadKarmaBuffer;
        private static string _tThreeGoodKarmaBuffer;
        private static string _tThreeNeutralKarmaBuffer;
        private static string _tThreeBadKarmaBuffer;
        private static string _tFourGoodKarmaBuffer;
        private static string _tFourNeutralKarmaBuffer;
        private static string _tFourBadKarmaBuffer;
        private static Vector2 _viewerScrollPos = Vector2.zero;
        private static Vector2 _karmaScrollPos = Vector2.zero;
        private static Vector2 _coinsScrollPos = Vector2.zero;
        private static Vector2 _patchesScrollPos = Vector2.zero;

        public static void Draw(Rect region)
        {
            GUI.BeginGroup(region);
            Rect wikiRect = LayoutHelper.IconRect(region.width - Text.SmallFontHeight, 0f, Text.SmallFontHeight - 4f, Text.SmallFontHeight - 4f);

            if (Widgets.ButtonImage(wikiRect, Textures.QuestionMark))
            {
                Application.OpenURL("https://storytoolkit.fandom.com/wiki/StoryToolkit_Wiki");
            }

            if (_tabWorker == null)
            {
                CreateTabs();
            }

            DrawSettings(region.AtZero());
            GUI.EndGroup();
        }

        private static void CreateTabs()
        {
            _tabWorker = new TabWorker();
            _coinTabItem = new TabItem { ContentDrawer = DrawCoinSettings, Label = "TwitchToolkitCoins".Translate(), Tooltip = "TKUtils.Coins.Tooltip".Localize() };

            _cooldownTabItem = new TabItem
            {
                ContentDrawer = DrawCooldownSettings, Label = "TwitchToolkitCooldowns".Translate(), Tooltip = "TKUtils.Cooldowns.Tooltip".Localize()
            };

            _karmaTabItem = new TabItem { ContentDrawer = DrawKarmaSettings, Label = "TwitchToolkitKarma".Translate(), Tooltip = "TKUtils.Karma.Tooltip".Localize() };

            _patchesTabItem = new TabItem { ContentDrawer = DrawPatchesSettings, Label = "TKUtils.Addons.Label".Localize(), Tooltip = "TKUtils.Addons.Tooltip".Localize() };
            _storeTabItem = new TabItem { ContentDrawer = DrawStoreSettings, Label = "TwitchToolkitStore".Translate(), Tooltip = "TKUtils.Store.Tooltip".Localize() };

            _storytellerTabItem = new TabItem
            {
                ContentDrawer = DrawStorytellerSettings, Label = "TKUtils.Storyteller".Localize(), Tooltip = "TKUtils.Storyteller.Tooltip".Localize()
            };

            _viewerTabItem = new TabItem { ContentDrawer = DrawViewerSettings, Label = "TwitchToolkitViewers".Translate(), Tooltip = "TKUtils.Viewers.Tooltip".Localize() };

            _tabWorker.AddTab(_coinTabItem);
            _tabWorker.AddTab(_cooldownTabItem);
            _tabWorker.AddTab(_karmaTabItem);
            _tabWorker.AddTab(_patchesTabItem);
            _tabWorker.AddTab(_storeTabItem);
            _tabWorker.AddTab(_storytellerTabItem);
            _tabWorker.AddTab(_viewerTabItem);
        }

        private static void DrawCoinSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var viewRect = new Rect(0f, 0f, region.width - 16f, Text.SmallFontHeight * 32f);
            Widgets.BeginScrollView(region, ref _coinsScrollPos, viewRect);

            listing.Begin(viewRect);

            listing.CheckboxLabeled(
                ToolkitSettings.CoinInterval > 1
                    ? "TKUtils.EarningCoins.Label".LocalizeKeyed(ToolkitSettings.CoinInterval)
                    : "TKUtils.EarningCoins.Singular.Label".Localize(),
                ref ToolkitSettings.EarningCoins
            );

            listing.DrawDescription(
                ToolkitSettings.CoinInterval > 1
                    ? "TKUtils.EarningCoins.Description".LocalizeKeyed(ToolkitSettings.CoinInterval)
                    : "TKUtils.EarningCoins.Singular.Label".Localize()
            );

            if (ToolkitSettings.EarningCoins)
            {
                (Rect startBalLabel, Rect startBalField) = listing.Split(0.85f);
                UiHelper.Label(startBalLabel, "TKUtils.StartingBalance.Label".Localize());
                _startingBalanceBuffer ??= ToolkitSettings.StartingBalance.ToString();
                Widgets.TextFieldNumeric(startBalField, ref ToolkitSettings.StartingBalance, ref _startingBalanceBuffer);
                listing.DrawDescription("TKUtils.StartingBalance.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(150)));

                (Rect coinIntLabel, Rect coinIntField) = listing.Split(0.85f);
                UiHelper.Label(coinIntLabel, "TKUtils.CoinInterval.Label".Localize());
                _coinIntervalBuffer ??= ToolkitSettings.CoinInterval.ToString();
                Widgets.TextFieldNumeric(coinIntField, ref ToolkitSettings.CoinInterval, ref _coinIntervalBuffer);
                listing.DrawDescription("TKUtils.CoinInterval.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(2)));

                (Rect coinAmountLabel, Rect coinAmountField) = listing.Split(0.85f);
                UiHelper.Label(coinAmountLabel, "TKUtils.CoinAmount.Label".Localize());
                _coinAmountBuffer ??= ToolkitSettings.CoinAmount.ToString();
                Widgets.TextFieldNumeric(coinAmountField, ref ToolkitSettings.CoinAmount, ref _coinAmountBuffer);

                listing.DrawDescription(
                    (ToolkitSettings.CoinInterval > 1
                        ? "TKUtils.CoinAmount.Description".LocalizeKeyed(ToolkitSettings.CoinInterval.ToString("N0"))
                        : "TKUtils.CoinAmount.Singular.Description".Localize()).AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(30))
                );
            }

            (Rect minPurLabel, Rect minPurField) = listing.Split(0.85f);
            UiHelper.Label(minPurLabel, "TKUtils.MinimumPurchaseAmount.Label".Localize());
            _minimumPurchaseBuffer ??= ToolkitSettings.MinimumPurchasePrice.ToString();
            Widgets.TextFieldNumeric(minPurField, ref ToolkitSettings.MinimumPurchasePrice, ref _minimumPurchaseBuffer);
            listing.DrawDescription("TKUtils.MinimumPurchaseAmount.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(60)));

            listing.Gap();
            listing.CheckboxLabeled("TKUtils.UnlimitedCoins.Label".Localize(), ref ToolkitSettings.UnlimitedCoins);
            listing.DrawDescription("TKUtils.UnlimitedCoins.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize())));
            listing.GapLine();
            listing.Gap();

            listing.CheckboxLabeled("TKUtils.RequireParticipation.Label".Localize(), ref ToolkitSettings.ChatReqsForCoins);
            listing.DrawDescription("TKUtils.RequireParticipation.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));

            if (!ToolkitSettings.ChatReqsForCoins)
            {
                listing.End();
                Widgets.EndScrollView();

                return;
            }

            (Rect halfCoinsLabel, Rect halfCoinsField) = listing.Split(0.85f);
            UiHelper.Label(halfCoinsLabel, "TKUtils.HalfCoins.Label".Localize());
            _halfCoinsBuffer ??= ToolkitSettings.TimeBeforeHalfCoins.ToString();
            Widgets.TextFieldNumeric(halfCoinsField, ref ToolkitSettings.TimeBeforeHalfCoins, ref _halfCoinsBuffer);
            listing.DrawDescription("TKUtils.HalfCoins.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(30)));

            (Rect noCoinsLabel, Rect noCoinsField) = listing.Split(0.85f);
            UiHelper.Label(noCoinsLabel, "TKUtils.NoCoins.Label".Localize());
            _noCoinsBuffer ??= ToolkitSettings.TimeBeforeNoCoins.ToString();
            Widgets.TextFieldNumeric(noCoinsField, ref ToolkitSettings.TimeBeforeNoCoins, ref _noCoinsBuffer);
            listing.DrawDescription("TKUtils.NoCoins.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(60)));

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawCooldownSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect cooldownLabel, Rect cooldownField) = listing.Split(0.85f);
            UiHelper.Label(cooldownLabel, "TKUtils.CooldownPeriod.Label".Localize());
            _eventCooldownBuffer ??= ToolkitSettings.EventCooldownInterval.ToString();
            Widgets.TextFieldNumeric(cooldownField, ref ToolkitSettings.EventCooldownInterval, ref _eventCooldownBuffer, 1f, 15f);

            listing.DrawDescription(
                (ToolkitSettings.EventCooldownInterval > 1
                    ? "TKUtils.CooldownPeriod.Description".LocalizeKeyed(ToolkitSettings.EventCooldownInterval)
                    : "TKUtils.CooldownPeriod.Singular.Description".Localize()).AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(15))
            );

            listing.Gap();

            listing.CheckboxLabeled("TKUtils.MaxEvents.Label".Localize(), ref ToolkitSettings.MaxEvents);
            listing.DrawDescription("TKUtils.MaxEvents.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize())));
            listing.Gap();

            if (ToolkitSettings.MaxEvents)
            {
                (Rect badEventsLabel, Rect badEventsField) = listing.Split(0.85f);
                UiHelper.Label(badEventsLabel, "TKUtils.MaxBadEvents.Label".Localize());
                _maxBadEventsBuffer ??= ToolkitSettings.MaxBadEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(badEventsField, ref ToolkitSettings.MaxBadEventsPerInterval, ref _maxBadEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".LocalizeKeyed(3));

                (Rect goodEventsLabel, Rect goodEventsField) = listing.Split(0.85f);
                UiHelper.Label(goodEventsLabel, "TKUtils.MaxGoodEvents.Label".Localize());
                _maxGoodEventsBuffer ??= ToolkitSettings.MaxGoodEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(goodEventsField, ref ToolkitSettings.MaxGoodEventsPerInterval, ref _maxGoodEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".LocalizeKeyed(10));

                (Rect neutralEventsLabel, Rect neutralEventsField) = listing.Split(0.85f);
                UiHelper.Label(neutralEventsLabel, "TKUtils.MaxNeutralEvents.Label".Localize());
                _maxNeutralEventsBuffer ??= ToolkitSettings.MaxNeutralEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(neutralEventsField, ref ToolkitSettings.MaxNeutralEventsPerInterval, ref _maxNeutralEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".LocalizeKeyed(10));

                (Rect itemEventsLabel, Rect itemEventsField) = listing.Split(0.85f);
                UiHelper.Label(itemEventsLabel, "TKUtils.MaxItemEvents.Label".Localize());
                _maxItemEventsBuffer ??= ToolkitSettings.MaxCarePackagesPerInterval.ToString();
                Widgets.TextFieldNumeric(itemEventsField, ref ToolkitSettings.MaxCarePackagesPerInterval, ref _maxItemEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".LocalizeKeyed(10));

                listing.Gap();
            }

            listing.CheckboxLabeled("TKUtils.EventCooldowns.Label".Localize(), ref ToolkitSettings.EventsHaveCooldowns);
            listing.DrawDescription("TKUtils.EventCooldowns.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));

            listing.End();
        }

        private static void DrawKarmaSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, region.width - 16f, Text.SmallFontHeight * 72f);

            Widgets.BeginScrollView(region, ref _karmaScrollPos, viewPort);
            listing.Begin(viewPort);

            (Rect startKarmaLabel, Rect startKarmaField) = listing.Split(0.85f);
            UiHelper.Label(startKarmaLabel, "TKUtils.StartingKarma.Label".Localize());
            _startKarmaBuffer ??= ToolkitSettings.StartingKarma.ToString();
            Widgets.TextFieldNumeric(startKarmaField, ref ToolkitSettings.StartingKarma, ref _startKarmaBuffer, 50f, 250f);
            listing.DrawDescription("TKUtils.StartingKarma.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(100)));

            (Rect karmaCapLabel, Rect karmaCapField) = listing.Split(0.85f);
            UiHelper.Label(karmaCapLabel, "TKUtils.KarmaCap.Label".Localize());
            _karmaCapBuffer ??= ToolkitSettings.KarmaCap.ToString();
            Widgets.TextFieldNumeric(karmaCapField, ref ToolkitSettings.KarmaCap, ref _karmaCapBuffer);
            listing.DrawDescription("TKUtils.KarmaCap.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(140)));

            listing.CheckboxLabeled("TKUtils.NegativeKarma.Label".Localize(), ref ToolkitSettings.BanViewersWhoPurchaseAlwaysBad);
            listing.DrawDescription("TKUtils.NegativeKarma.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));
            listing.Gap();

            (Rect karMinLabel, Rect karMinField) = listing.Split(0.85f);
            UiHelper.Label(karMinLabel, "TKUtils.KarmaMinimum.Label".Localize());
            _minKarmaBuffer ??= ToolkitSettings.KarmaMinimum.ToString();
            Widgets.TextFieldNumeric(karMinField, ref ToolkitSettings.KarmaMinimum, ref _minKarmaBuffer, -200000, 100f);
            listing.DrawDescription("TKUtils.KarmaMinimum.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(10)));
            listing.Gap();

            listing.CheckboxLabeled("TKUtils.GiftingRequiresKarma.Label".Localize(), ref ToolkitSettings.KarmaReqsForGifting);
            listing.DrawDescription("TKUtils.GiftingRequiresKarma.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize())));
            listing.Gap();

            if (ToolkitSettings.KarmaReqsForGifting)
            {
                (Rect minKarmaGiftLabel, Rect minKarmaGiftField) = listing.Split(0.85f);
                UiHelper.Label(minKarmaGiftLabel, "TKUtils.KarmaForReceiving.Label".Localize());
                _minGiftKarmaBuffer ??= ToolkitSettings.MinimumKarmaToRecieveGifts.ToString();
                Widgets.TextFieldNumeric(minKarmaGiftField, ref ToolkitSettings.MinimumKarmaToRecieveGifts, ref _minGiftKarmaBuffer, 10f);
                listing.DrawDescription("TKUtils.KarmaForReceiving.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(35)));

                (Rect minKarmaGiftingLabel, Rect minKarmaGiftingField) = listing.Split(0.85f);
                UiHelper.Label(minKarmaGiftingLabel, "TKUtils.KarmaForGifting.Label".Localize());
                _minGiftingKarmaBuffer ??= ToolkitSettings.MinimumKarmaToSendGifts.ToString();
                Widgets.TextFieldNumeric(minKarmaGiftingField, ref ToolkitSettings.MinimumKarmaToSendGifts, ref _minGiftingKarmaBuffer, 20f, 150f);
                listing.DrawDescription("TKUtils.KarmaForGifting.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(100)));
                listing.Gap();
            }

            string goodViewersText = "TwitchToolkitGoodViewers".Translate();

            string goodViewersKarmaText = "TKUtils.Karma.Description".LocalizeKeyed(
                goodViewersText.ToLowerInvariant(),
                goodViewersText.ToLowerInvariant().CapitalizeFirst(),
                (ToolkitSettings.KarmaCap * 0.56).ToString("N0"),
                ToolkitSettings.KarmaCap.ToString("N0")
            );

            listing.GroupHeader(goodViewersText);
            (Rect tOneGoodLabel, Rect tOneGoodField) = listing.Split(0.85f);
            UiHelper.Label(tOneGoodLabel, "TKUtils.GoodKarma.Label".Localize());
            _tOneGoodKarmaBuffer ??= ToolkitSettings.TierOneGoodBonus.ToString();
            Widgets.TextFieldNumeric(tOneGoodField, ref ToolkitSettings.TierOneGoodBonus, ref _tOneGoodKarmaBuffer, 1f);
            listing.DrawDescription(goodViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(16)));

            (Rect tOneNeutralLabel, Rect tOneNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tOneNeutralLabel, "TKUtils.NeutralKarma.Label".Localize());
            _tOneNeutralKarmaBuffer ??= ToolkitSettings.TierOneNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tOneNeutralField, ref ToolkitSettings.TierOneNeutralBonus, ref _tOneNeutralKarmaBuffer, 1f);
            listing.DrawDescription(goodViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(36)));

            (Rect tOneBadLabel, Rect tOneBadField) = listing.Split(0.85f);
            UiHelper.Label(tOneBadLabel, "TKUtils.BadKarma.Label".Localize());
            _tOneBadKarmaBuffer ??= ToolkitSettings.TierOneBadBonus.ToString();
            Widgets.TextFieldNumeric(tOneBadField, ref ToolkitSettings.TierOneBadBonus, ref _tOneBadKarmaBuffer, 1f);
            listing.DrawDescription(goodViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(24)));

            string neutralViewersText = "TwitchToolkitNeutralViewers".Translate();

            string neutralViewersKarmaText = "TKUtils.Karma.Description".LocalizeKeyed(
                neutralViewersText.ToLowerInvariant(),
                neutralViewersText.ToLowerInvariant().CapitalizeFirst(),
                (ToolkitSettings.KarmaCap * 0.37).ToString("N0"),
                (ToolkitSettings.KarmaCap * 0.55).ToString("N0")
            );

            listing.GroupHeader(neutralViewersText);
            (Rect tTwoGoodLabel, Rect tTwoGoodField) = listing.Split(0.85f);
            UiHelper.Label(tTwoGoodLabel, "TKUtils.GoodKarma.Label".Localize());
            _tTwoGoodKarmaBuffer ??= ToolkitSettings.TierTwoGoodBonus.ToString();
            Widgets.TextFieldNumeric(tTwoGoodField, ref ToolkitSettings.TierTwoGoodBonus, ref _tTwoGoodKarmaBuffer, 1f);
            listing.DrawDescription(neutralViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(10)));

            (Rect tTwoNeutralLabel, Rect tTwoNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tTwoNeutralLabel, "TKUtils.NeutralKarma.Label".Localize());
            _tTwoNeutralKarmaBuffer ??= ToolkitSettings.TierTwoNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tTwoNeutralField, ref ToolkitSettings.TierTwoNeutralBonus, ref _tTwoNeutralKarmaBuffer, 1f);
            listing.DrawDescription(neutralViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(30)));

            (Rect tTwoBadLabel, Rect tTwoBadField) = listing.Split(0.85f);
            UiHelper.Label(tTwoBadLabel, "TKUtils.BadKarma.Label".Localize());
            _tTwoBadKarmaBuffer ??= ToolkitSettings.TierTwoBadBonus.ToString();
            Widgets.TextFieldNumeric(tTwoBadField, ref ToolkitSettings.TierTwoBadBonus, ref _tTwoBadKarmaBuffer, 1f);
            listing.DrawDescription(neutralViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(20)));

            string badViewersText = "TwitchToolkitBadViewers".Translate();

            string badViewersKarmaText = "TKUtils.Karma.Description".LocalizeKeyed(
                badViewersText.ToLowerInvariant(),
                badViewersText.ToLowerInvariant().CapitalizeFirst(),
                (ToolkitSettings.KarmaCap * 0.07).ToString("N0"),
                (ToolkitSettings.KarmaCap * 0.36).ToString("N0")
            );

            listing.GroupHeader(badViewersText);
            (Rect tThreeGoodLabel, Rect tThreeGoodField) = listing.Split(0.85f);
            UiHelper.Label(tThreeGoodLabel, "TKUtils.GoodKarma.Label".Localize());
            _tThreeGoodKarmaBuffer ??= ToolkitSettings.TierThreeGoodBonus.ToString();
            Widgets.TextFieldNumeric(tThreeGoodField, ref ToolkitSettings.TierThreeGoodBonus, ref _tThreeGoodKarmaBuffer, 1f);
            listing.DrawDescription(badViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(10)));

            (Rect tThreeNeutralLabel, Rect tThreeNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tThreeNeutralLabel, "TKUtils.NeutralKarma.Label".Localize());
            _tThreeNeutralKarmaBuffer ??= ToolkitSettings.TierThreeNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tThreeNeutralField, ref ToolkitSettings.TierThreeNeutralBonus, ref _tThreeNeutralKarmaBuffer, 1f);
            listing.DrawDescription(badViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(24)));

            (Rect tThreeBadLabel, Rect tThreeBadField) = listing.Split(0.85f);
            UiHelper.Label(tThreeBadLabel, "TKUtils.BadKarma.Label".Localize());
            _tThreeBadKarmaBuffer ??= ToolkitSettings.TierThreeBadBonus.ToString();
            Widgets.TextFieldNumeric(tThreeBadField, ref ToolkitSettings.TierThreeBadBonus, ref _tThreeBadKarmaBuffer, 1f);
            listing.DrawDescription(badViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(18)));

            string doomViewersText = "TwitchToolkitDoomViewers".Translate();

            string doomViewersKarmaText = "TKUtils.Karma.Description".LocalizeKeyed(
                doomViewersText.ToLowerInvariant(),
                doomViewersText.ToLowerInvariant().CapitalizeFirst(),
                "0",
                (ToolkitSettings.KarmaCap * 0.06).ToString("N0")
            );

            listing.GroupHeader(doomViewersText);
            (Rect tFourGoodLabel, Rect tFourGoodField) = listing.Split(0.85f);
            UiHelper.Label(tFourGoodLabel, "TKUtils.GoodKarma.Label".Localize());
            _tFourGoodKarmaBuffer ??= ToolkitSettings.TierFourGoodBonus.ToString();
            Widgets.TextFieldNumeric(tFourGoodField, ref ToolkitSettings.TierFourGoodBonus, ref _tFourGoodKarmaBuffer, 1f);
            listing.DrawDescription(doomViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(6)));

            (Rect tFourNeutralLabel, Rect tFourNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tFourNeutralLabel, "TKUtils.NeutralKarma.Label".Localize());
            _tFourNeutralKarmaBuffer ??= ToolkitSettings.TierFourNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tFourNeutralField, ref ToolkitSettings.TierFourNeutralBonus, ref _tFourNeutralKarmaBuffer, 1f);
            listing.DrawDescription(doomViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(18)));

            (Rect tFourBadLabel, Rect tFourBadField) = listing.Split(0.85f);
            UiHelper.Label(tFourBadLabel, "TKUtils.BadKarma.Label".Localize());
            _tFourBadKarmaBuffer ??= ToolkitSettings.TierFourBadBonus.ToString();
            Widgets.TextFieldNumeric(tFourBadField, ref ToolkitSettings.TierFourBadBonus, ref _tFourBadKarmaBuffer, 1f);
            listing.DrawDescription(doomViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(12)));

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawPatchesSettings(Rect region)
        {
            List<ToolkitExtension> extensions = Settings_ToolkitExtensions.GetExtensions;

            string settingsText = "TKUtils.Buttons.Settings".Localize();
            var viewRect = new Rect(0f, 0f, region.width - 16f, LineHeight * extensions.Count);
            Widgets.BeginScrollView(region, ref _patchesScrollPos, viewRect);
            var listing = new Listing_Standard();

            listing.Begin(viewRect);

            foreach (ToolkitExtension ext in Settings_ToolkitExtensions.GetExtensions)
            {
                (Rect label, Rect field) = listing.Split(0.85f);
                UiHelper.Label(label, ext.mod.SettingsCategory());

                if (!Widgets.ButtonText(field, settingsText))
                {
                    continue;
                }

                SettingsWindow window = null;

                try
                {
                    window = Activator.CreateInstance(ext.windowType, ext.mod) as SettingsWindow;
                }
                catch (Exception e)
                {
                    TkUtils.Logger.Error($"Could not open settings window for {ext.mod.SettingsCategory()}'s storyteller", e);
                }

                if (window != null)
                {
                    Find.WindowStack.Add(window);
                }
            }

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawStoreSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect listLabel, Rect listField) = listing.Split(0.85f);
            UiHelper.Label(listLabel, "TKUtils.PurchaseList.Label".Localize());
            ToolkitSettings.CustomPricingSheetLink = Widgets.TextField(listField, ToolkitSettings.CustomPricingSheetLink);
            listing.DrawDescription("TKUtils.PurchaseList.Description".LocalizeKeyed(CommandDefOf.PurchaseList.command));

            if (UiHelper.FieldButton(listField, '?'))
            {
                Application.OpenURL("https://sirrandoo.github.io/toolkit-utils/itemlist");
            }

            listing.Gap();
            listing.GapLine();

            string openText = "TKUtils.Buttons.Open".Translate();
            (Rect itemsEditLabel, Rect itemsEditField) = listing.Split(0.85f);
            UiHelper.Label(itemsEditLabel, "Items Edit");

            if (Widgets.ButtonText(itemsEditField, openText))
            {
                Find.WindowStack.Add(new StoreDialog());
            }

            (Rect eventsEditLabel, Rect eventsEditField) = listing.Split(0.85f);
            UiHelper.Label(eventsEditLabel, "Events Edit");

            if (Widgets.ButtonText(eventsEditField, openText))
            {
                Find.WindowStack.Add(new StoreIncidentsWindow());
            }

            (Rect traitsEditLabel, Rect traitsEditField) = listing.Split(0.85f);
            UiHelper.Label(traitsEditLabel, $"[ToolkitUtils] {"Traits".TranslateSimple()}");

            if (Widgets.ButtonText(traitsEditField, openText))
            {
                Find.WindowStack.Add(new TraitConfigDialog());
            }

            (Rect kindsEditLabel, Rect kindsEditField) = listing.Split(0.85f);
            UiHelper.Label(kindsEditLabel, $"[ToolkitUtils] {"Race".TranslateSimple().Pluralize()}");

            if (Widgets.ButtonText(kindsEditField, openText))
            {
                Find.WindowStack.Add(new PawnKindConfigDialog());
            }

            (Rect editorLabel, Rect editorField) = listing.Split(0.85f);
            UiHelper.Label(editorLabel, $"[ToolkitUtils] {"TKUtils.Editor.Title".TranslateSimple()}");

            if (Widgets.ButtonText(editorField, openText))
            {
                Find.WindowStack.Add(new Editor());
            }

            listing.Gap();
            listing.GapLine();

            listing.CheckboxLabeled("TKUtils.PurchaseConfirmations.Label".Localize(), ref ToolkitSettings.PurchaseConfirmations);
            listing.DrawDescription("TKUtils.PurchaseConfirmations.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));
            listing.CheckboxLabeled("TKUtils.RaidersAreViewers.Label".Localize(), ref ToolkitSettings.RepeatViewerNames);
            listing.DrawDescription("TKUtils.RaidersAreViewers.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize())));
            listing.CheckboxLabeled("TKUtils.IncludeMinifiables.Label".Localize(), ref ToolkitSettings.MinifiableBuildings);
            listing.DrawDescription("TKUtils.IncludeMinifiables.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize())));

            listing.End();
        }

        private static void DrawStorytellerSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect voteTimeLabel, Rect voteTimeField) = listing.Split(0.85f);
            UiHelper.Label(voteTimeLabel, "TKUtils.VoteTime.Label".Localize());
            _voteTimeBuffer ??= ToolkitSettings.VoteTime.ToString();
            Widgets.TextFieldNumeric(voteTimeField, ref ToolkitSettings.VoteTime, ref _voteTimeBuffer, 1f, 15f);
            listing.DrawDescription("TKUtils.VoteTime.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(2)));

            (Rect voteOptionsLabel, Rect voteOptionsField) = listing.Split(0.85f);
            UiHelper.Label(voteOptionsLabel, "TKUtils.MaximumOptions.Label".Localize());
            _voteOptionsBuffer ??= ToolkitSettings.VoteOptions.ToString();
            Widgets.TextFieldNumeric(voteOptionsField, ref ToolkitSettings.VoteOptions, ref _voteOptionsBuffer, 2f, 5f);
            listing.DrawDescription("TKUtils.MaximumOptions.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(3)));

            listing.CheckboxLabeled("TKUtils.OptionsToChat.Label".Localize(), ref ToolkitSettings.VotingChatMsgs);
            listing.DrawDescription("TKUtils.OptionsToChat.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize())));
            listing.CheckboxLabeled("TKUtils.ShowVoteWindow.Label".Localize(), ref ToolkitSettings.VotingWindow);
            listing.DrawDescription("TKUtils.ShowVoteWindow.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));
            listing.CheckboxLabeled("TKUtils.EnlargeWindow.Label".Localize(), ref ToolkitSettings.LargeVotingWindow);
            listing.DrawDescription("TKUtils.EnlargeWindow.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));
            listing.Gap();
            listing.Gap();

            (Rect editPacksLabel, Rect editPacksField) = listing.Split(0.85f);
            UiHelper.Label(editPacksLabel, "TKUtils.EditStorytellerPacks.Label".Localize());

            if (!Widgets.ButtonText(editPacksField, "TKUtils.Buttons.EditStorytellerPacks".Localize()))
            {
                listing.End();

                return;
            }

            Find.WindowStack.Add(new Window_StorytellerPacks());
            listing.End();
        }

        private static void DrawViewerSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var innerRegion = new Rect(region.x, region.y, region.width, region.height - LineHeight);
            var viewPort = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * 41f);

            Widgets.BeginScrollView(innerRegion, ref _viewerScrollPos, viewPort);
            listing.Begin(viewPort);

            listing.CheckboxLabeled("TKUtils.NameQueue.Label".Localize(), ref ToolkitSettings.EnableViewerQueue);

            listing.DrawDescription(
                "TKUtils.NameQueue.Description".LocalizeKeyed(CommandDefOf.JoinQueue.command).AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize()))
            );

            listing.CheckboxLabeled("TwitchToolkitViewerColonistQueue".Translate(), ref ToolkitSettings.ViewerNamedColonistQueue);
            listing.DrawDescription("TKUtils.UnnamedNotification.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("On".Localize())));
            listing.CheckboxLabeled("TKUtils.EnableQueueCost.Label".Localize(), ref ToolkitSettings.ChargeViewersForQueue);

            listing.DrawDescription(
                "TKUtils.EnableQueueCost.Description".LocalizeKeyed(ToolkitSettings.CostToJoinQueue)
                   .AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed("Off".Localize()))
            );

            if (ToolkitSettings.ChargeViewersForQueue)
            {
                (Rect chargeLabel, Rect chargeField) = listing.Split(0.85f);
                UiHelper.Label(chargeLabel, "TKUtils.QueueCost.Label".Localize());
                _queueCostBuffer ??= ToolkitSettings.CostToJoinQueue.ToString();
                Widgets.TextFieldNumeric(chargeField, ref ToolkitSettings.CostToJoinQueue, ref _queueCostBuffer);
                listing.DrawDescription("TKUtils.QueueCost.Description".Localize().AppendWithSpace("TKUtils.Fields.DefaultValue".LocalizeKeyed(0)));
            }

            listing.GroupHeader("TKUtils.SpecialViewer".LocalizeKeyed("TKUtils.SpecialViewer.Subscriber".Localize().ColorTagged(ColorLibrary.Pink)));
            (Rect subCoinLabel, Rect subCoinField) = listing.Split(0.85f);
            UiHelper.Label(subCoinLabel, "TKUtils.ExtraCoins.Label".Localize());
            _subCoinBuffer ??= ToolkitSettings.SubscriberExtraCoins.ToString();
            Widgets.TextFieldNumeric(subCoinField, ref ToolkitSettings.SubscriberExtraCoins, ref _subCoinBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraCoins.Description".Localize());

            (Rect subMultLabel, Rect subMultField) = listing.Split(0.85f);
            UiHelper.Label(subMultLabel, "TKUtils.CoinMultiplier.Label".Localize());
            _subMultBuffer ??= ToolkitSettings.SubscriberCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(subMultField, ref ToolkitSettings.SubscriberCoinMultiplier, ref _subMultBuffer, 1f, 5f);
            listing.DrawDescription("TKUtils.CoinMultiplier.Description".Localize());

            (Rect subVotesLabel, Rect subVotesField) = listing.Split(0.85f);
            UiHelper.Label(subVotesLabel, "TKUtils.ExtraVotes.Label".Localize());
            _subVotesBuffer ??= ToolkitSettings.SubscriberExtraVotes.ToString();
            Widgets.TextFieldNumeric(subVotesField, ref ToolkitSettings.SubscriberExtraVotes, ref _subVotesBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraVotes.Description".Localize());

            listing.GroupHeader("TKUtils.SpecialViewer".LocalizeKeyed("TKUtils.SpecialViewer.Vip".Localize().ColorTagged(ColorLibrary.Lavender)));
            (Rect vipCoinLabel, Rect vipCoinField) = listing.Split(0.85f);
            UiHelper.Label(vipCoinLabel, "TKUtils.ExtraCoins.Label".Localize());
            _vipCoinBuffer ??= ToolkitSettings.VIPExtraCoins.ToString();
            Widgets.TextFieldNumeric(vipCoinField, ref ToolkitSettings.VIPExtraCoins, ref _vipCoinBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraCoins.Description".Localize());

            (Rect vipMultLabel, Rect vipMultField) = listing.Split(0.85f);
            UiHelper.Label(vipMultLabel, "TKUtils.CoinMultiplier.Label".Localize());
            _vipMultBuffer ??= ToolkitSettings.VIPCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(vipMultField, ref ToolkitSettings.VIPCoinMultiplier, ref _vipMultBuffer, 1f, 5f);
            listing.DrawDescription("TKUtils.CoinMultiplier.Description".Localize());

            (Rect vipVotesLabel, Rect vipVotesField) = listing.Split(0.85f);
            UiHelper.Label(vipVotesLabel, "TKUtils.ExtraVotes.Label".Localize());
            _vipVotesBuffer ??= ToolkitSettings.VIPExtraVotes.ToString();
            Widgets.TextFieldNumeric(vipVotesField, ref ToolkitSettings.VIPExtraVotes, ref _vipVotesBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraVotes.Description".Localize());

            listing.GroupHeader("TKUtils.SpecialViewer".LocalizeKeyed("TKUtils.SpecialViewer.Moderator".Localize().ColorTagged(ColorLibrary.PaleGreen)));
            (Rect modCoinLabel, Rect modCoinField) = listing.Split(0.85f);
            UiHelper.Label(modCoinLabel, "TKUtils.ExtraCoins.Label".Localize());
            _modCoinBuffer ??= ToolkitSettings.ModExtraCoins.ToString();
            Widgets.TextFieldNumeric(modCoinField, ref ToolkitSettings.ModExtraCoins, ref _modCoinBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraCoins.Description".Localize());

            (Rect modMultLabel, Rect modMultField) = listing.Split(0.85f);
            UiHelper.Label(modMultLabel, "TKUtils.CoinMultiplier.Label".Localize());
            _modMultBuffer ??= ToolkitSettings.ModCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(modMultField, ref ToolkitSettings.ModCoinMultiplier, ref _modMultBuffer, 1f, 5f);
            listing.DrawDescription("TKUtils.CoinMultiplier.Description".Localize());

            (Rect modVotesLabel, Rect modVotesField) = listing.Split(0.85f);
            UiHelper.Label(modVotesLabel, "TKUtils.ExtraVotes.Label".Localize());
            _modVotesBuffer ??= ToolkitSettings.ModExtraVotes.ToString();
            Widgets.TextFieldNumeric(modVotesField, ref ToolkitSettings.ModExtraVotes, ref _modVotesBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraVotes.Description".Localize());
            listing.End();
            Widgets.EndScrollView();

            GUI.BeginGroup(new Rect(region.x, region.y + region.height - LineHeight, region.width, LineHeight));
            float midpoint = Mathf.CeilToInt(region.width / 2f);
            float buttonWidth = Mathf.CeilToInt(region.width * 0.35f);
            var editViewers = new Rect(midpoint - Mathf.CeilToInt(buttonWidth / 2f), 0f, buttonWidth, LineHeight);

            if (Widgets.ButtonText(editViewers, "Edit Viewers"))
            {
                Find.WindowStack.Add(new Window_Viewers());
            }

            GUI.EndGroup();
        }

        private static void DrawSettings(Rect region)
        {
            var titleRect = new Rect(region.x, region.y, region.width, Text.SmallFontHeight);
            var gapRect = new Rect(region.x, region.y + titleRect.height, region.width, Text.SmallFontHeight);
            var tabRect = new Rect(gapRect.x, gapRect.y + gapRect.height, gapRect.width, Mathf.FloorToInt(Text.SmallFontHeight * 1.5f));
            int lineGapWidth = Mathf.FloorToInt(gapRect.width * 0.2f);

            UiHelper.Label(titleRect, Toolkit.Mod.Content.Name, new Color(1f, 0.27f, 0.92f), TextAnchor.MiddleCenter, GameFont.Medium);

            Widgets.DrawLineHorizontal(gapRect.x + lineGapWidth, gapRect.y + Mathf.FloorToInt(gapRect.height * 0.35f), gapRect.width - lineGapWidth * 2f);

            GUI.BeginGroup(tabRect);
            DrawTabButtons(tabRect.AtZero());
            GUI.EndGroup();

            DrawContent(new Rect(tabRect.x, tabRect.y + tabRect.height, region.width, region.height - tabRect.height - gapRect.height - titleRect.height));
        }

        private static void DrawContent(Rect region)
        {
            GUI.BeginGroup(region);

            Rect contentRect = region.AtZero().ContractedBy(16f);

            GUI.BeginGroup(contentRect);
            _tabWorker.SelectedTab.ContentDrawer(contentRect.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private static void DrawTabButtons(Rect region)
        {
            float buttonWidth = Mathf.FloorToInt(region.width / 8f);
            float start = region.center.x - buttonWidth * 3f - Mathf.FloorToInt(buttonWidth / 2f);
            var coinsRect = new Rect(start, 0f, buttonWidth, region.height);
            Rect cooldownsRect = coinsRect.Shift(Direction8Way.East, 0f);
            Rect karmaRect = cooldownsRect.Shift(Direction8Way.East, 0f);
            Rect patchesRect = karmaRect.Shift(Direction8Way.East, 0f);
            Rect storeRect = patchesRect.Shift(Direction8Way.East, 0f);
            Rect storytellerRect = storeRect.Shift(Direction8Way.East, 0f);
            Rect viewersRect = storytellerRect.Shift(Direction8Way.East, 0f);

            if (DrawTabButton(coinsRect, _coinTabItem.Label, _coinTabItem.Tooltip, _tabWorker.SelectedTab == _coinTabItem))
            {
                _tabWorker.SelectedTab = _coinTabItem;
            }

            if (DrawTabButton(cooldownsRect, _cooldownTabItem.Label, _cooldownTabItem.Tooltip, _tabWorker.SelectedTab == _cooldownTabItem))
            {
                _tabWorker.SelectedTab = _cooldownTabItem;
            }

            if (DrawTabButton(karmaRect, _karmaTabItem.Label, _karmaTabItem.Tooltip, _tabWorker.SelectedTab == _karmaTabItem))
            {
                _tabWorker.SelectedTab = _karmaTabItem;
            }

            if (DrawTabButton(patchesRect, _patchesTabItem.Label, _patchesTabItem.Tooltip, _tabWorker.SelectedTab == _patchesTabItem))
            {
                _tabWorker.SelectedTab = _patchesTabItem;
            }

            if (DrawTabButton(storeRect, _storeTabItem.Label, _storeTabItem.Tooltip, _tabWorker.SelectedTab == _storeTabItem))
            {
                _tabWorker.SelectedTab = _storeTabItem;
            }

            if (DrawTabButton(storytellerRect, _storytellerTabItem.Label, _storytellerTabItem.Tooltip, _tabWorker.SelectedTab == _storytellerTabItem))
            {
                _tabWorker.SelectedTab = _storytellerTabItem;
            }

            if (DrawTabButton(viewersRect, _viewerTabItem.Label, _viewerTabItem.Tooltip, _tabWorker.SelectedTab == _viewerTabItem))
            {
                _tabWorker.SelectedTab = _viewerTabItem;
            }
        }

        private static bool DrawTabButton(Rect region, string text, string tooltip, bool active = false)
        {
            if (!active)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }

            Widgets.DrawAtlas(region, Widgets.ButtonBGAtlas);
            GUI.color = Color.white;
            UiHelper.Label(region, text, TextAnchor.MiddleCenter);
            TooltipHandler.TipRegion(region, tooltip);
            bool result = Widgets.ButtonInvisible(region);

            return result;
        }
    }
}
