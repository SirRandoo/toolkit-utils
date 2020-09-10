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
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RemoveTrait : IncidentHelperVariables
    {
        private TraitItem buyable;
        private Pawn pawn;
        private Trait trait;
        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            string query = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
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

            if (!Viewer.CanAfford(traitQuery.CostToRemove))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        traitQuery.CostToRemove.ToString("N0"),
                        Viewer.GetViewerCoins().ToString("N0")
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

            if (AlienRace.Enabled && AlienRace.IsTraitForced(pawn, target.def.defName, target.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Kind".Localize(pawn.kindDef.LabelCap, traitQuery.Name)
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
            if (pawn == null || trait == null)
            {
                return;
            }

            TraitHelper.RemoveTraitFromPawn(pawn, trait);

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(buyable.CostToRemove);
            }

            Viewer.CalculateNewKarma(
                buyable.Data?.KarmaTypeForRemoving ?? storeIncident.karmaType,
                buyable.CostToRemove
            );

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemoveTrait.Complete".Localize(trait.LabelCap));
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.RemoveDescription".Localize(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}
