﻿// ToolkitUtils
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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Wear : IncidentVariablesBase
    {
        private int cost;
        private ArgWorker.ItemProxy item;
        private Pawn pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsItem(out item) || !item.IsValid())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            if (item.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(viewer.username, error);
                return false;
            }

            if (!(item.Thing.Thing is { IsApparel: true }) || item.Thing.ItemData?.IsWearable != true)
            {
                return false;
            }

            List<ResearchProjectDef> prerequisites = item.Thing.Thing.GetUnfinishedPrerequisites();
            if (BuyItemSettings.mustResearchFirst && prerequisites.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".LocalizeKeyed(item.Thing.Thing.LabelCap.RawText, prerequisites.Select(p => p.LabelCap.RawText).SectionJoin())
                );
                return false;
            }

            cost = item.Quality.HasValue ? item.Thing.GetItemPrice(item.Stuff, item.Quality.Value) : item.Thing.GetItemPrice(item.Stuff);

            if (!viewer.CanAfford(cost))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));
                return false;
            }

            if (!item.Thing.Thing.MadeFromStuff || item.Stuff == null || item.Thing.Thing.CanBeStuff(item.Stuff.Thing))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Item.MaterialViolation".LocalizeKeyed(item.Thing.Name, item.Stuff.Name));
            return false;
        }

        public override void Execute()
        {
            if (!((item.Stuff != null && item.Thing.Thing.MadeFromStuff
                ? ThingMaker.MakeThing(item.Thing.Thing, item.Stuff.Thing)
                : ThingMaker.MakeThing(item.Thing.Thing, GenStuff.RandomStuffByCommonalityFor(item.Thing.Thing))) is Apparel apparel))
            {
                LogHelper.Warn("Tried to wear a null apparel.");
                return;
            }

            if (item.Quality.HasValue)
            {
                apparel.GetComp<CompQuality>()?.SetQuality(item.Quality.Value, ArtGenerationContext.Outsider);
            }

            if (!ApparelUtility.HasPartsToWear(pawn, item.Thing.Thing))
            {
                bool wasBackpacked = TryBackpack(apparel);
                FinalizeTransaction(apparel, SpawnCode.NoParts, !wasBackpacked);
                return;
            }

            if (pawn.apparel.WouldReplaceLockedApparel(apparel))
            {
                bool wasBackpacked = TryBackpack(apparel);
                FinalizeTransaction(apparel, SpawnCode.LockedApparel, !wasBackpacked);
                return;
            }

            if (!EquipmentUtility.CanEquip(apparel, pawn))
            {
                bool wasBackpacked = TryBackpack(apparel);
                FinalizeTransaction(apparel, SpawnCode.Unequippable, !wasBackpacked);
                return;
            }

            pawn.apparel.Wear(apparel);
            pawn.outfits.forcedHandler.SetForced(apparel, true);
            Viewer.Charge(cost, item.Thing.ItemData?.Weight ?? 1f, item.Thing.ItemData?.KarmaTypeForWearing ?? storeIncident.karmaType);
            FinalizeTransaction(apparel, SpawnCode.Success, false);
        }

        private bool TryBackpack(Thing apparel)
        {
            if (pawn.inventory.innerContainer.TryAdd(apparel))
            {
                return true;
            }

            PurchaseHelper.SpawnItem(DropCellFinder.TradeDropSpot(pawn.Map), pawn.Map, apparel);
            return false;
        }

        private void FinalizeTransaction([NotNull] Thing thing, SpawnCode code, bool spawned)
        {
            Viewer.Charge(cost, item.Thing.ItemData?.Weight ?? 1f, item.Thing.ItemData?.KarmaTypeForWearing ?? storeIncident.karmaType);

            switch (code)
            {
                case SpawnCode.Unequippable:
                    SendUnquippableNotifications(thing, spawned);
                    return;
                case SpawnCode.LockedApparel:
                    SendLockedApparelNotifications(thing, spawned);
                    return;
                case SpawnCode.NoParts:
                    SendNoPartsNotifications(thing, spawned);
                    return;
                default:
                    SendSuccessNotification(thing, spawned);
                    return;
            }
        }

        private void SendUnquippableNotifications([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(Viewer.username, (spawned ? "TKUtils.Wear.UnequippableSpawned" : "TKUtils.Wear.Unequippable").LocalizeKeyed(thing.Label, cost.ToString("N0")));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".Localize(),
                (spawned ? "TKUtils.WearLetter.UnequippableSpawnedDescription" : "TKUtils.WearLetter.UnequippableDescription").LocalizeKeyed(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : pawn
            );
        }

        private void SendLockedApparelNotifications([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(Viewer.username, (spawned ? "TKUtils.Wear.LockedApparelSpawned" : "TKUtils.Wear.LockedApparel").LocalizeKeyed(thing.Label, cost.ToString("N0")));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".Localize(),
                (spawned ? "TKUtils.WearLetter.LockedApparelSpawnedDescription" : "TKUtils.WearLetter.LockedApparelDescription").LocalizeKeyed(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : pawn
            );
        }

        private void SendNoPartsNotifications([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(Viewer.username, (spawned ? "TKUtils.Wear.NoPartsSpawned" : "TKUtils.Wear.NoParts").LocalizeKeyed(thing.Label, cost.ToString("N0")));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".Localize(),
                (spawned ? "TKUtils.WearLetter.NoPartsSpawnedDescription" : "TKUtils.WearLetter.NoPartsDescription").LocalizeKeyed(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : pawn
            );
        }

        private void SendSuccessNotification([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(Viewer.username, (spawned ? "TKUtils.Wear.Spawned" : "TKUtils.Wear.Complete").LocalizeKeyed(thing.Label, cost.ToString("N0")));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".Localize(),
                (spawned ? "TKUtils.WearLetter.SpawnedDescription" : "TKUtils.WearLetter.Description").LocalizeKeyed(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : pawn
            );
        }

        private enum SpawnCode { Success, NoParts, LockedApparel, Unequippable }
    }
}
