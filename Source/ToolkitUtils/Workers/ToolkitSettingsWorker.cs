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

#if !RW12
using System;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit;
using TwitchToolkit.Settings;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

#endif

namespace SirRandoo.ToolkitUtils.Workers
{
#if !RW12
    public static class ToolkitSettingsWorker
    {
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

        public static void Draw(Rect region)
        {
            GUI.BeginGroup(region);
            Rect wikiRect = SettingsHelper.RectForIcon(
                new Rect(region.width - Text.SmallFontHeight, 0f, Text.SmallFontHeight - 4f, Text.SmallFontHeight - 4f)
            );

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
            _tabWorker.AddTab(
                _coinTabItem ??= new TabItem
                {
                    ContentDrawer = DrawCoinSettings, Label = "TwitchToolkitCoins".Localize()
                }
            );
            _tabWorker.AddTab(
                _cooldownTabItem ??= new TabItem
                {
                    ContentDrawer = DrawCooldownSettings, Label = "TwitchToolkitCooldowns".Localize()
                }
            );
            _tabWorker.AddTab(
                _karmaTabItem ??= new TabItem
                {
                    ContentDrawer = DrawKarmaSettings, Label = "TwitchToolkitKarma".Localize()
                }
            );
            _tabWorker.AddTab(
                _patchesTabItem ??= new TabItem { ContentDrawer = DrawPatchesSettings, Label = "Patches" }
            );
            _tabWorker.AddTab(
                _storeTabItem ??= new TabItem
                {
                    ContentDrawer = DrawStoreSettings, Label = "TwitchToolkitStore".Localize()
                }
            );
            _tabWorker.AddTab(
                _storytellerTabItem ??= new TabItem { ContentDrawer = DrawStorytellerSettings, Label = "Storyteller" }
            );
            _tabWorker.AddTab(
                _viewerTabItem ??= new TabItem
                {
                    ContentDrawer = DrawViewerSettings, Label = "TwitchToolkitViewers".Localize()
                }
            );
        }

        private static void DrawCooldownSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect cooldownLabel, Rect cooldownField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(cooldownLabel, "Days per cooldown period");
            _eventCooldownBuffer ??= ToolkitSettings.EventCooldownInterval.ToString();
            Widgets.TextFieldNumeric(
                cooldownField,
                ref ToolkitSettings.EventCooldownInterval,
                ref _eventCooldownBuffer,
                1f,
                15f
            );
            listing.Gap();

            listing.CheckboxLabeled("TwitchToolkitMaxEventsLimit".Localize(), ref ToolkitSettings.MaxEvents);
            listing.Gap();

