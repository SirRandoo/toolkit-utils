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
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RemoveTrait : IncidentVariablesBase
    {
        private TraitItem _buyable;
        private Pawn _pawn;
        private Trait _trait;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            List<Trait> traits = _pawn!.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.None".Localize());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsTrait(out _buyable))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (!_buyable!.CanRemove)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Disabled".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (!viewer.CanAfford(_buyable.CostToRemove))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".LocalizeKeyed(_buyable.CostToRemove.ToString("N0"), viewer.GetViewerCoins().ToString("N0"))
                );

                return false;
            }

            Trait target = traits?.FirstOrDefault(t => TraitHelper.CompareToInput(_buyable.GetDefaultName(_pawn.gender)!, t.Label));

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (!PassesModChecks(viewer, target, worker))
            {
                return false;
            }

            _trait = target;

            return true;
        }

        private bool PassesModChecks(Viewer viewer, Trait target, ArgWorker worker)
        {
            if (RationalRomance.Active && RationalRomance.IsTraitDisabled(target.def))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.RationalRomance".LocalizeKeyed(_buyable.Name.CapitalizeFirst()));

                return false;
            }

            if (AlienRace.Active && AlienRace.IsTraitForced(_pawn, target.def.defName, target.Degree))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Kind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, _buyable.Name));

                return false;
            }

            if ((CompatRegistry.Magic?.IsClassTrait(target.def) ?? false) && !TkSettings.ClassChanges)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Class".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            return true;
        }

        public override void Execute()
        {
            if (CompatRegistry.Magic?.IsClassTrait(_trait.def) == true && TkSettings.ResetClass)
            {
                CompatRegistry.Magic.ResetClass(_pawn);
            }

            TraitHelper.RemoveTraitFromPawn(_pawn, _trait);

            Viewer.Charge(_buyable.CostToRemove, _buyable.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemoveTrait.Complete".LocalizeKeyed(_trait.LabelCap));

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.RemoveDescription".LocalizeKeyed(Viewer.username, _trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(_pawn)
            );
        }
    }
}
