using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Store;

using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    internal class RemoveTraitHelper : IncidentHelperVariables
    {
        private BuyableTrait buyable;
        private Pawn pawn;
        private bool separateChannel;
        private Trait trait;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if(viewer == null)
            {
                return false;
            }

            Viewer = viewer;
            this.separateChannel = separateChannel;

            var query = CommandParser.Parse(message, prefix: TKSettings.Prefix).Skip(2).FirstOrDefault();

            if(query.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if(pawn == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.NoPawn".Translate(),
                    separateChannel
                );
                return false;
            }

            var traits = pawn.story.traits.allTraits;

            if(traits != null && traits.Count <= 0)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.RemoveTrait.None".Translate(),
                    separateChannel
                );
                return false;
            }

            var buyable = AllTraits.buyableTraits.Where(t => TraitHelper.MultiCompare(t, query)).FirstOrDefault();

            if(buyable == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.TraitQueryInvalid".Translate(
                        query.Named("QUERY")
                    ),
                    separateChannel
                );
                return false;
            }

            var target = traits.Where(t => TraitHelper.MultiCompare(buyable, t.Label)).FirstOrDefault();

            if(target == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.RemoveTrait.Missing".Translate(
                        query.Named("QUERY")
                    ),
                    separateChannel
                );
                return false;
            }

            this.trait = target;
            this.buyable = buyable;
            return true;
        }

        public override void TryExecute()
        {
            if(pawn == null) return;
            if(trait == null) return;

            pawn.story.traits.allTraits.Remove(trait);
            var data = trait.def.DataAtDegree(buyable.degree);

            if(data != null && data.skillGains != null)
            {
                foreach(var gain in data.skillGains)
                {
                    var skill = pawn.skills.GetSkill(gain.Key);
                    skill.Level -= gain.Value;
                }
            }

            Viewer.TakeViewerCoins(storeIncident.cost);
            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            if(ToolkitSettings.PurchaseConfirmations)
            {
                CommandBase.SendCommandMessage(
                    Viewer.username,
                    "TKUtils.Responses.RemoveTrait.Removed".Translate(
                        trait.LabelCap.Named("TRAIT")
                    ),
                    separateChannel
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Trait.Title".Translate(),
                "TKUtils.Letters.TraitRemove.Description".Translate(
                    Viewer.username.Named("VIEWER"),
                    trait.LabelCap.Named("TRAIT")
                ),
                LetterDefOf.PositiveEvent,
                new LookTargets(pawn),
                null
            );
        }
    }
}
