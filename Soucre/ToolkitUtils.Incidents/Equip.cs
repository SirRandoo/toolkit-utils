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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class Equip : IncidentVariablesBase
    {
        private int _cost;
        private ArgWorker.ItemProxy _item;
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsItem(out _item) || !_item.IsValid())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (_item.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(viewer.username, error);

                return false;
            }

            if (!(_item.Thing.Thing is { IsWeapon: true }) || _item.Thing.ItemData?.IsEquippable != true)
            {
                return false;
            }

            List<ResearchProjectDef> prerequisites = _item.Thing.Thing.GetUnfinishedPrerequisites();

            if (BuyItemSettings.mustResearchFirst && prerequisites.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".LocalizeKeyed(_item.Thing.Thing.LabelCap.RawText, prerequisites.Select(p => p.LabelCap.RawText).SectionJoin())
                );

                return false;
            }

            _cost = _item.Quality.HasValue ? _item.Thing.GetItemPrice(_item.Stuff, _item.Quality.Value) : _item.Thing.GetItemPrice(_item.Stuff);

            if (!viewer.CanAfford(_cost))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(_cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

                return false;
            }

            if (!_item.Thing.Thing.MadeFromStuff || _item.Stuff == null || _item.Thing.Thing.CanBeStuff(_item.Stuff.Thing))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Item.MaterialViolation".LocalizeKeyed(_item.Thing.Name, _item.Stuff.Name));

            return false;
        }

        public override void Execute()
        {
            if (!((_item.Stuff == null || !_item.Thing.Thing.MadeFromStuff
                ? ThingMaker.MakeThing(_item.Thing.Thing, GenStuff.RandomStuffByCommonalityFor(_item.Thing.Thing))
                : ThingMaker.MakeThing(_item.Thing.Thing, _item.Stuff.Thing)) is ThingWithComps weapon))
            {
                TkUtils.Logger.Warn("Tried to equip a null weapon.");

                return;
            }

            if (_item.Quality.HasValue)
            {
                weapon.GetComp<CompQuality>()?.SetQuality(_item.Quality.Value, ArtGenerationContext.Outsider);
            }

            if (!EquipmentUtility.CanEquip(weapon, _pawn) || !MassUtility.CanEverCarryAnything(_pawn))
            {
                PurchaseHelper.SpawnItem(DropCellFinder.TradeDropSpot(_pawn.Map), _pawn.Map, weapon);
                Viewer.Charge(_cost, _item.Thing.ItemData?.Weight ?? 1f, _item.Thing.ItemData?.KarmaType ?? storeIncident.karmaType);

                return;
            }

            ThingWithComps old = null;

            if (_pawn.equipment.Primary != null && !_pawn.equipment.TryTransferEquipmentToContainer(_pawn.equipment.Primary, _pawn.inventory.innerContainer)
                && !_pawn.equipment.TryDropEquipment(_pawn.equipment.Primary, out old, _pawn.Position))
            {
                TkUtils.Logger.Warn($"Could not make room for {Viewer.username}'s new weapon.");
            }

            if (MassUtility.WillBeOverEncumberedAfterPickingUp(_pawn, weapon, 1) && old != null)
            {
                _pawn.equipment.AddEquipment(old);
                PurchaseHelper.SpawnItem(_pawn.Position, _pawn.Map, weapon);
                Viewer.Charge(_cost, _item.Thing.ItemData?.Weight ?? 1f, _item.Thing.ItemData?.KarmaTypeForEquipping ?? storeIncident.karmaType);
                NotifyComplete(weapon, true);

                return;
            }

            _pawn.equipment.AddEquipment(weapon);
            Viewer.Charge(_cost, _item.Thing.ItemData?.Weight ?? 1f, _item.Thing.ItemData?.KarmaTypeForEquipping ?? storeIncident.karmaType);
            NotifyComplete(weapon);
        }

        private void NotifyComplete([NotNull] Thing thing, bool spawned = false)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                (spawned ? "TKUtils.Equip.Spawned" : "TKUtils.Equip.Complete").LocalizeKeyed(thing.LabelCap ?? thing.def.defName, _cost.ToString("N0"))
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.EquipLetter.Title".Localize(),
                (spawned ? "TKUtils.EquipLetter.SpawnedDescription" : "TKUtils.EquipLetter.Description").LocalizeKeyed(
                    Viewer.username,
                    thing.LabelCap ?? thing.def.defName
                ),
                LetterDefOf.NeutralEvent,
                spawned ? thing : _pawn
            );
        }
    }
}
