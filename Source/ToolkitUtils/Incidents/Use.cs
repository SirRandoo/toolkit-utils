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
using SirRandoo.ToolkitUtils.Interfaces;
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
        private int _amount = 1;
        private ThingItem _buyableItem;
        private IUsabilityHandler _handler;
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy item) || !item.IsValid())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".LocalizeKeyed(item?.Thing?.Name ?? worker.GetLast()));

                return false;
            }

            _buyableItem = item.Thing;

            if (item.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(viewer.username, error);

                return false;
            }

            if (!worker.TryGetNextAsInt(out _amount, 1, viewer.GetMaximumPurchaseAmount(_buyableItem.Cost)))
            {
                _amount = 1;
            }

            if (!PurchaseHelper.TryMultiply(_buyableItem.Cost, _amount, out int cost))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Overflowed".Localize());

                return false;
            }

            if (!viewer.CanAfford(cost))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

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

            foreach (IUsabilityHandler h in CompatRegistry.AllUsabilityHandlers)
            {
                if (!h.IsUsable(item.Thing.Thing))
                {
                    continue;
                }

                _handler = h;

                break;
            }

            if (_handler == null || item.Thing.ItemData?.IsUsable != true)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.DisabledItem".Localize());

                return false;
            }

            _buyableItem = item!.Thing;

            return true;
        }

        public override void Execute()
        {
            if (!_pawn.Spawned)
            {
                LogHelper.Warn("Tried to use an item on an unspawned pawn.");

                return;
            }

            Thing thing = ThingMaker.MakeThing(_buyableItem.Thing);

            if (!(thing is ThingWithComps))
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Use.Unusable".LocalizeKeyed(_buyableItem.Name));
                LogHelper.Warn("Tried to use an item that can't have comps!");

                return;
            }

            try
            {
                for (var _ = 0; _ < _amount; _++)
                {
                    _handler.Use(_pawn, _buyableItem.Thing);
                }
            }
            catch (Exception e)
            {
                LogHelper.Error($@"The usability handler ""{_handler.GetType().Name}"" did not complete successfully", e);

                return;
            }

            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.Use.Complete".LocalizeKeyed(thing.LabelCap ?? thing.def.defName, _amount.ToString("N0"), (_buyableItem.Cost * _amount).ToString("N0"))
            );

            Viewer.Charge(_buyableItem.Cost * _amount, _buyableItem.ItemData?.Weight ?? 1f, _buyableItem.ItemData?.KarmaTypeForUsing ?? storeIncident.karmaType);

            Find.LetterStack.ReceiveLetter(
                "TKUtils.UseLetter.Title".Localize(),
                "TKUtils.UseLetter.Description".LocalizeKeyed(Viewer.username, thing.LabelCap ?? thing.def.defName, _amount.ToString("N0")),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }
    }
}
