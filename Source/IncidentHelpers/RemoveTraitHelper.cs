using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    internal class RemoveTraitHelper : IncidentHelperVariables
    {
        private XmlTrait buyable;
        private Pawn pawn;
        private Trait trait;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            var query = CommandParser.Parse(message, TkSettings.Prefix).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            var traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.RemoveTrait.None".Translate());
                return false;
            }

            var traitQuery = TkUtils.ShopExpansion.Traits.FirstOrDefault(t => TraitHelper.MultiCompare(t, query));

            if (traitQuery == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(query));
                return false;
            }

            if (!traitQuery.CanRemove)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.BuyTrait.RemoveDisabled".Translate(query));
                return false;
            }

            if (Viewer.GetViewerCoins() < traitQuery.RemovePrice)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.NotEnoughCoins".Translate(
                        traitQuery.RemovePrice.ToString("N0"),
                        Viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            var target = traits?.FirstOrDefault(t => TraitHelper.MultiCompare(traitQuery, t.Label));

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.RemoveTrait.Missing".Translate(query));
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

            pawn.story.traits.allTraits.Remove(trait);
            var data = trait.def.DataAtDegree(buyable.Degree);

            if (data?.skillGains != null)
            {
                foreach (var gain in data.skillGains)
                {
                    var skill = pawn.skills.GetSkill(gain.Key);
                    skill.Level -= gain.Value;
                }
            }

            Viewer.TakeViewerCoins(buyable.RemovePrice);
            Viewer.CalculateNewKarma(storeIncident.karmaType, buyable.RemovePrice);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Responses.RemoveTrait.Removed".Translate(trait.LabelCap)
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Trait.Title".Translate(),
                "TKUtils.Letters.TraitRemove.Description".Translate(Viewer.username, trait.LabelCap),
                LetterDefOf.PositiveEvent,
                new LookTargets(pawn)
            );
        }
    }
}
