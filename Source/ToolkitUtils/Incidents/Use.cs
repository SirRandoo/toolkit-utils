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
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Use : IncidentVariablesBase
    {
        private int amount = 1;
        private ThingItem buyableItem;
        private Pawn pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy item)
                || !item.IsValid()
                || !item.Thing.Thing.HasAssignableCompFrom(typeof(CompUseEffect)))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InvalidItemQuery".LocalizeKeyed(item?.Thing?.Name ?? worker.GetLast())
                );
                return false;
            }

            buyableItem = item.Thing;
            if (item.TryGetInvalidSelector(out ThingItem i))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Item.Disabled".LocalizeKeyed(i.Name));
                return false;
            }

            if (!worker.TryGetNextAsInt(out amount, 1, viewer.GetMaximumPurchaseAmount(buyableItem.Cost)))
            {
                amount = 1;
            }

            if (!viewer.CanAfford(buyableItem.Cost * amount))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".LocalizeKeyed(
                        buyableItem.Cost.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
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

            buyableItem = item!.Thing;
            return true;
        }

        public override void Execute()
        {
            if (!pawn.Spawned)
            {
                LogHelper.Warn("Tried to use an item on an unspawned pawn.");
                return;
            }

            Thing thing = ThingMaker.MakeThing(buyableItem.Thing);
            var comp = thing.TryGetComp<CompUseEffect>();
            string failReason = null;

            if (comp == null || !comp.CanBeUsedBy(pawn, out failReason))
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Use.Unusable".LocalizeKeyed(buyableItem.Name));

                if (failReason != null)
                {
                    LogHelper.Warn($"Tried to use an item on a pawn that can't use it. Fail reason: {failReason}");
                }

                return;
            }

            try
            {
                GenSpawn.Spawn(thing, pawn.Position, pawn.Map);
                comp.DoEffect(pawn);

                if (thing.Spawned)
                {
                    thing.DeSpawn();
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(
                    $"Could not use the item {thing.Label ?? thing.def.defName} on {Viewer.username}'s pawn.",
                    e
                );
                return;
            }

            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.Use.Complete".LocalizeKeyed(thing.LabelCap ?? thing.def.defName)
            );

            Viewer.Charge(
                buyableItem.Cost,
                buyableItem.ItemData?.Weight ?? 1f,
                buyableItem.ItemData?.KarmaType ?? storeIncident.karmaType
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.UseLetter.Title".Localize(),
                "TKUtils.UseLetter.Description".LocalizeKeyed(Viewer.username, thing.LabelCap ?? thing.def.defName),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}
