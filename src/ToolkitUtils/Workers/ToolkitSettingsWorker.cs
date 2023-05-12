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
using SirRandoo.CommonLib.Workers;
using ToolkitUtils.Helpers;
using ToolkitUtils.Windows;
using TwitchToolkit;
using TwitchToolkit.Settings;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Workers
{
    public static class ToolkitSettingsWorker
    {
        private static readonly float LineHeight = Mathf.FloorToInt(Text.SmallFontHeight * 1.25f);
        private static TabWorker _tabWorker;
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
            _tabWorker.AddTab("coins", "TwitchToolkitCoins".TranslateSimple(), "TKUtils.Coins.Tooltip".TranslateSimple(), DrawCoinSettings);
            _tabWorker.AddTab("cooldowns", "TwitchToolkitCooldowns".TranslateSimple(), "TKUtils.Cooldowns.Tooltip".TranslateSimple(), DrawCooldownSettings);
            _tabWorker.AddTab("karma", "TwitchToolkitKarma".TranslateSimple(), "TKUtils.Karma.Tooltip".TranslateSimple(), DrawKarmaSettings);
            _tabWorker.AddTab("addons", "TKUtils.Addon.Label".TranslateSimple(), "TKUtils.Karma.Tooltip".TranslateSimple(), DrawKarmaSettings);
            _tabWorker.AddTab("store", "TwitchToolkitStore".TranslateSimple(), "TKUtils.Store.Tooltip".TranslateSimple(), DrawStoreSettings);
            _tabWorker.AddTab("storyteller", "TKUtils.Storyteller".TranslateSimple(), "TKUtils.Storyteller.Tooltip".TranslateSimple(), DrawStorytellerSettings);
            _tabWorker.AddTab("viewers", "TwitchToolkitViewers".TranslateSimple(), "TKUtils.Viewers.Tooltip".TranslateSimple(), DrawViewerSettings);
        }

        private static void DrawCoinSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var viewRect = new Rect(0f, 0f, region.width - 16f, Text.SmallFontHeight * 32f);
            Widgets.BeginScrollView(region, ref _coinsScrollPos, viewRect);

            listing.Begin(viewRect);

            listing.CheckboxLabeled(
                ToolkitSettings.CoinInterval > 1
                    ? (string)"TKUtils.EarningCoins.Label".Translate(ToolkitSettings.CoinInterval)
                    : "TKUtils.EarningCoins.Singular.Label".TranslateSimple(),
                ref ToolkitSettings.EarningCoins
            );

            listing.DrawDescription(
                ToolkitSettings.CoinInterval > 1
                    ? (string)"TKUtils.EarningCoins.Description".Translate(ToolkitSettings.CoinInterval)
                    : "TKUtils.EarningCoins.Singular.Label".TranslateSimple()
            );

            if (ToolkitSettings.EarningCoins)
            {
                (Rect startBalLabel, Rect startBalField) = listing.Split(0.85f);
                UiHelper.Label(startBalLabel, "TKUtils.StartingBalance.Label".TranslateSimple());
                _startingBalanceBuffer ??= ToolkitSettings.StartingBalance.ToString();
                Widgets.TextFieldNumeric(startBalField, ref ToolkitSettings.StartingBalance, ref _startingBalanceBuffer);
                listing.DrawDescription("TKUtils.StartingBalance.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(150)));

                (Rect coinIntLabel, Rect coinIntField) = listing.Split(0.85f);
                UiHelper.Label(coinIntLabel, "TKUtils.CoinInterval.Label".Translate());
                _coinIntervalBuffer ??= ToolkitSettings.CoinInterval.ToString();
                Widgets.TextFieldNumeric(coinIntField, ref ToolkitSettings.CoinInterval, ref _coinIntervalBuffer);
                listing.DrawDescription("TKUtils.CoinInterval.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(2)));

                (Rect coinAmountLabel, Rect coinAmountField) = listing.Split(0.85f);
                UiHelper.Label(coinAmountLabel, "TKUtils.CoinAmount.Label".TranslateSimple());
                _coinAmountBuffer ??= ToolkitSettings.CoinAmount.ToString();
                Widgets.TextFieldNumeric(coinAmountField, ref ToolkitSettings.CoinAmount, ref _coinAmountBuffer);

                listing.DrawDescription(
                    (ToolkitSettings.CoinInterval > 1
                        ? (string)"TKUtils.CoinAmount.Description".Translate(ToolkitSettings.CoinInterval.ToString("N0"))
                        : "TKUtils.CoinAmount.Singular.Description".TranslateSimple()).AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(30))
                );
            }

            (Rect minPurLabel, Rect minPurField) = listing.Split(0.85f);
            UiHelper.Label(minPurLabel, "TKUtils.MinimumPurchaseAmount.Label".TranslateSimple());
            _minimumPurchaseBuffer ??= ToolkitSettings.MinimumPurchasePrice.ToString();
            Widgets.TextFieldNumeric(minPurField, ref ToolkitSettings.MinimumPurchasePrice, ref _minimumPurchaseBuffer);
            listing.DrawDescription("TKUtils.MinimumPurchaseAmount.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(60)));

            listing.Gap();
            listing.CheckboxLabeled("TKUtils.UnlimitedCoins.Label".TranslateSimple(), ref ToolkitSettings.UnlimitedCoins);

            listing.DrawDescription(
                "TKUtils.UnlimitedCoins.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple()))
            );

            listing.GapLine();
            listing.Gap();

            listing.CheckboxLabeled("TKUtils.RequireParticipation.Label".TranslateSimple(), ref ToolkitSettings.ChatReqsForCoins);

            listing.DrawDescription(
                "TKUtils.RequireParticipation.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple()))
            );

            if (!ToolkitSettings.ChatReqsForCoins)
            {
                listing.End();
                Widgets.EndScrollView();

                return;
            }

            (Rect halfCoinsLabel, Rect halfCoinsField) = listing.Split(0.85f);
            UiHelper.Label(halfCoinsLabel, "TKUtils.HalfCoins.Label".TranslateSimple());
            _halfCoinsBuffer ??= ToolkitSettings.TimeBeforeHalfCoins.ToString();
            Widgets.TextFieldNumeric(halfCoinsField, ref ToolkitSettings.TimeBeforeHalfCoins, ref _halfCoinsBuffer);
            listing.DrawDescription("TKUtils.HalfCoins.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(30)));

            (Rect noCoinsLabel, Rect noCoinsField) = listing.Split(0.85f);
            UiHelper.Label(noCoinsLabel, "TKUtils.NoCoins.Label".Translate());
            _noCoinsBuffer ??= ToolkitSettings.TimeBeforeNoCoins.ToString();
            Widgets.TextFieldNumeric(noCoinsField, ref ToolkitSettings.TimeBeforeNoCoins, ref _noCoinsBuffer);
            listing.DrawDescription("TKUtils.NoCoins.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(60)));

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawCooldownSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect cooldownLabel, Rect cooldownField) = listing.Split(0.85f);
            UiHelper.Label(cooldownLabel, "TKUtils.CooldownPeriod.Label".TranslateSimple());
            _eventCooldownBuffer ??= ToolkitSettings.EventCooldownInterval.ToString();
            Widgets.TextFieldNumeric(cooldownField, ref ToolkitSettings.EventCooldownInterval, ref _eventCooldownBuffer, 1f, 15f);

            listing.DrawDescription(
                (ToolkitSettings.EventCooldownInterval > 1
                    ? (string)"TKUtils.CooldownPeriod.Description".Translate(ToolkitSettings.EventCooldownInterval)
                    : "TKUtils.CooldownPeriod.Singular.Description".TranslateSimple()).AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(15))
            );

            listing.Gap();

            listing.CheckboxLabeled("TKUtils.MaxEvents.Label".TranslateSimple(), ref ToolkitSettings.MaxEvents);
            listing.DrawDescription("TKUtils.MaxEvents.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple())));
            listing.Gap();

            if (ToolkitSettings.MaxEvents)
            {
                (Rect badEventsLabel, Rect badEventsField) = listing.Split(0.85f);
                UiHelper.Label(badEventsLabel, "TKUtils.MaxBadEvents.Label".TranslateSimple());
                _maxBadEventsBuffer ??= ToolkitSettings.MaxBadEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(badEventsField, ref ToolkitSettings.MaxBadEventsPerInterval, ref _maxBadEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".Translate(3));

                (Rect goodEventsLabel, Rect goodEventsField) = listing.Split(0.85f);
                UiHelper.Label(goodEventsLabel, "TKUtils.MaxGoodEvents.Label".TranslateSimple());
                _maxGoodEventsBuffer ??= ToolkitSettings.MaxGoodEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(goodEventsField, ref ToolkitSettings.MaxGoodEventsPerInterval, ref _maxGoodEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".Translate(10));

                (Rect neutralEventsLabel, Rect neutralEventsField) = listing.Split(0.85f);
                UiHelper.Label(neutralEventsLabel, "TKUtils.MaxNeutralEvents.Label".TranslateSimple());
                _maxNeutralEventsBuffer ??= ToolkitSettings.MaxNeutralEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(neutralEventsField, ref ToolkitSettings.MaxNeutralEventsPerInterval, ref _maxNeutralEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".Translate(10));

                (Rect itemEventsLabel, Rect itemEventsField) = listing.Split(0.85f);
                UiHelper.Label(itemEventsLabel, "TKUtils.MaxItemEvents.Label".TranslateSimple());
                _maxItemEventsBuffer ??= ToolkitSettings.MaxCarePackagesPerInterval.ToString();
                Widgets.TextFieldNumeric(itemEventsField, ref ToolkitSettings.MaxCarePackagesPerInterval, ref _maxItemEventsBuffer);
                listing.DrawDescription("TKUtils.Fields.DefaultValue".Translate(10));

                listing.Gap();
            }

            listing.CheckboxLabeled("TKUtils.EventCooldowns.Label".TranslateSimple(), ref ToolkitSettings.EventsHaveCooldowns);

            listing.DrawDescription(
                "TKUtils.EventCooldowns.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple()))
            );

            listing.End();
        }

        private static void DrawKarmaSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, region.width - 16f, Text.SmallFontHeight * 72f);

            Widgets.BeginScrollView(region, ref _karmaScrollPos, viewPort);
            listing.Begin(viewPort);

            (Rect startKarmaLabel, Rect startKarmaField) = listing.Split(0.85f);
            UiHelper.Label(startKarmaLabel, "TKUtils.StartingKarma.Label".TranslateSimple());
            _startKarmaBuffer ??= ToolkitSettings.StartingKarma.ToString();
            Widgets.TextFieldNumeric(startKarmaField, ref ToolkitSettings.StartingKarma, ref _startKarmaBuffer, 50f, 250f);
            listing.DrawDescription("TKUtils.StartingKarma.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(100)));

            (Rect karmaCapLabel, Rect karmaCapField) = listing.Split(0.85f);
            UiHelper.Label(karmaCapLabel, "TKUtils.KarmaCap.Label".TranslateSimple());
            _karmaCapBuffer ??= ToolkitSettings.KarmaCap.ToString();
            Widgets.TextFieldNumeric(karmaCapField, ref ToolkitSettings.KarmaCap, ref _karmaCapBuffer);
            listing.DrawDescription("TKUtils.KarmaCap.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(140)));

            listing.CheckboxLabeled("TKUtils.NegativeKarma.Label".TranslateSimple(), ref ToolkitSettings.BanViewersWhoPurchaseAlwaysBad);
            listing.DrawDescription("TKUtils.NegativeKarma.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple())));
            listing.Gap();

            (Rect karMinLabel, Rect karMinField) = listing.Split(0.85f);
            UiHelper.Label(karMinLabel, "TKUtils.KarmaMinimum.Label".TranslateSimple());
            _minKarmaBuffer ??= ToolkitSettings.KarmaMinimum.ToString();
            Widgets.TextFieldNumeric(karMinField, ref ToolkitSettings.KarmaMinimum, ref _minKarmaBuffer, -200000, 100f);
            listing.DrawDescription("TKUtils.KarmaMinimum.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(10)));
            listing.Gap();

            listing.CheckboxLabeled("TKUtils.GiftingRequiresKarma.Label".TranslateSimple(), ref ToolkitSettings.KarmaReqsForGifting);

            listing.DrawDescription(
                "TKUtils.GiftingRequiresKarma.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple()))
            );

            listing.Gap();

            if (ToolkitSettings.KarmaReqsForGifting)
            {
                (Rect minKarmaGiftLabel, Rect minKarmaGiftField) = listing.Split(0.85f);
                UiHelper.Label(minKarmaGiftLabel, "TKUtils.KarmaForReceiving.Label".TranslateSimple());
                _minGiftKarmaBuffer ??= ToolkitSettings.MinimumKarmaToRecieveGifts.ToString();
                Widgets.TextFieldNumeric(minKarmaGiftField, ref ToolkitSettings.MinimumKarmaToRecieveGifts, ref _minGiftKarmaBuffer, 10f);
                listing.DrawDescription("TKUtils.KarmaForReceiving.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(35)));

                (Rect minKarmaGiftingLabel, Rect minKarmaGiftingField) = listing.Split(0.85f);
                UiHelper.Label(minKarmaGiftingLabel, "TKUtils.KarmaForGifting.Label".TranslateSimple());
                _minGiftingKarmaBuffer ??= ToolkitSettings.MinimumKarmaToSendGifts.ToString();
                Widgets.TextFieldNumeric(minKarmaGiftingField, ref ToolkitSettings.MinimumKarmaToSendGifts, ref _minGiftingKarmaBuffer, 20f, 150f);
                listing.DrawDescription("TKUtils.KarmaForGifting.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(100)));
                listing.Gap();
            }

            string goodViewersText = "TwitchToolkitGoodViewers".Translate();

            string goodViewersKarmaText = "TKUtils.Karma.Description".Translate(
                goodViewersText.ToLowerInvariant(),
                goodViewersText.ToLowerInvariant().CapitalizeFirst(),
                (ToolkitSettings.KarmaCap * 0.56).ToString("N0"),
                ToolkitSettings.KarmaCap.ToString("N0")
            );

            listing.GroupHeader(goodViewersText);
            (Rect tOneGoodLabel, Rect tOneGoodField) = listing.Split(0.85f);
            UiHelper.Label(tOneGoodLabel, "TKUtils.GoodKarma.Label".TranslateSimple());
            _tOneGoodKarmaBuffer ??= ToolkitSettings.TierOneGoodBonus.ToString();
            Widgets.TextFieldNumeric(tOneGoodField, ref ToolkitSettings.TierOneGoodBonus, ref _tOneGoodKarmaBuffer, 1f);
            listing.DrawDescription(goodViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(16)));

            (Rect tOneNeutralLabel, Rect tOneNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tOneNeutralLabel, "TKUtils.NeutralKarma.Label".TranslateSimple());
            _tOneNeutralKarmaBuffer ??= ToolkitSettings.TierOneNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tOneNeutralField, ref ToolkitSettings.TierOneNeutralBonus, ref _tOneNeutralKarmaBuffer, 1f);
            listing.DrawDescription(goodViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(36)));

            (Rect tOneBadLabel, Rect tOneBadField) = listing.Split(0.85f);
            UiHelper.Label(tOneBadLabel, "TKUtils.BadKarma.Label".TranslateSimple());
            _tOneBadKarmaBuffer ??= ToolkitSettings.TierOneBadBonus.ToString();
            Widgets.TextFieldNumeric(tOneBadField, ref ToolkitSettings.TierOneBadBonus, ref _tOneBadKarmaBuffer, 1f);
            listing.DrawDescription(goodViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(24)));

            string neutralViewersText = "TwitchToolkitNeutralViewers".Translate();

            string neutralViewersKarmaText = "TKUtils.Karma.Description".Translate(
                neutralViewersText.ToLowerInvariant(),
                neutralViewersText.ToLowerInvariant().CapitalizeFirst(),
                (ToolkitSettings.KarmaCap * 0.37).ToString("N0"),
                (ToolkitSettings.KarmaCap * 0.55).ToString("N0")
            );

            listing.GroupHeader(neutralViewersText);
            (Rect tTwoGoodLabel, Rect tTwoGoodField) = listing.Split(0.85f);
            UiHelper.Label(tTwoGoodLabel, "TKUtils.GoodKarma.Label".TranslateSimple());
            _tTwoGoodKarmaBuffer ??= ToolkitSettings.TierTwoGoodBonus.ToString();
            Widgets.TextFieldNumeric(tTwoGoodField, ref ToolkitSettings.TierTwoGoodBonus, ref _tTwoGoodKarmaBuffer, 1f);
            listing.DrawDescription(neutralViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(10)));

            (Rect tTwoNeutralLabel, Rect tTwoNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tTwoNeutralLabel, "TKUtils.NeutralKarma.Label".TranslateSimple());
            _tTwoNeutralKarmaBuffer ??= ToolkitSettings.TierTwoNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tTwoNeutralField, ref ToolkitSettings.TierTwoNeutralBonus, ref _tTwoNeutralKarmaBuffer, 1f);
            listing.DrawDescription(neutralViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(30)));

            (Rect tTwoBadLabel, Rect tTwoBadField) = listing.Split(0.85f);
            UiHelper.Label(tTwoBadLabel, "TKUtils.BadKarma.Label".TranslateSimple());
            _tTwoBadKarmaBuffer ??= ToolkitSettings.TierTwoBadBonus.ToString();
            Widgets.TextFieldNumeric(tTwoBadField, ref ToolkitSettings.TierTwoBadBonus, ref _tTwoBadKarmaBuffer, 1f);
            listing.DrawDescription(neutralViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(20)));

            string badViewersText = "TwitchToolkitBadViewers".Translate();

            string badViewersKarmaText = "TKUtils.Karma.Description".Translate(
                badViewersText.ToLowerInvariant(),
                badViewersText.ToLowerInvariant().CapitalizeFirst(),
                (ToolkitSettings.KarmaCap * 0.07).ToString("N0"),
                (ToolkitSettings.KarmaCap * 0.36).ToString("N0")
            );

            listing.GroupHeader(badViewersText);
            (Rect tThreeGoodLabel, Rect tThreeGoodField) = listing.Split(0.85f);
            UiHelper.Label(tThreeGoodLabel, "TKUtils.GoodKarma.Label".TranslateSimple());
            _tThreeGoodKarmaBuffer ??= ToolkitSettings.TierThreeGoodBonus.ToString();
            Widgets.TextFieldNumeric(tThreeGoodField, ref ToolkitSettings.TierThreeGoodBonus, ref _tThreeGoodKarmaBuffer, 1f);
            listing.DrawDescription(badViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(10)));

            (Rect tThreeNeutralLabel, Rect tThreeNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tThreeNeutralLabel, "TKUtils.NeutralKarma.Label".TranslateSimple());
            _tThreeNeutralKarmaBuffer ??= ToolkitSettings.TierThreeNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tThreeNeutralField, ref ToolkitSettings.TierThreeNeutralBonus, ref _tThreeNeutralKarmaBuffer, 1f);
            listing.DrawDescription(badViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(24)));

            (Rect tThreeBadLabel, Rect tThreeBadField) = listing.Split(0.85f);
            UiHelper.Label(tThreeBadLabel, "TKUtils.BadKarma.Label".TranslateSimple());
            _tThreeBadKarmaBuffer ??= ToolkitSettings.TierThreeBadBonus.ToString();
            Widgets.TextFieldNumeric(tThreeBadField, ref ToolkitSettings.TierThreeBadBonus, ref _tThreeBadKarmaBuffer, 1f);
            listing.DrawDescription(badViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(18)));

            string doomViewersText = "TwitchToolkitDoomViewers".Translate();

            string doomViewersKarmaText = "TKUtils.Karma.Description".Translate(
                doomViewersText.ToLowerInvariant(),
                doomViewersText.ToLowerInvariant().CapitalizeFirst(),
                "0",
                (ToolkitSettings.KarmaCap * 0.06).ToString("N0")
            );

            listing.GroupHeader(doomViewersText);
            (Rect tFourGoodLabel, Rect tFourGoodField) = listing.Split(0.85f);
            UiHelper.Label(tFourGoodLabel, "TKUtils.GoodKarma.Label".TranslateSimple());
            _tFourGoodKarmaBuffer ??= ToolkitSettings.TierFourGoodBonus.ToString();
            Widgets.TextFieldNumeric(tFourGoodField, ref ToolkitSettings.TierFourGoodBonus, ref _tFourGoodKarmaBuffer, 1f);
            listing.DrawDescription(doomViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(6)));

            (Rect tFourNeutralLabel, Rect tFourNeutralField) = listing.Split(0.85f);
            UiHelper.Label(tFourNeutralLabel, "TKUtils.NeutralKarma.Label".TranslateSimple());
            _tFourNeutralKarmaBuffer ??= ToolkitSettings.TierFourNeutralBonus.ToString();
            Widgets.TextFieldNumeric(tFourNeutralField, ref ToolkitSettings.TierFourNeutralBonus, ref _tFourNeutralKarmaBuffer, 1f);
            listing.DrawDescription(doomViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(18)));

            (Rect tFourBadLabel, Rect tFourBadField) = listing.Split(0.85f);
            UiHelper.Label(tFourBadLabel, "TKUtils.BadKarma.Label".TranslateSimple());
            _tFourBadKarmaBuffer ??= ToolkitSettings.TierFourBadBonus.ToString();
            Widgets.TextFieldNumeric(tFourBadField, ref ToolkitSettings.TierFourBadBonus, ref _tFourBadKarmaBuffer, 1f);
            listing.DrawDescription(doomViewersKarmaText.AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(12)));

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawPatchesSettings(Rect region)
        {
            List<ToolkitExtension> extensions = Settings_ToolkitExtensions.GetExtensions;

            string settingsText = "TKUtils.Buttons.Settings".TranslateSimple();
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

                if (ext.mod is TkUtils)
                {
                    TkUtils.Instance.DoSettingsWindowContents(region);
                    listing.End();
                    Widgets.EndScrollView();

                    return;
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
            UiHelper.Label(listLabel, "TKUtils.PurchaseList.Label".TranslateSimple());
            listing.DrawDescription("TKUtils.PurchaseList.Description".Translate(CommandDefOf.PurchaseList.command));

            Rect listHelpBtn = LayoutHelper.IconRect(listField.x + listField.width - listField.height, listField.y, listField.height, listField.height);
            listField = listField.Trim(Direction8Way.East, listHelpBtn.width + 7f);
            ToolkitSettings.CustomPricingSheetLink = Widgets.TextField(listField, ToolkitSettings.CustomPricingSheetLink);

            if (Widgets.ButtonText(listHelpBtn, "?", false))
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
                Find.WindowStack.Add(new IncidentCategoryWindow());
            }

            (Rect commandsLabel, Rect commandsField) = listing.Split(0.85f);
            UiHelper.Label(commandsLabel, "Commands Edit");

            if (Widgets.ButtonText(commandsField, openText))
            {
                Find.WindowStack.Add(new CommandCategoryWindow());
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

            listing.CheckboxLabeled("TKUtils.PurchaseConfirmations.Label".TranslateSimple(), ref ToolkitSettings.PurchaseConfirmations);

            listing.DrawDescription(
                "TKUtils.PurchaseConfirmations.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple()))
            );

            listing.CheckboxLabeled("TKUtils.RaidersAreViewers.Label".TranslateSimple(), ref ToolkitSettings.RepeatViewerNames);

            listing.DrawDescription(
                "TKUtils.RaidersAreViewers.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple()))
            );

            listing.CheckboxLabeled("TKUtils.IncludeMinifiables.Label".TranslateSimple(), ref ToolkitSettings.MinifiableBuildings);

            listing.DrawDescription(
                "TKUtils.IncludeMinifiables.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple()))
            );

            listing.End();
        }

        private static void DrawStorytellerSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect voteTimeLabel, Rect voteTimeField) = listing.Split(0.85f);
            UiHelper.Label(voteTimeLabel, "TKUtils.VoteTime.Label".TranslateSimple());
            _voteTimeBuffer ??= ToolkitSettings.VoteTime.ToString();
            Widgets.TextFieldNumeric(voteTimeField, ref ToolkitSettings.VoteTime, ref _voteTimeBuffer, 1f, 15f);
            listing.DrawDescription("TKUtils.VoteTime.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(2)));

            (Rect voteOptionsLabel, Rect voteOptionsField) = listing.Split(0.85f);
            UiHelper.Label(voteOptionsLabel, "TKUtils.MaximumOptions.Label".TranslateSimple());
            _voteOptionsBuffer ??= ToolkitSettings.VoteOptions.ToString();
            Widgets.TextFieldNumeric(voteOptionsField, ref ToolkitSettings.VoteOptions, ref _voteOptionsBuffer, 2f, 5f);
            listing.DrawDescription("TKUtils.MaximumOptions.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(3)));

            listing.CheckboxLabeled("TKUtils.OptionsToChat.Label".TranslateSimple(), ref ToolkitSettings.VotingChatMsgs);

            listing.DrawDescription(
                "TKUtils.OptionsToChat.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple()))
            );

            listing.CheckboxLabeled("TKUtils.ShowVoteWindow.Label".TranslateSimple(), ref ToolkitSettings.VotingWindow);

            listing.DrawDescription(
                "TKUtils.ShowVoteWindow.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple()))
            );

            listing.CheckboxLabeled("TKUtils.EnlargeWindow.Label".TranslateSimple(), ref ToolkitSettings.LargeVotingWindow);
            listing.DrawDescription("TKUtils.EnlargeWindow.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple())));
            listing.Gap();
            listing.Gap();

            (Rect editPacksLabel, Rect editPacksField) = listing.Split(0.85f);
            UiHelper.Label(editPacksLabel, "TKUtils.EditStorytellerPacks.Label".TranslateSimple());

            if (!Widgets.ButtonText(editPacksField, "TKUtils.Buttons.EditStorytellerPacks".TranslateSimple()))
            {
                listing.End();

                return;
            }

            Find.WindowStack.Add(new StorytellerPackDialog());
            listing.End();
        }

        private static void DrawViewerSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var innerRegion = new Rect(region.x, region.y, region.width, region.height - LineHeight);
            var viewPort = new Rect(0f, 0f, region.width - 16f, Text.LineHeight * 41f);

            Widgets.BeginScrollView(innerRegion, ref _viewerScrollPos, viewPort);
            listing.Begin(viewPort);

            listing.CheckboxLabeled("TKUtils.NameQueue.Label".TranslateSimple(), ref ToolkitSettings.EnableViewerQueue);

            listing.DrawDescription(
                ((string)"TKUtils.NameQueue.Description".Translate(CommandDefOf.JoinQueue.command)).AppendWithSpace(
                    "TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple())
                )
            );

            listing.CheckboxLabeled("TwitchToolkitViewerColonistQueue".Translate(), ref ToolkitSettings.ViewerNamedColonistQueue);

            listing.DrawDescription(
                "TKUtils.UnnamedNotification.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate("On".TranslateSimple()))
            );

            listing.CheckboxLabeled("TKUtils.EnableQueueCost.Label".TranslateSimple(), ref ToolkitSettings.ChargeViewersForQueue);

            listing.DrawDescription(
                ((string)"TKUtils.EnableQueueCost.Description".Translate(ToolkitSettings.CostToJoinQueue)).AppendWithSpace(
                    "TKUtils.Fields.DefaultValue".Translate("Off".TranslateSimple())
                )
            );

            if (ToolkitSettings.ChargeViewersForQueue)
            {
                (Rect chargeLabel, Rect chargeField) = listing.Split(0.85f);
                UiHelper.Label(chargeLabel, "TKUtils.QueueCost.Label".TranslateSimple());
                _queueCostBuffer ??= ToolkitSettings.CostToJoinQueue.ToString();
                Widgets.TextFieldNumeric(chargeField, ref ToolkitSettings.CostToJoinQueue, ref _queueCostBuffer);
                listing.DrawDescription("TKUtils.QueueCost.Description".TranslateSimple().AppendWithSpace("TKUtils.Fields.DefaultValue".Translate(0)));
            }

            listing.GroupHeader("TKUtils.SpecialViewer".Translate("TKUtils.SpecialViewer.Subscriber".TranslateSimple().ColorTagged(ColorLibrary.Pink)));
            (Rect subCoinLabel, Rect subCoinField) = listing.Split(0.85f);
            UiHelper.Label(subCoinLabel, "TKUtils.ExtraCoins.Label".TranslateSimple());
            _subCoinBuffer ??= ToolkitSettings.SubscriberExtraCoins.ToString();
            Widgets.TextFieldNumeric(subCoinField, ref ToolkitSettings.SubscriberExtraCoins, ref _subCoinBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraCoins.Description".TranslateSimple());

            (Rect subMultLabel, Rect subMultField) = listing.Split(0.85f);
            UiHelper.Label(subMultLabel, "TKUtils.CoinMultiplier.Label".TranslateSimple());
            _subMultBuffer ??= ToolkitSettings.SubscriberCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(subMultField, ref ToolkitSettings.SubscriberCoinMultiplier, ref _subMultBuffer, 1f, 5f);
            listing.DrawDescription("TKUtils.CoinMultiplier.Description".TranslateSimple());

            (Rect subVotesLabel, Rect subVotesField) = listing.Split(0.85f);
            UiHelper.Label(subVotesLabel, "TKUtils.ExtraVotes.Label".TranslateSimple());
            _subVotesBuffer ??= ToolkitSettings.SubscriberExtraVotes.ToString();
            Widgets.TextFieldNumeric(subVotesField, ref ToolkitSettings.SubscriberExtraVotes, ref _subVotesBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraVotes.Description".TranslateSimple());

            listing.GroupHeader("TKUtils.SpecialViewer".Translate("TKUtils.SpecialViewer.Vip".TranslateSimple().ColorTagged(ColorLibrary.Lavender)));
            (Rect vipCoinLabel, Rect vipCoinField) = listing.Split(0.85f);
            UiHelper.Label(vipCoinLabel, "TKUtils.ExtraCoins.Label".TranslateSimple());
            _vipCoinBuffer ??= ToolkitSettings.VIPExtraCoins.ToString();
            Widgets.TextFieldNumeric(vipCoinField, ref ToolkitSettings.VIPExtraCoins, ref _vipCoinBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraCoins.Description".TranslateSimple());

            (Rect vipMultLabel, Rect vipMultField) = listing.Split(0.85f);
            UiHelper.Label(vipMultLabel, "TKUtils.CoinMultiplier.Label".TranslateSimple());
            _vipMultBuffer ??= ToolkitSettings.VIPCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(vipMultField, ref ToolkitSettings.VIPCoinMultiplier, ref _vipMultBuffer, 1f, 5f);
            listing.DrawDescription("TKUtils.CoinMultiplier.Description".TranslateSimple());

            (Rect vipVotesLabel, Rect vipVotesField) = listing.Split(0.85f);
            UiHelper.Label(vipVotesLabel, "TKUtils.ExtraVotes.Label".TranslateSimple());
            _vipVotesBuffer ??= ToolkitSettings.VIPExtraVotes.ToString();
            Widgets.TextFieldNumeric(vipVotesField, ref ToolkitSettings.VIPExtraVotes, ref _vipVotesBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraVotes.Description".TranslateSimple());

            listing.GroupHeader("TKUtils.SpecialViewer".Translate("TKUtils.SpecialViewer.Moderator".TranslateSimple().ColorTagged(ColorLibrary.PaleGreen)));
            (Rect modCoinLabel, Rect modCoinField) = listing.Split(0.85f);
            UiHelper.Label(modCoinLabel, "TKUtils.ExtraCoins.Label".TranslateSimple());
            _modCoinBuffer ??= ToolkitSettings.ModExtraCoins.ToString();
            Widgets.TextFieldNumeric(modCoinField, ref ToolkitSettings.ModExtraCoins, ref _modCoinBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraCoins.Description".TranslateSimple());

            (Rect modMultLabel, Rect modMultField) = listing.Split(0.85f);
            UiHelper.Label(modMultLabel, "TKUtils.CoinMultiplier.Label".TranslateSimple());
            _modMultBuffer ??= ToolkitSettings.ModCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(modMultField, ref ToolkitSettings.ModCoinMultiplier, ref _modMultBuffer, 1f, 5f);
            listing.DrawDescription("TKUtils.CoinMultiplier.Description".TranslateSimple());

            (Rect modVotesLabel, Rect modVotesField) = listing.Split(0.85f);
            UiHelper.Label(modVotesLabel, "TKUtils.ExtraVotes.Label".TranslateSimple());
            _modVotesBuffer ??= ToolkitSettings.ModExtraVotes.ToString();
            Widgets.TextFieldNumeric(modVotesField, ref ToolkitSettings.ModExtraVotes, ref _modVotesBuffer, max: 100f);
            listing.DrawDescription("TKUtils.ExtraVotes.Description".TranslateSimple());
            listing.End();
            Widgets.EndScrollView();

            GUI.BeginGroup(new Rect(region.x, region.y + region.height - LineHeight, region.width, LineHeight));
            float midpoint = Mathf.CeilToInt(region.width / 2f);
            float buttonWidth = Mathf.CeilToInt(region.width * 0.35f);
            var editViewers = new Rect(midpoint - Mathf.CeilToInt(buttonWidth / 2f), 0f, buttonWidth, LineHeight);

            if (Widgets.ButtonText(editViewers, "Edit Viewers"))
            {
                Find.WindowStack.Add(new ViewersDialog());
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
