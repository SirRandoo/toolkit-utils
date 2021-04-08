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
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class ReplaceTrait : IncidentVariablesBase
    {
        private Pawn pawn;
        private TraitItem replaceThatShop;

        private Trait replaceThatTrait;
        private TraitDef replaceThatTraitDef;
        private TraitItem replaceThisShop;

        private Trait replaceThisTrait;
        private TraitDef replaceThisTraitDef;

        public override Viewer Viewer { get; set; }

        public override bool CanHappen(string msg, Viewer viewer)
        {
            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();

            if (segments.Length < 2)
            {
                return false;
            }

            string toReplace = segments.FirstOrDefault();
            string toReplaceWith = segments.Skip(1).FirstOrDefault();

            if (toReplace.NullOrEmpty() || toReplaceWith.NullOrEmpty())
            {
                return false;
            }

            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            if (!Data.TryGetTrait(toReplace, out replaceThisShop))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(toReplace));
                return false;
            }

            if (!Data.TryGetTrait(toReplaceWith, out replaceThatShop))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(toReplaceWith));
                return false;
            }

            if (!replaceThisShop!.CanRemove)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Disabled".Localize(replaceThisShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (!replaceThatShop!.CanAdd)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Disabled".Localize(replaceThatShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (pawn!.story.traits.allTraits.Count > AddTraitSettings.maxTraits
                && (replaceThisShop.BypassLimit && !replaceThatShop.BypassLimit
                    || !replaceThisShop.BypassLimit && !replaceThatShop.BypassLimit))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ReplaceTrait.Violation".Localize(replaceThisShop.Name, replaceThatShop.Name)
                );
                return false;
            }

            if (!viewer.CanAfford(replaceThisShop.CostToRemove + replaceThatShop.CostToAdd))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        (replaceThisShop.CostToRemove + replaceThatShop.CostToAdd).ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            replaceThisTraitDef = replaceThisShop.TraitDef;
            replaceThatTraitDef = replaceThatShop.TraitDef;

            if (replaceThisTraitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(toReplace));
                return false;
            }

            if (replaceThatTraitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(toReplaceWith));
                return false;
            }

            if (replaceThatTraitDef.IsDisallowedByBackstory(pawn, replaceThatShop.Degree) is { } thatBackstory)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByBackstory".Localize(thatBackstory.identifier, toReplaceWith)
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(replaceThatTraitDef.defName)) ?? false)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.race.LabelCap, toReplaceWith)
                );
                return false;
            }

            if (RationalRomance.Active
                && RationalRomance.IsTraitDisabled(replaceThisTraitDef)
                && !RationalRomance.IsTraitDisabled(replaceThatTraitDef))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ReplaceTrait.RationalRomance".Localize(replaceThisShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (AlienRace.Enabled && AlienRace.IsTraitForced(pawn, replaceThisShop.DefName, replaceThisShop.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Kind".Localize(pawn.kindDef.race.LabelCap, replaceThisShop.Name)
                );
                return false;
            }

            if (replaceThatTraitDef.IsDisallowedByKind(pawn, replaceThatShop.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.race.LabelCap, replaceThatShop.Name)
                );
                return false;
            }

            List<Trait> traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.None".Localize());
                return false;
            }

            replaceThisTrait = traits?.FirstOrDefault(
                t => TraitHelper.CompareToInput(replaceThisShop.GetDefaultName()!, t.Label)
            );

            if (replaceThisTrait == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".Localize(toReplace));
                return false;
            }

            if ((CompatRegistry.Magic?.IsClassTrait(replaceThisTraitDef) ?? false) && !TkSettings.ClassChanges)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Class".Localize(replaceThisTrait.Label)
                );
                return false;
            }

            if (traits?.Find(s => TraitHelper.CompareToInput(replaceThatShop.GetDefaultName()!, s.Label)) == null)
            {
                replaceThatTrait = new Trait(replaceThatTraitDef, replaceThatShop.Degree);
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Duplicate".Localize(toReplaceWith));
            return false;
        }

        public override void Execute()
        {
            if ((CompatRegistry.Magic?.IsClassTrait(replaceThisTraitDef) ?? false) && TkSettings.ResetClass)
            {
                CompatRegistry.Magic.ResetClass(pawn);
            }

            TraitHelper.RemoveTraitFromPawn(pawn, replaceThisTrait);

            Viewer.Charge(
                replaceThisShop.CostToRemove,
                replaceThisShop.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType
            );


            TraitHelper.GivePawnTrait(pawn, replaceThatTrait);

            Viewer.Charge(replaceThatShop.CostToAdd, replaceThatShop.Data?.KarmaType ?? storeIncident.karmaType);

            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.ReplaceTrait.Complete".Localize(replaceThisTrait.LabelCap, replaceThatTrait.LabelCap)
            );

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.ReplaceDescription".Localize(
                    Viewer.username,
                    replaceThisTrait.LabelCap,
                    replaceThatTrait.LabelCap
                ),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}
