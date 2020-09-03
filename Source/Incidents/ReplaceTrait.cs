using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class ReplaceTrait : IncidentHelperVariables
    {
        private Pawn pawn;
        private TraitItem replaceThatShop;

        private Trait replaceThatTrait;
        private TraitDef replaceThatTraitDef;
        private TraitItem replaceThisShop;

        private Trait replaceThisTrait;
        private TraitDef replaceThisTraitDef;

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

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

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            replaceThisShop = Data.Traits.FirstOrDefault(t => TraitHelper.CompareToInput(t, toReplace));
            replaceThatShop = Data.Traits.FirstOrDefault(t => TraitHelper.CompareToInput(t, toReplaceWith));

            if (replaceThisShop == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(toReplace));
                return false;
            }

            if (replaceThatShop == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(toReplaceWith));
                return false;
            }

            if (!replaceThisShop.CanRemove)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Disabled".Localize(replaceThisShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (!replaceThatShop.CanAdd)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Disabled".Localize(replaceThatShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (replaceThisShop.BypassLimit
                && !replaceThatShop.BypassLimit
                && pawn.story.traits.allTraits.Count > AddTraitSettings.maxTraits)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ReplaceTrait.Violation".Localize(replaceThisShop.Name, replaceThatShop.Name)
                );
                return false;
            }

            if (!Viewer.CanAfford(replaceThisShop.CostToRemove + replaceThatShop.CostToAdd))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        (replaceThisShop.CostToRemove + replaceThatShop.CostToAdd).ToString("N0"),
                        Viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            List<TraitDef> traitDefs = DefDatabase<TraitDef>.AllDefsListForReading;
            replaceThisTraitDef = traitDefs.FirstOrDefault(t => t.defName.Equals(replaceThisShop.DefName));
            replaceThatTraitDef = traitDefs.FirstOrDefault(t => t.defName.Equals(replaceThatShop.DefName));

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

            if (replaceThisTraitDef.IsDisallowedByBackstory(pawn, replaceThisShop.Degree) is { } thisBackstory)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByBackstory".Localize(thisBackstory.identifier, toReplace)
                );
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

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(replaceThisTraitDef.defName)) ?? false)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.LabelCap, toReplace)
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(replaceThatTraitDef.defName)) ?? false)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.LabelCap, toReplaceWith)
                );
                return false;
            }

            List<Trait> traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.None".Localize());
                return false;
            }

            replaceThisTrait = traits?.FirstOrDefault(t => TraitHelper.CompareToInput(replaceThisShop, t.Label));

            if (replaceThisTrait == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".Localize(toReplace));
                return false;
            }

            if (MagicComp.Active
                && (MagicComp.GetAllClasses()?.Any(c => c.defName.Equals(replaceThisTrait.def.defName)) ?? false))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Class".Localize(replaceThisTrait.Label)
                );
                return false;
            }

            if (traits?.Find(s => TraitHelper.CompareToInput(replaceThatShop, s.Label)) == null)
            {
                replaceThatTrait = new Trait(replaceThatTraitDef, replaceThatShop.Degree);
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Duplicate".Localize(toReplaceWith));
            return false;
        }

        public override void TryExecute()
        {
            if (pawn == null || replaceThisTrait == null)
            {
                return;
            }

            TraitHelper.RemoveTraitFromPawn(pawn, replaceThisTrait);

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(replaceThisShop.CostToRemove);
            }

            Viewer.CalculateNewKarma(
                replaceThisShop.Data?.KarmaTypeForRemoving ?? storeIncident.karmaType,
                replaceThisShop.CostToRemove
            );


            TraitHelper.GivePawnTrait(pawn, replaceThatTrait);

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(replaceThatShop.CostToAdd);
            }

            Viewer.CalculateNewKarma(
                replaceThatShop.Data?.KarmaTypeForAdding ?? storeIncident.karmaType,
                replaceThatShop.CostToAdd
            );


            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.ReplaceTrait.Complete".Localize(replaceThisTrait.LabelCap, replaceThatTrait.LabelCap)
                );
            }

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
