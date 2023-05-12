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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using ToolkitUtils.Workers;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace ToolkitUtils.Incidents
{
    public class Wear : IncidentVariablesBase
    {
        private static readonly IRimLogger Logger = new RimThreadedLogger("TKU.Events.Wear");
        
        private int _cost;
        private ArgWorker.ItemProxy _item;
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".TranslateSimple());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsItem(out _item) || !_item.IsValid())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".Translate(worker.GetLast()));

                return false;
            }

            if (_item.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(viewer.username, error);

                return false;
            }

            if (!(_item.Thing.Thing is { IsApparel: true }) || _item.Thing.ItemData?.IsWearable != true)
            {
                return false;
            }

            List<ResearchProjectDef> prerequisites = _item.Thing.Thing.GetUnfinishedPrerequisites();

            if (BuyItemSettings.mustResearchFirst && prerequisites.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".Translate(_item.Thing.Thing.LabelCap.RawText, prerequisites.Select(p => p.LabelCap.RawText).SectionJoin())
                );

                return false;
            }

            _cost = _item.Quality.HasValue ? _item.Thing.GetItemPrice(_item.Stuff, _item.Quality.Value) : _item.Thing.GetItemPrice(_item.Stuff);

            if (!viewer.CanAfford(_cost))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".Translate(_cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

                return false;
            }

            if (!_item.Thing.Thing.MadeFromStuff || _item.Stuff == null || _item.Thing.Thing.CanBeStuff(_item.Stuff.Thing))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Item.MaterialViolation".Translate(_item.Thing.Name, _item.Stuff.Name));

            return false;
        }

        public override void Execute()
        {
            if (!((_item.Stuff != null && _item.Thing.Thing.MadeFromStuff
                ? ThingMaker.MakeThing(_item.Thing.Thing, _item.Stuff.Thing)
                : ThingMaker.MakeThing(_item.Thing.Thing, GenStuff.RandomStuffByCommonalityFor(_item.Thing.Thing))) is Apparel apparel))
            {
                Logger.Warn("Tried to wear a null apparel.");

                return;
            }

            if (_item.Quality.HasValue)
            {
                apparel.GetComp<CompQuality>()?.SetQuality(_item.Quality.Value, ArtGenerationContext.Outsider);
            }

            if (!ApparelUtility.HasPartsToWear(_pawn, _item.Thing.Thing))
            {
                bool wasBackpacked = TryBackpack(apparel);
                FinalizeTransaction(apparel, SpawnCode.NoParts, !wasBackpacked);

                return;
            }

            if (_pawn.apparel.WouldReplaceLockedApparel(apparel))
            {
                bool wasBackpacked = TryBackpack(apparel);
                FinalizeTransaction(apparel, SpawnCode.LockedApparel, !wasBackpacked);

                return;
            }

            if (!EquipmentUtility.CanEquip(apparel, _pawn))
            {
                bool wasBackpacked = TryBackpack(apparel);
                FinalizeTransaction(apparel, SpawnCode.Unequippable, !wasBackpacked);

                return;
            }

            _pawn.apparel.Wear(apparel);
            _pawn.outfits.forcedHandler.SetForced(apparel, true);
            FinalizeTransaction(apparel, SpawnCode.Success, false);
        }

        private bool TryBackpack(Thing apparel)
        {
            if (_pawn.inventory.innerContainer.TryAdd(apparel))
            {
                return true;
            }

            PurchaseHelper.SpawnItem(DropCellFinder.TradeDropSpot(_pawn.Map), _pawn.Map, apparel);

            return false;
        }

        private void FinalizeTransaction([NotNull] Thing thing, SpawnCode code, bool spawned)
        {
            Viewer.Charge(_cost, _item.Thing.ItemData?.Weight ?? 1f, _item.Thing.ItemData?.KarmaTypeForWearing ?? storeIncident.karmaType);

            switch (code)
            {
                case SpawnCode.Unequippable:
                    SendUnequippableNotifications(thing, spawned);

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

        private void SendUnequippableNotifications([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                (spawned ? "TKUtils.Wear.UnequippableSpawned" : "TKUtils.Wear.Unequippable").Translate(thing.Label, _cost.ToString("N0"))
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".TranslateSimple(),
                (spawned ? "TKUtils.WearLetter.UnequippableSpawnedDescription" : "TKUtils.WearLetter.UnequippableDescription").Translate(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : _pawn
            );
        }

        private void SendLockedApparelNotifications([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                (spawned ? "TKUtils.Wear.LockedApparelSpawned" : "TKUtils.Wear.LockedApparel").Translate(thing.Label, _cost.ToString("N0"))
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".TranslateSimple(),
                (spawned ? "TKUtils.WearLetter.LockedApparelSpawnedDescription" : "TKUtils.WearLetter.LockedApparelDescription")
               .Translate(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : _pawn
            );
        }

        private void SendNoPartsNotifications([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                (spawned ? "TKUtils.Wear.NoPartsSpawned" : "TKUtils.Wear.NoParts").Translate(thing.Label, _cost.ToString("N0"))
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".TranslateSimple(),
                (spawned ? "TKUtils.WearLetter.NoPartsSpawnedDescription" : "TKUtils.WearLetter.NoPartsDescription").Translate(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : _pawn
            );
        }

        private void SendSuccessNotification([NotNull] Thing thing, bool spawned)
        {
            MessageHelper.SendConfirmation(Viewer.username, (spawned ? "TKUtils.Wear.Spawned" : "TKUtils.Wear.Complete").Translate(thing.Label, _cost.ToString("N0")));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.WearLetter.Title".TranslateSimple(),
                (spawned ? "TKUtils.WearLetter.SpawnedDescription" : "TKUtils.WearLetter.Description").Translate(Viewer.username, thing.Label),
                LetterDefOf.NeutralEvent,
                spawned ? thing : _pawn
            );
        }

        private enum SpawnCode { Success, NoParts, LockedApparel, Unequippable }
    }
}