            if (ToolkitSettings.MaxEvents)
            {
                (Rect badEventsLabel, Rect badEventsField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(badEventsLabel, "TwitchToolkitMaxBadEvents".Localize());
                _maxBadEventsBuffer ??= ToolkitSettings.MaxBadEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(
                    badEventsField,
                    ref ToolkitSettings.MaxBadEventsPerInterval,
                    ref _maxBadEventsBuffer
                );

                (Rect goodEventsLabel, Rect goodEventsField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(goodEventsLabel, "TwitchToolkitMaxGoodEvents".Localize());
                _maxGoodEventsBuffer ??= ToolkitSettings.MaxGoodEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(
                    goodEventsField,
                    ref ToolkitSettings.MaxGoodEventsPerInterval,
                    ref _maxGoodEventsBuffer
                );

                (Rect neutralEventsLabel, Rect neutralEventsField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(neutralEventsLabel, "TwitchToolkitMaxNeutralEvents".Localize());
                _maxNeutralEventsBuffer ??= ToolkitSettings.MaxNeutralEventsPerInterval.ToString();
                Widgets.TextFieldNumeric(
                    neutralEventsField,
                    ref ToolkitSettings.MaxNeutralEventsPerInterval,
                    ref _maxNeutralEventsBuffer
                );

                (Rect itemEventsLabel, Rect itemEventsField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(itemEventsLabel, "TwitchToolkitMaxItemEvents".Localize());
                _maxItemEventsBuffer ??= ToolkitSettings.MaxCarePackagesPerInterval.ToString();
                Widgets.TextFieldNumeric(
                    itemEventsField,
                    ref ToolkitSettings.MaxCarePackagesPerInterval,
                    ref _maxItemEventsBuffer
                );

                listing.Gap();
            }

            listing.CheckboxLabeled(
                "TwitchToolkitEventsHaveCooldowns".Localize(),
                ref ToolkitSettings.EventsHaveCooldowns
            );

            listing.End();
        }

        private static void DrawKarmaSettings(Rect region)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(
                0f,
                0f,
                region.width - 16f,
                Text.LineHeight * (ToolkitSettings.KarmaReqsForGifting ? 32f : 29f)
            );

            Widgets.BeginScrollView(region, ref _karmaScrollPos, viewPort);
            listing.Begin(viewPort);

            (Rect startKarmaLabel, Rect startKarmaField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(startKarmaLabel, "TwitchToolkitStartingKarma".Localize());
            _startKarmaBuffer ??= ToolkitSettings.StartingKarma.ToString();
            Widgets.TextFieldNumeric(
                startKarmaField,
                ref ToolkitSettings.StartingKarma,
                ref _startKarmaBuffer,
                50f,
                250f
            );

            (Rect karmaCapLabel, Rect karmaCapField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(karmaCapLabel, "TwitchToolkitKarmaCap".Localize());
            _karmaCapBuffer ??= ToolkitSettings.KarmaCap.ToString();
            Widgets.TextFieldNumeric(karmaCapField, ref ToolkitSettings.KarmaCap, ref _karmaCapBuffer);

            listing.CheckboxLabeled(
                "TwitchToolkitBanViewersWhoAreBad".Localize(),
                ref ToolkitSettings.BanViewersWhoPurchaseAlwaysBad
            );
            listing.Gap();

            (Rect karMinLabel, Rect karMinField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(karMinLabel, "What is the minimum amount of karma viewers can reach?");
            _minKarmaBuffer ??= ToolkitSettings.KarmaMinimum.ToString();
            Widgets.TextFieldNumeric(karMinField, ref ToolkitSettings.KarmaMinimum, ref _minKarmaBuffer, -200000, 100f);
            listing.Gap();

            listing.CheckboxLabeled(
                "TwitchToolkitKarmaReqsForGifting".Localize(),
                ref ToolkitSettings.KarmaReqsForGifting
            );
            listing.Gap();

            if (ToolkitSettings.KarmaReqsForGifting)
            {
                (Rect minKarmaGiftLabel, Rect minKarmaGiftField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(minKarmaGiftLabel, "TwitchToolkitMinKarmaForGifts".Localize());
                _minGiftKarmaBuffer ??= ToolkitSettings.MinimumKarmaToRecieveGifts.ToString();
                Widgets.TextFieldNumeric(
                    minKarmaGiftField,
                    ref ToolkitSettings.MinimumKarmaToRecieveGifts,
                    ref _minGiftKarmaBuffer,
                    10f
                );

                (Rect minKarmaGiftingLabel, Rect minKarmaGiftingField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(minKarmaGiftingLabel, "TwitchToolkitMinKarmaSendGifts".Localize());
                _minGiftingKarmaBuffer ??= ToolkitSettings.MinimumKarmaToSendGifts.ToString();
                Widgets.TextFieldNumeric(
                    minKarmaGiftingField,
                    ref ToolkitSettings.MinimumKarmaToSendGifts,
                    ref _minGiftingKarmaBuffer,
                    20f,
                    150f
                );
                listing.Gap();
            }

            listing.DrawGroupHeader("TwitchToolkitGoodViewers".Localize());
            (Rect tOneGoodLabel, Rect tOneGoodField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tOneGoodLabel, "TwitchToolkitGoodKarma".Localize());
            _tOneGoodKarmaBuffer ??= ToolkitSettings.TierOneGoodBonus.ToString();
            Widgets.TextFieldNumeric(tOneGoodField, ref ToolkitSettings.TierOneGoodBonus, ref _tOneGoodKarmaBuffer, 1f);

            (Rect tOneNeutralLabel, Rect tOneNeutralField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tOneNeutralLabel, "TwitchToolkitNeutralKarma".Localize());
            _tOneNeutralKarmaBuffer ??= ToolkitSettings.TierOneNeutralBonus.ToString();
            Widgets.TextFieldNumeric(
                tOneNeutralField,
                ref ToolkitSettings.TierOneNeutralBonus,
                ref _tOneNeutralKarmaBuffer,
                1f
            );

            (Rect tOneBadLabel, Rect tOneBadField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tOneBadLabel, "TwitchToolkitBadKarma".Localize());
            _tOneBadKarmaBuffer ??= ToolkitSettings.TierOneBadBonus.ToString();
            Widgets.TextFieldNumeric(tOneBadField, ref ToolkitSettings.TierOneBadBonus, ref _tOneBadKarmaBuffer, 1f);

            listing.DrawGroupHeader("TwitchToolkitNeutralViewers".Localize());
            (Rect tTwoGoodLabel, Rect tTwoGoodField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tTwoGoodLabel, "TwitchToolkitGoodKarma".Localize());
            _tTwoGoodKarmaBuffer ??= ToolkitSettings.TierTwoGoodBonus.ToString();
            Widgets.TextFieldNumeric(tTwoGoodField, ref ToolkitSettings.TierTwoGoodBonus, ref _tTwoGoodKarmaBuffer, 1f);

            (Rect tTwoNeutralLabel, Rect tTwoNeutralField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tTwoNeutralLabel, "TwitchToolkitNeutralKarma".Localize());
            _tTwoNeutralKarmaBuffer ??= ToolkitSettings.TierTwoNeutralBonus.ToString();
            Widgets.TextFieldNumeric(
                tTwoNeutralField,
                ref ToolkitSettings.TierTwoNeutralBonus,
                ref _tTwoNeutralKarmaBuffer,
                1f
            );

            (Rect tTwoBadLabel, Rect tTwoBadField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tTwoBadLabel, "TwitchToolkitBadKarma".Localize());
            _tTwoBadKarmaBuffer ??= ToolkitSettings.TierTwoBadBonus.ToString();
            Widgets.TextFieldNumeric(tTwoBadField, ref ToolkitSettings.TierTwoBadBonus, ref _tTwoBadKarmaBuffer, 1f);

            listing.DrawGroupHeader("TwitchToolkitBadViewers".Localize());
            (Rect tThreeGoodLabel, Rect tThreeGoodField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tThreeGoodLabel, "TwitchToolkitGoodKarma".Localize());
            _tThreeGoodKarmaBuffer ??= ToolkitSettings.TierThreeGoodBonus.ToString();
            Widgets.TextFieldNumeric(
                tThreeGoodField,
                ref ToolkitSettings.TierThreeGoodBonus,
                ref _tThreeGoodKarmaBuffer,
                1f
            );

            (Rect tThreeNeutralLabel, Rect tThreeNeutralField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tThreeNeutralLabel, "TwitchToolkitNeutralKarma".Localize());
            _tThreeNeutralKarmaBuffer ??= ToolkitSettings.TierThreeNeutralBonus.ToString();
            Widgets.TextFieldNumeric(
                tThreeNeutralField,
                ref ToolkitSettings.TierThreeNeutralBonus,
                ref _tThreeNeutralKarmaBuffer,
                1f
            );

            (Rect tThreeBadLabel, Rect tThreeBadField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tThreeBadLabel, "TwitchToolkitBadKarma".Localize());
            _tThreeBadKarmaBuffer ??= ToolkitSettings.TierThreeBadBonus.ToString();
            Widgets.TextFieldNumeric(
                tThreeBadField,
                ref ToolkitSettings.TierThreeBadBonus,
                ref _tThreeBadKarmaBuffer,
                1f
            );

            listing.DrawGroupHeader("TwitchToolkitDoomViewers".Localize());
            (Rect tFourGoodLabel, Rect tFourGoodField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tFourGoodLabel, "TwitchToolkitGoodKarma".Localize());
            _tFourGoodKarmaBuffer ??= ToolkitSettings.TierFourGoodBonus.ToString();
            Widgets.TextFieldNumeric(
                tFourGoodField,
                ref ToolkitSettings.TierFourGoodBonus,
                ref _tFourGoodKarmaBuffer,
                1f
            );

            (Rect tFourNeutralLabel, Rect tFourNeutralField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tFourNeutralLabel, "TwitchToolkitNeutralKarma".Localize());
            _tFourNeutralKarmaBuffer ??= ToolkitSettings.TierFourNeutralBonus.ToString();
            Widgets.TextFieldNumeric(
                tFourNeutralField,
                ref ToolkitSettings.TierFourNeutralBonus,
                ref _tFourNeutralKarmaBuffer,
                1f
            );

            (Rect tFourBadLabel, Rect tFourBadField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(tFourBadLabel, "TwitchToolkitBadKarma".Localize());
            _tFourBadKarmaBuffer ??= ToolkitSettings.TierFourBadBonus.ToString();
            Widgets.TextFieldNumeric(tFourBadField, ref ToolkitSettings.TierFourBadBonus, ref _tFourBadKarmaBuffer, 1f);

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawPatchesSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            foreach (ToolkitExtension ext in Settings_ToolkitExtensions.GetExtensions)
            {
                (Rect label, Rect field) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(label, ext.mod.SettingsCategory());

                if (!Widgets.ButtonText(field, "Settings"))
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
                    LogHelper.Error(
                        $"Could not open settings window for {ext.mod.SettingsCategory()}'s storyteller",
                        e
                    );
                }

                if (window != null)
                {
                    Find.WindowStack.Add(window);
                }
            }

            listing.End();
        }

        private static void DrawStoreSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            listing.CheckboxLabeled("TwitchToolkitEarningCoins".Localize(), ref ToolkitSettings.EarningCoins);

            (Rect listLabel, Rect listField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(listLabel, "TwitchToolkitCustomPricingLink".Localize());
            ToolkitSettings.CustomPricingSheetLink = Widgets.TextField(
                listField,
                ToolkitSettings.CustomPricingSheetLink
            );
            listing.Gap();
            listing.GapLine();

            string openText = "TKUtils.Buttons.Open".Localize();
            (Rect itemsEditLabel, Rect itemsEditField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(itemsEditLabel, "Items Edit");

            if (Widgets.ButtonText(itemsEditField, openText))
            {
                Find.WindowStack.Add(new StoreDialog());
            }

            (Rect eventsEditLabel, Rect eventsEditField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(eventsEditLabel, "Events Edit");

            if (Widgets.ButtonText(eventsEditField, openText))
            {
                Find.WindowStack.Add(new StoreIncidentsWindow());
            }

            (Rect traitsEditLabel, Rect traitsEditField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(traitsEditLabel, $"[ToolkitUtils] {"Traits".Localize()}");

            if (Widgets.ButtonText(traitsEditField, openText))
            {
                Find.WindowStack.Add(new TraitConfigDialog());
            }

            (Rect kindsEditLabel, Rect kindsEditField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(kindsEditLabel, $"[ToolkitUtils] {"Race".Localize().Pluralize()}");

            if (Widgets.ButtonText(kindsEditField, openText))
            {
                Find.WindowStack.Add(new PawnKindConfigDialog());
            }

            (Rect editorLabel, Rect editorField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(editorLabel, $"[ToolkitUtils] {"TKUtils.Editor.Title".Localize()}");

            if (Widgets.ButtonText(editorField, openText))
            {
                Find.WindowStack.Add(new Editor());
            }

            listing.Gap();
            listing.GapLine();

            listing.CheckboxLabeled(
                "TwitchToolkitPurchaseConfirmations".Localize(),
                ref ToolkitSettings.PurchaseConfirmations
            );
            listing.CheckboxLabeled("TwitchToolkitRepeatViewerNames".Localize(), ref ToolkitSettings.RepeatViewerNames);
            listing.CheckboxLabeled(
                "TwitchToolkitMinifiableBuildings".Localize(),
                ref ToolkitSettings.MinifiableBuildings
            );

            listing.End();
        }

        private static void DrawStorytellerSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            listing.DrawGroupHeader("Global", false);

            (Rect voteTimeLabel, Rect voteTimeField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(voteTimeLabel, "TwitchToolkitVoteTime".Localize());
            _voteTimeBuffer ??= ToolkitSettings.VoteTime.ToString();
            Widgets.TextFieldNumeric(voteTimeField, ref ToolkitSettings.VoteTime, ref _voteTimeBuffer, 1f, 15f);

            (Rect voteOptionsLabel, Rect voteOptionsField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(voteOptionsLabel, "TwitchToolkitVoteOptions".Localize());
            _voteOptionsBuffer ??= ToolkitSettings.VoteOptions.ToString();
            Widgets.TextFieldNumeric(voteOptionsField, ref ToolkitSettings.VoteOptions, ref _voteOptionsBuffer, 2f, 5f);

            listing.CheckboxLabeled("TwitchToolkitVotingChatMsgs".Localize(), ref ToolkitSettings.VotingChatMsgs);
            listing.CheckboxLabeled("TwitchToolkitVotingWindow".Localize(), ref ToolkitSettings.VotingWindow);
            listing.CheckboxLabeled("TwitchToolkitLargeVotingWindow".Localize(), ref ToolkitSettings.LargeVotingWindow);
            listing.Gap();
            listing.Gap();

            (Rect editPacksLabel, Rect editPacksField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(editPacksLabel, "Edit Storyteller Packs");

            if (!Widgets.ButtonText(editPacksField, "Storyteller Packs"))
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
            var viewPort = new Rect(
                0f,
                0f,
                region.width - 16f,
                Text.LineHeight * (ToolkitSettings.ChargeViewersForQueue ? 23f : 22f)
            );

            Widgets.BeginScrollView(region.AtZero(), ref _viewerScrollPos, viewPort);
            listing.Begin(viewPort);

            listing.CheckboxLabeled(
                "Allow viewers to !joinqueue to join name queue?",
                ref ToolkitSettings.EnableViewerQueue
            );
            listing.CheckboxLabeled(
                "TwitchToolkitViewerColonistQueue".Localize(),
                ref ToolkitSettings.ViewerNamedColonistQueue
            );
            listing.CheckboxLabeled("Charge viewers to join queue?", ref ToolkitSettings.ChargeViewersForQueue);

            if (ToolkitSettings.ChargeViewersForQueue)
            {
                (Rect chargeLabel, Rect chargeField) = listing.GetRectAsForm(0.85f);
                SettingsHelper.DrawLabel(chargeLabel, "Cost to join queue");
                _queueCostBuffer ??= ToolkitSettings.CostToJoinQueue.ToString();
                Widgets.TextFieldNumeric(chargeField, ref ToolkitSettings.CostToJoinQueue, ref _queueCostBuffer);
            }

            listing.DrawGroupHeader($"Special Viewers - {"Subscribers".ColorTagged(ColorLibrary.Pink)}");
            (Rect subCoinLabel, Rect subCoinField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(subCoinLabel, "Extra coins");
            _subCoinBuffer ??= ToolkitSettings.SubscriberExtraCoins.ToString();
            Widgets.TextFieldNumeric(
                subCoinField,
                ref ToolkitSettings.SubscriberExtraCoins,
                ref _subCoinBuffer,
                max: 100f
            );

            (Rect subMultLabel, Rect subMultField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(subMultLabel, "Coin bonus multiplier");
            _subMultBuffer ??= ToolkitSettings.SubscriberCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(
                subMultField,
                ref ToolkitSettings.SubscriberCoinMultiplier,
                ref _subMultBuffer,
                1f,
                5f
            );

            (Rect subVotesLabel, Rect subVotesField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(subVotesLabel, "Extra votes");
            _subVotesBuffer ??= ToolkitSettings.SubscriberExtraVotes.ToString();
            Widgets.TextFieldNumeric(
                subVotesField,
                ref ToolkitSettings.SubscriberExtraVotes,
                ref _subVotesBuffer,
                max: 100f
            );

            listing.DrawGroupHeader($"Special Viewers - {"VIPs".ColorTagged(ColorLibrary.Lavender)}");
            (Rect vipCoinLabel, Rect vipCoinField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(vipCoinLabel, "Extra coins");
            _vipCoinBuffer ??= ToolkitSettings.VIPExtraCoins.ToString();
            Widgets.TextFieldNumeric(vipCoinField, ref ToolkitSettings.VIPExtraCoins, ref _vipCoinBuffer, max: 100f);

            (Rect vipMultLabel, Rect vipMultField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(vipMultLabel, "Coin bonus multiplier");
            _vipMultBuffer ??= ToolkitSettings.VIPCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(vipMultField, ref ToolkitSettings.VIPCoinMultiplier, ref _vipMultBuffer, 1f, 5f);

            (Rect vipVotesLabel, Rect vipVotesField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(vipVotesLabel, "Extra votes");
            _vipVotesBuffer ??= ToolkitSettings.VIPExtraVotes.ToString();
            Widgets.TextFieldNumeric(vipVotesField, ref ToolkitSettings.VIPExtraVotes, ref _vipVotesBuffer, max: 100f);

            listing.DrawGroupHeader($"Special Viewers - {"Mods".ColorTagged(ColorLibrary.PaleGreen)}");
            (Rect modCoinLabel, Rect modCoinField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(modCoinLabel, "Extra coins");
            _modCoinBuffer ??= ToolkitSettings.ModExtraCoins.ToString();
            Widgets.TextFieldNumeric(modCoinField, ref ToolkitSettings.ModExtraCoins, ref _modCoinBuffer, max: 100f);

            (Rect modMultLabel, Rect modMultField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(modMultLabel, "Coin bonus multiplier");
            _modMultBuffer ??= ToolkitSettings.ModCoinMultiplier.ToString();
            Widgets.TextFieldNumeric(modMultField, ref ToolkitSettings.ModCoinMultiplier, ref _modMultBuffer, 1f, 5f);

            (Rect modVotesLabel, Rect modVotesField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(modVotesLabel, "Extra votes");
            _modVotesBuffer ??= ToolkitSettings.ModExtraVotes.ToString();
            Widgets.TextFieldNumeric(modVotesField, ref ToolkitSettings.ModExtraVotes, ref _modVotesBuffer, max: 100f);

            listing.Gap();

            Rect lineRect = listing.GetRect(Text.LineHeight * 1.5f);
            float buttonWidth = lineRect.width * 0.35f;
            var editViewers = new Rect(
                lineRect.center.x - Mathf.FloorToInt(buttonWidth / 2f),
                lineRect.y,
                buttonWidth,
                lineRect.height
            );

            if (!Widgets.ButtonText(editViewers, "Edit Viewers"))
            {
                listing.End();
                Widgets.EndScrollView();
                return;
            }

            Find.WindowStack.Add(new Window_Viewers());
            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawSettings(Rect region)
        {
            var titleRect = new Rect(region.x, region.y, region.width, Text.SmallFontHeight);
            var gapRect = new Rect(region.x, region.y + titleRect.height, region.width, Text.SmallFontHeight);
            var tabRect = new Rect(
                gapRect.x,
                gapRect.y + gapRect.height,
                gapRect.width,
                Mathf.FloorToInt(Text.SmallFontHeight * 1.5f)
            );

            SettingsHelper.DrawColoredLabel(
                titleRect,
                Toolkit.Mod.Content.Name,
                Color.magenta,
                TextAnchor.MiddleCenter,
                GameFont.Medium
            );

            Widgets.DrawLineHorizontal(gapRect.x, gapRect.y, gapRect.width);

            GUI.BeginGroup(tabRect);
            DrawTabButtons(tabRect.AtZero());
            GUI.EndGroup();

            DrawContent(
                new Rect(
                    tabRect.x,
                    tabRect.y + tabRect.height,
                    region.width,
                    region.height - tabRect.height - gapRect.height - titleRect.height
                )
            );
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
            Rect cooldownsRect = coinsRect.ShiftRight(0f);
            Rect karmaRect = cooldownsRect.ShiftRight(0f);
            Rect patchesRect = karmaRect.ShiftRight(0f);
            Rect storeRect = patchesRect.ShiftRight(0f);
            Rect storytellerRect = storeRect.ShiftRight(0f);
            Rect viewersRect = storytellerRect.ShiftRight(0f);

            if (DrawTabButton(coinsRect, _coinTabItem.Label, _tabWorker.SelectedTab == _coinTabItem))
            {
                _tabWorker.SelectedTab = _coinTabItem;
            }

            if (DrawTabButton(cooldownsRect, _cooldownTabItem.Label, _tabWorker.SelectedTab == _cooldownTabItem))
            {
                _tabWorker.SelectedTab = _cooldownTabItem;
            }

            if (DrawTabButton(karmaRect, _karmaTabItem.Label, _tabWorker.SelectedTab == _karmaTabItem))
            {
                _tabWorker.SelectedTab = _karmaTabItem;
            }

            if (DrawTabButton(patchesRect, _patchesTabItem.Label, _tabWorker.SelectedTab == _patchesTabItem))
            {
                _tabWorker.SelectedTab = _patchesTabItem;
            }

            if (DrawTabButton(storeRect, _storeTabItem.Label, _tabWorker.SelectedTab == _storeTabItem))
            {
                _tabWorker.SelectedTab = _storeTabItem;
            }

            if (DrawTabButton(
                storytellerRect,
                _storytellerTabItem.Label,
                _tabWorker.SelectedTab == _storytellerTabItem
            ))
            {
                _tabWorker.SelectedTab = _storytellerTabItem;
            }

            if (DrawTabButton(viewersRect, _viewerTabItem.Label, _tabWorker.SelectedTab == _viewerTabItem))
            {
                _tabWorker.SelectedTab = _viewerTabItem;
            }
        }

        private static void DrawCoinSettings(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);
            (Rect startBalLabel, Rect startBalField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(startBalLabel, "TwitchToolkitStartingBalance".Localize());
            _startingBalanceBuffer ??= ToolkitSettings.StartingBalance.ToString();
            Widgets.TextFieldNumeric(startBalField, ref ToolkitSettings.StartingBalance, ref _startingBalanceBuffer);

            (Rect coinIntLabel, Rect coinIntField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(coinIntLabel, "TwitchToolkitCoinInterval".Localize());
            _coinIntervalBuffer ??= ToolkitSettings.CoinInterval.ToString();
            Widgets.TextFieldNumeric(coinIntField, ref ToolkitSettings.CoinInterval, ref _coinIntervalBuffer);

            (Rect coinAmountLabel, Rect coinAmountField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(coinAmountLabel, "TwitchToolkitCoinAmount".Localize());
            _coinAmountBuffer ??= ToolkitSettings.CoinAmount.ToString();
            Widgets.TextFieldNumeric(coinAmountField, ref ToolkitSettings.CoinAmount, ref _coinAmountBuffer);

            (Rect minPurLabel, Rect minPurField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(minPurLabel, "TwitchToolkitMinimumPurchasePrice".Localize());
            _minimumPurchaseBuffer ??= ToolkitSettings.MinimumPurchasePrice.ToString();
            Widgets.TextFieldNumeric(minPurField, ref ToolkitSettings.MinimumPurchasePrice, ref _minimumPurchaseBuffer);

            listing.Gap();
            listing.CheckboxLabeled("TwitchToolkitUnlimitedCoins".Localize(), ref ToolkitSettings.UnlimitedCoins);
            listing.GapLine();
            listing.Gap();

            listing.CheckboxLabeled("TwitchToolkitChatReqsForCoins".Localize(), ref ToolkitSettings.ChatReqsForCoins);

            if (!ToolkitSettings.ChatReqsForCoins)
            {
                listing.End();
                return;
            }

            (Rect halfCoinsLabel, Rect halfCoinsField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(halfCoinsLabel, "TwitchToolkitTimeBeforeHalfCoins".Localize());
            _halfCoinsBuffer ??= ToolkitSettings.TimeBeforeHalfCoins.ToString();
            Widgets.TextFieldNumeric(halfCoinsField, ref ToolkitSettings.TimeBeforeHalfCoins, ref _halfCoinsBuffer);

            (Rect noCoinsLabel, Rect noCoinsField) = listing.GetRectAsForm(0.85f);
            SettingsHelper.DrawLabel(noCoinsLabel, "TwitchToolkitTimeBeforeNoCoins".Localize());
            _noCoinsBuffer ??= ToolkitSettings.TimeBeforeNoCoins.ToString();
            Widgets.TextFieldNumeric(noCoinsField, ref ToolkitSettings.TimeBeforeNoCoins, ref _noCoinsBuffer);
            listing.End();
        }

        private static bool DrawTabButton(Rect region, string text, bool active = false)
        {
            if (!active)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }

            Widgets.DrawAtlas(region, Widgets.ButtonBGAtlas);
            GUI.color = Color.white;
            SettingsHelper.DrawLabel(region, text, TextAnchor.MiddleCenter);
            bool result = Widgets.ButtonInvisible(region);
            return result;
        }
    }
#endif
}
