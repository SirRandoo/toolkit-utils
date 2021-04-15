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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
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

            if (!(item.Thing.Thing is {IsApparel: true}))
            {
                return false;
            }

            cost = item.Quality.HasValue
                ? item.Thing.GetItemPrice(item.Stuff, item.Quality.Value)
                : item.Thing.GetItemPrice(item.Stuff);

            if (!Viewer.CanAfford(cost))
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

            if (!item.Thing.Thing.MadeFromStuff)
            {
                return true;
            }

            return item.Thing.Thing.stuffCategories
               .Select(category => item.Stuff.Thing.stuffCategories.Any(c => c.Equals(category)))
               .Any(valid => valid);
        }

        public override void Execute()
        {
            if (!((item.Stuff == null || !item.Thing.Thing.MadeFromStuff
                ? ThingMaker.MakeThing(item.Thing.Thing)
                : ThingMaker.MakeThing(item.Thing.Thing, item.Stuff.Thing)) is Apparel apparel))
            {
                LogHelper.Warn("Tried to wear a null apparel.");
                return;
            }

            if (MassUtility.WillBeOverEncumberedAfterPickingUp(pawn, apparel, 1)
                || pawn.apparel.WouldReplaceLockedApparel(apparel)
                || !EquipmentUtility.CanEquip(apparel, pawn))
            {
                TradeUtility.SpawnDropPod(pawn.Position, pawn.Map, apparel);
                Viewer.Charge(
                    cost,
                    item.Thing.ItemData?.Weight ?? 1f,
                    item.Thing.ItemData?.KarmaType ?? storeIncident.karmaType
                );
                return;
            }

            pawn.apparel.Wear(apparel);
            Viewer.Charge(
                cost,
                item.Thing.ItemData?.Weight ?? 1f,
                item.Thing.ItemData?.KarmaType ?? storeIncident.karmaType
            );
        }
    }
}
