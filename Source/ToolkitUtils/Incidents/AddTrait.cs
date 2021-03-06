﻿// ToolkitUtils
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
    [UsedImplicitly]
    public class AddTrait : IncidentVariablesBase
    {
        private TraitItem buyableTrait;
        private Pawn pawn;
        private Trait trait;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsTrait(out buyableTrait)
                || buyableTrait.CanAdd == false
                || buyableTrait.TraitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            if (!viewer.CanAfford(buyableTrait!.CostToAdd))
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

            int total = pawn!.story?.traits?.allTraits?.Count ?? 0;

            if (total >= AddTraitSettings.maxTraits && buyableTrait.TraitData?.CanBypassLimit == false)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.LimitReached".LocalizeKeyed(AddTraitSettings.maxTraits)
                );
                return false;
            }

            if (!PassesCharacterChecks(viewer, worker))
            {
                return false;
            }

            trait = new Trait(buyableTrait.TraitDef, buyableTrait.Degree);

            return PassesValidationChecks(viewer) && PassesModChecks(viewer);
        }

        private bool PassesModChecks(Viewer viewer)
        {
            if (CompatRegistry.Magic == null || !CompatRegistry.Magic.IsClassTrait(buyableTrait.TraitDef!))
            {
                return true;
            }

            if (!CompatRegistry.Magic.HasClass(pawn))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Class".Localize());
            return false;
        }

        private bool PassesValidationChecks(Viewer viewer)
        {
            if (pawn.story.traits.allTraits != null)
            {
                foreach (Trait t in pawn.story.traits.allTraits.Where(
                    t => t.def.ConflictsWith(trait) || buyableTrait.TraitDef!.ConflictsWith(t)
                ))
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Trait.Conflict".LocalizeKeyed(t.LabelCap, trait.LabelCap)
                    );
                    return false;
                }
            }

            Trait duplicateTrait =
                pawn.story.traits.allTraits?.FirstOrDefault(t => t.def.defName.Equals(buyableTrait.TraitDef!.defName));
            if (duplicateTrait != null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Duplicate".LocalizeKeyed(duplicateTrait.Label, trait.Label)
                );
                return false;
            }

            return true;
        }

        private bool PassesCharacterChecks(Viewer viewer, ArgWorker worker)
        {
            if (buyableTrait.TraitDef.IsDisallowedByBackstory(pawn!, buyableTrait.Degree, out Backstory backstory))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByBackstory".LocalizeKeyed(backstory.identifier, worker.GetLast())
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(buyableTrait.TraitDef!.defName)) == true)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(pawn.kindDef.race.LabelCap, worker.GetLast())
                );
                return false;
            }

            if (!buyableTrait.TraitDef.IsDisallowedByKind(pawn, buyableTrait.Degree))
            {
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(pawn.kindDef.race.LabelCap, worker.GetLast())
            );
            return false;
        }

        public override void Execute()
        {
            TraitHelper.GivePawnTrait(pawn, trait);
            Viewer.Charge(buyableTrait.CostToAdd, buyableTrait.Data?.KarmaType ?? storeIncident.karmaType);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Trait.Complete".LocalizeKeyed(trait.Label));

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.AddDescription".LocalizeKeyed(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}
