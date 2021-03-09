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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class RemoveTrait : IncidentHelperVariables
    {
        private TraitItem buyable;
        private Pawn pawn;
        private Trait trait;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            string query = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            List<Trait> traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.None".Localize());
                return false;
            }

            if (!Data.TryGetTrait(query, out TraitItem traitQuery))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(query));
                return false;
            }

            if (!traitQuery.CanRemove)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Disabled".Localize(query));
                return false;
            }

            if (!viewer.CanAfford(traitQuery.CostToRemove))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        traitQuery.CostToRemove.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            Trait target =
                traits?.FirstOrDefault(t => TraitHelper.CompareToInput(traitQuery.GetDefaultName(), t.Label));

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".Localize(query));
                return false;
            }

            if (RationalRomance.Active && RationalRomance.IsTraitDisabled(target.def))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.RationalRomance".Localize(traitQuery.Name.CapitalizeFirst())
                );
                return false;
            }

            if (AlienRace.Enabled && AlienRace.IsTraitForced(pawn, target.def.defName, target.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Kind".Localize(pawn.kindDef.race.LabelCap, traitQuery.Name)
                );
                return false;
            }

            if (MagicComp.Active
                && (MagicComp.GetAllClasses()?.Any(c => c.defName.Equals(target.def.defName)) ?? false)
                && !TkSettings.ClassChanges)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Class".Localize(query));
                return false;
            }

            trait = target;
            buyable = traitQuery;
            return true;
        }

        public override void TryExecute()
        {
            if (MagicComp.Active
                && TkSettings.ClassChanges
                && (MagicComp.GetAllClasses()?.Any(c => c.defName.Equals(trait.def.defName)) ?? false))
            {
                CharacterData characterData = MagicComp.GetCharacterData(pawn);
                TraitHelper.RemoveTraitFromPawn(pawn, trait);
                characterData?.Reset();
            }
            else
            {
                TraitHelper.RemoveTraitFromPawn(pawn, trait);
            }

            Viewer.Charge(buyable.CostToRemove, buyable.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemoveTrait.Complete".Localize(trait.LabelCap));

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.RemoveDescription".Localize(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}
