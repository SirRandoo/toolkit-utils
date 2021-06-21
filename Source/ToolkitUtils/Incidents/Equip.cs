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
    [UsedImplicitly]
    public class Equip : IncidentVariablesBase
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

            if (!(item.Thing.Thing is {IsWeapon: true}))
            {
                return false;
            }

            List<ResearchProjectDef> prerequisites = item.Thing.Thing.GetUnfinishedPrerequisites();
            if (BuyItemSettings.mustResearchFirst && prerequisites.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".LocalizeKeyed(
                        item.Thing.Thing.LabelCap.RawText,
                        prerequisites.Select(p => p.LabelCap.RawText).SectionJoin()
                    )
                );
                return false;
            }

            cost = item.Quality.HasValue
                ? item.Thing.GetItemPrice(item.Stuff, item.Quality.Value)
                : item.Thing.GetItemPrice(item.Stuff);

            if (!viewer.CanAfford(cost))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".LocalizeKeyed(
                        cost.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            if (!item.Thing.Thing.MadeFromStuff || item.Stuff == null || item.Thing.Thing.CanBeStuff(item.Stuff.Thing))
            {
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Item.MaterialViolation".LocalizeKeyed(item.Thing.Name, item.Stuff.Name)
            );
            return false;
        }

        public override void Execute()
        {
            if (!((item.Stuff == null || !item.Thing.Thing.MadeFromStuff
                ? ThingMaker.MakeThing(item.Thing.Thing, GenStuff.RandomStuffByCommonalityFor(item.Thing.Thing))
                : ThingMaker.MakeThing(item.Thing.Thing, item.Stuff.Thing)) is ThingWithComps weapon))
            {
                LogHelper.Warn("Tried to equip a null weapon.");
                return;
            }

            if (item.Quality.HasValue)
            {
                weapon.GetComp<CompQuality>()?.SetQuality(item.Quality.Value, ArtGenerationContext.Outsider);
            }

            if (!EquipmentUtility.CanEquip(weapon, pawn) || !MassUtility.CanEverCarryAnything(pawn))
            {
                TradeUtility.SpawnDropPod(pawn.Position, pawn.Map, weapon);
                Viewer.Charge(
                    cost,
                    item.Thing.ItemData?.Weight ?? 1f,
                    item.Thing.ItemData?.KarmaType ?? storeIncident.karmaType
                );
                return;
            }

            ThingWithComps old = null;
            if (pawn.equipment.Primary != null
                && !pawn.equipment.TryTransferEquipmentToContainer(
                    pawn.equipment.Primary,
                    pawn.inventory.innerContainer
                )
                && !pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out old, pawn.Position))
            {
                LogHelper.Warn($"Could not make room for {Viewer.username}'s new weapon.");
            }

            if (MassUtility.WillBeOverEncumberedAfterPickingUp(pawn, weapon, 1) && old != null)
            {
                pawn.equipment.AddEquipment(old);
                PurchaseHelper.SpawnItem(pawn.Position, pawn.Map, weapon);
                Viewer.Charge(
                    cost,
                    item.Thing.ItemData?.Weight ?? 1f,
                    item.Thing.ItemData?.KarmaType ?? storeIncident.karmaType
                );
                NotifyComplete(weapon, true);
                return;
            }

            pawn.equipment.AddEquipment(weapon);
            Viewer.Charge(
                cost,
                item.Thing.ItemData?.Weight ?? 1f,
                item.Thing.ItemData?.KarmaType ?? storeIncident.karmaType
            );
            NotifyComplete(weapon);
        }

        private void NotifyComplete([NotNull] Thing thing, bool spawned = false)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                (spawned ? "TKUtils.Equip.Spawned" : "TKUtils.Equip.Complete").LocalizeKeyed(
                    thing.LabelCap ?? thing.def.defName
                )
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.EquipLetter.Title".Localize(),
                (spawned ? "TKUtils.EquipLetter.SpawnedDescription" : "TKUtils.EquipLetter.Description").LocalizeKeyed(
                    Viewer.username,
                    thing.LabelCap ?? thing.def.defName
                ),
                LetterDefOf.NeutralEvent,
                spawned ? thing : pawn
            );
        }
    }
}
