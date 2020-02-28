using System.Linq;

using HtmlAgilityPack;

using RimWorld;

using SirRandoo.ToolkitUtils.Commands;

using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Traits;

using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class AddTraitHelper : AddTrait
    {
        private BuyableTrait buyableTrait;
        private Pawn pawn;
        private bool separateChannel;
        private Trait trait;
        private TraitDef traitDef;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            this.separateChannel = separateChannel;
            Viewer = viewer;

            var segments = message.Split(' ');

            if(segments.Length < 3)
            {
                CommandBase.SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        viewer.username.Named("VIEWER"),
                        "TKUtils.Responses.NoTrait".Translate(storeIncident.syntax.Named("SYNTAX")).Named("MESSAGE")
                    ).Replace("!Buy", "!buy"),
                    separateChannel
                );
                return false;
            }

            var pawn = CommandBase.GetPawn(viewer.username);

            if(pawn == null)
            {
                CommandBase.SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        viewer.username.Named("VIEWER"),
                        "TKUtils.Responses.NoPawn".Translate().Named("MESSAGE")
                    ),
                    separateChannel
                );
                return false;
            }

            var input = segments[2].ToLower();
            var buyable = AllTraits.buyableTraits.Where(t => MultiCompare(t, input)).FirstOrDefault();

            var maxTraits = AddTraitSettings.maxTraits > 0 ? AddTraitSettings.maxTraits : 4;
            var traits = pawn.story.traits.allTraits;

            if(traits != null)
            {
                var tally = traits.Where(t => !IsSpecialTrait(t)).Count();
                var flag = buyable == null ? false : IsSpecialTrait(buyable.def);

                //Log.Message($"TKUtils :: Flag:{flag},Tally:{tally},Max:{maxTraits}");

                if(tally >= maxTraits && !flag)
                {
                    CommandBase.SendMessage(
                        "TKUtils.Responses.Format".Translate(
                            viewer.username.Named("VIEWER"),
                            "TKUtils.Responses.TraitLimitReached".Translate(maxTraits.Named("LIMIT")).Named("MESSAGE")
                        ),
                        separateChannel
                    );
                    return false;
                }
            }

            if(buyable == null)
            {
                CommandBase.SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        viewer.username.Named("VIEWER"),
                        "TKUtils.Responses.NoTraitFound".Translate(
                            input.Named("TRAIT")
                        ).Named("MESSAGE")
                    ),
                    separateChannel
                );

                return false;
            }

            var traitDef = buyable.def;
            var trait = new Trait(traitDef, degree: buyable.degree, forced: false);

            foreach(var t in pawn.story.traits.allTraits)
            {
                if(t.def.ConflictsWith(trait) || traitDef.ConflictsWith(t))
                {
                    CommandBase.SendMessage(
                        "TKUtils.Responses.Format".Translate(
                            viewer.username.Named("VIEWER"),
                            "TKUtils.Responses.TraitConflict".Translate(
                                traitDef.defName.Named("TRAIT"),
                                t.LabelCap.Named("CONFLICT")
                            ).Named("MESSAGE")
                        ),
                        separateChannel
                    );

                    return false;
                }
            }

            if(traits != null && traits.Find(s => s.def.defName == trait.def.defName) != null)
            {
                CommandBase.SendMessage(
                    "TKUtils.Responses.HasTrait".Translate(
                        viewer.username.Named("VIEWER"),
                        "TKUtils.Responses.HasTrait".Translate(
                            trait.Label.Named("TRAIT")
                        ).Named("MESSAGE")
                    ),
                    separateChannel
                );

                return false;
            }

            this.trait = trait;
            this.traitDef = traitDef;
            this.buyableTrait = buyable;
            this.pawn = pawn;

            return trait != null && traitDef != null && buyableTrait != null;
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

            CommandBase.SendMessage(
                "TKUtils.Responses.Format".Translate(
                    Viewer.username.Named("VIEWER"),
                    "TKUtils.Responses.TraitAdded".Translate(
                        trait.Label.Named("TRAIT")
                    ).Named("MESSAGE")
                ),
                separateChannel
            );

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

        private bool IsSpecialTrait(Trait trait)
        {
            if(trait.def.Equals(TraitDefOf.Gay)) return true;
            if(trait.def.Equals(TraitDefOf.Bisexual)) return true;

            return false;
        }

        private bool IsSpecialTrait(TraitDef trait)
        {
            if(trait.Equals(TraitDefOf.Gay)) return true;
            if(trait.Equals(TraitDefOf.Bisexual)) return true;

            return false;
        }

        private bool MultiCompare(BuyableTrait trait, string input)
        {
            var label = trait.label;

            if(input.Equals(label)) return true;

            if(trait.label.Contains('<') || trait.label.Contains('>'))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(trait.label);

                if(doc.DocumentNode.InnerText.Equals(input)) return true;
            }

            return false;
        }
    }
}
