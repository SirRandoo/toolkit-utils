using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Store;

using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class AddTraitHelper : IncidentHelperVariables
    {
        private BuyableTrait buyableTrait;
        private Pawn pawn;
        private bool separateChannel;
        private Trait trait;
        private TraitDef traitDef;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if(viewer == null)
            {
                return false;
            }

            Viewer = viewer;
            this.separateChannel = separateChannel;

            var traitQuery = CommandParser.Parse(message, TkSettings.Prefix).Skip(2).FirstOrDefault();

            if (traitQuery.NullOrEmpty())
            {
                return false;
            }

            var viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.NoPawn".Translate(),
                    separateChannel
                );
                return false;
            }

            var buyable = AllTraits.buyableTraits.Where(t => TraitHelper.MultiCompare(t, trait)).FirstOrDefault();
            var maxTraits = AddTraitSettings.maxTraits > 0 ? AddTraitSettings.maxTraits : 4;
            var traits = viewerPawn.story.traits.allTraits;

            if(traits != null)
            {
                var tally = traits.Where(t => !TraitHelper.IsSpecialTrait(t)).Count();
                var flag = buyable == null ? false : TraitHelper.IsSpecialTrait(buyable.def);

                if(tally >= maxTraits && !flag)
                {
                    CommandBase.SendCommandMessage(
                        viewer.username,
                        "TKUtils.Responses.BuyTrait.LimitReached".Translate(
                            maxTraits.Named("LIMIT")
                        ),
                        separateChannel
                    );
                    return false;
                }
            }

            if(buyable == null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.TraitQueryInvalid".Translate(
                        trait.Named("QUERY")
                    ),
                    separateChannel
                );
                return false;
            }

            var buyableDef = buyable.def;
            var traitObj = new Trait(buyableDef, buyable.degree);

            foreach (var t in viewerPawn.story.traits.allTraits.Where(
                t => t.def.ConflictsWith(traitObj) || buyableDef.ConflictsWith(t)
            ))
            {
                if(t.def.ConflictsWith(traitObj) || traitDef.ConflictsWith(t))
                {
                    CommandBase.SendCommandMessage(
                        viewer.username,
                        "TKUtils.Responses.BuyTrait.Conflicts".Translate(
                            t.LabelCap.Named("TRAIT"),
                            traitDef.defName.Named("REQUESTED")
                        ),
                        separateChannel
                    );
                    return false;
                }
            }

            if(traits != null && traits.Find(s => s.def.defName == traitObj.def.defName) != null)
            {
                CommandBase.SendCommandMessage(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.Duplicate".Translate(
                        traitObj.Label.Named("TRAIT")
                    ),
                    separateChannel
                );
                return false;
            }

            trait = traitObj;
            traitDef = buyableDef;
            buyableTrait = buyable;
            pawn = viewerPawn;

            return traitQuery != null && buyableDef != null && buyableTrait != null;
        }

        public override void TryExecute()
        {
            pawn.story.traits.GainTrait(trait);
            var val = traitDef.DataAtDegree(buyableTrait.degree);
            if(val != null && val.skillGains != null)
            {
                foreach(var skillGain in val.skillGains)
                {
                    var skill = pawn.skills.GetSkill(skillGain.Key);
                    int level = TraitHelpers.FinalLevelOfSkill(pawn, skillGain.Key);
                    skill.Level = level;
                }
            }
            Viewer.TakeViewerCoins(storeIncident.cost);
            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            if(ToolkitSettings.PurchaseConfirmations)
            {
                CommandBase.SendCommandMessage(
                    Viewer.username,
                    "TKUtils.Responses.BuyTrait.Added".Translate(
                        trait.Label.Named("TRAIT")
                    ),
                    separateChannel
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Trait.Title".Translate(),
                "TKUtils.Letters.Trait.Description".Translate(
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
