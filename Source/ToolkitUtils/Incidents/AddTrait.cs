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
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class AddTrait : IncidentVariablesBase
    {
        private TraitItem buyableTrait;
        private Pawn pawn;
        private Trait trait;
        private TraitDef traitDef;
        public override Viewer Viewer { get; set; }

        public override bool CanHappen(string msg, Viewer viewer)
        {
            string traitQuery = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (traitQuery.NullOrEmpty())
            {
                return false;
            }

            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            int maxTraits = AddTraitSettings.maxTraits > 0 ? AddTraitSettings.maxTraits : 4;
            List<Trait> traits = pawn!.story.traits.allTraits;

            if (!Data.TryGetTrait(traitQuery, out buyableTrait))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(traitQuery));
                return false;
            }

            if (!buyableTrait!.CanAdd)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Disabled".Localize(buyableTrait.Name.CapitalizeFirst())
                );
                return false;
            }

            if (!viewer.CanAfford(buyableTrait.CostToAdd))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".LocalizeKeyed(
                        buyableTrait.CostToAdd.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            if (traits != null)
            {
                int tally = traits.Count(t => !t.IsSexualityTrait());
                bool canBypassLimit = buyableTrait.BypassLimit;

                if (tally >= maxTraits && !canBypassLimit)
                {
                    MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.LimitReached".LocalizeKeyed(maxTraits));
                    return false;
                }
            }

            traitDef = buyableTrait.TraitDef;

            if (traitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(traitQuery));
                return false;
            }

            if (traitDef.IsDisallowedByBackstory(pawn, buyableTrait.Degree) is { } backstory)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByBackstory".LocalizeKeyed(backstory.identifier, traitQuery)
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(traitDef.defName)) ?? false)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(pawn.kindDef.race.LabelCap, traitQuery)
                );
                return false;
            }

            if (traitDef.IsDisallowedByKind(pawn, buyableTrait.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(pawn.kindDef.race.LabelCap, traitQuery)
                );
                return false;
            }

            trait = new Trait(traitDef, buyableTrait.Degree);

            if (traits != null)
            {
                foreach (Trait t in traits.Where(t => t.def.ConflictsWith(trait) || traitDef.ConflictsWith(t)))
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Trait.Conflict".LocalizeKeyed(t.LabelCap, trait.LabelCap)
                    );
                    return false;
                }
            }

            Trait duplicateTrait = traits?.FirstOrDefault(t => t.def.defName.Equals(traitDef.defName));
            if (duplicateTrait != null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Duplicate".LocalizeKeyed(duplicateTrait.Label, trait.Label)
                );
                return false;
            }

            if (CompatRegistry.Magic == null || !CompatRegistry.Magic.IsClassTrait(traitDef))
            {
                return traitQuery != null && buyableTrait != null;
            }

            if (!CompatRegistry.Magic.HasClass(pawn))
            {
                return traitQuery != null && buyableTrait != null;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Class".Localize());
            return false;
        }

        public override void Execute()
        {
            TraitHelper.GivePawnTrait(pawn, trait);
            Viewer.Charge(buyableTrait.CostToAdd, buyableTrait.Data?.KarmaType ?? storeIncident.karmaType);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Trait.Complete".Localize(trait.Label));

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.AddDescription".LocalizeKeyed(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}
