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
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class AddTrait : IncidentVariablesBase
    {
        private TraitItem _buyableTrait;
        private Pawn _pawn;
        private Trait _trait;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsTrait(out _buyableTrait) || !_buyableTrait.CanAdd || _buyableTrait.TraitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (!viewer.CanAfford(_buyableTrait!.CostToAdd))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".LocalizeKeyed(_buyableTrait.CostToAdd.ToString("N0"), viewer.GetViewerCoins().ToString("N0"))
                );

                return false;
            }

            if (TraitHelper.GetTotalTraits(_pawn) >= AddTraitSettings.maxTraits && _buyableTrait.TraitData?.CanBypassLimit != true)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.LimitReached".LocalizeKeyed(AddTraitSettings.maxTraits));

                return false;
            }

            if (!PassesCharacterChecks(viewer, worker))
            {
                return false;
            }

            _trait = new Trait(_buyableTrait.TraitDef, _buyableTrait.Degree);

            return PassesValidationChecks(viewer) && PassesModChecks(viewer);
        }

        private bool PassesModChecks(Viewer viewer)
        {
            if (CompatRegistry.Magic == null || !CompatRegistry.Magic.IsClassTrait(_buyableTrait.TraitDef!))
            {
                return true;
            }

            if (!CompatRegistry.Magic.HasClass(_pawn))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Class".Localize());

            return false;
        }

        private bool PassesValidationChecks(Viewer viewer)
        {
            Trait conflicting = _pawn.story.traits.allTraits?.Find(t => t.def.ConflictsWith(_trait) || _buyableTrait.TraitDef!.ConflictsWith(t));

            if (conflicting != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Conflict".LocalizeKeyed(conflicting.LabelCap, _trait.LabelCap));

                return false;
            }

            Trait duplicateTrait = _pawn.story.traits.allTraits?.FirstOrDefault(t => t.def.defName.Equals(_buyableTrait.TraitDef!.defName));

            if (duplicateTrait != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Duplicate".LocalizeKeyed(duplicateTrait.Label, _trait.Label));

                return false;
            }

            if (!TraitHelper.IsAdditionAllowedByGenes(_pawn, _buyableTrait.TraitDef, _buyableTrait.Degree))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.GeneSuppressed".LocalizeKeyed(_buyableTrait.Name));

                return false;
            }

            return true;
        }

        private bool PassesCharacterChecks(Viewer viewer, ArgWorker worker)
        {
            if (!TraitHelper.IsOldEnoughForTraits(_pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.TooYoung".Localize());

                return false;
            }

            if (_buyableTrait.TraitDef.IsDisallowedByBackstory(_pawn!, _buyableTrait.Degree, out BackstoryDef backstory))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.RestrictedByBackstory".LocalizeKeyed(backstory.identifier, worker.GetLast()));

                return false;
            }

            if (_pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(_buyableTrait.TraitDef!.defName)) == true)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, worker.GetLast()));

                return false;
            }

            if (!_buyableTrait.TraitDef.IsDisallowedByKind(_pawn, _buyableTrait.Degree))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, worker.GetLast()));

            return false;
        }

        public override void Execute()
        {
            TraitHelper.GivePawnTrait(_pawn, _trait);
            Viewer.Charge(_buyableTrait.CostToAdd, _buyableTrait.Data?.KarmaType ?? storeIncident.karmaType);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Trait.Complete".LocalizeKeyed(_trait.Label));

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.AddDescription".LocalizeKeyed(Viewer.username, _trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(_pawn)
            );
        }
    }
}
