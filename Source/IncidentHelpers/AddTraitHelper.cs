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
        private XmlTrait buyableTrait;
        private Pawn pawn;
        private Trait trait;
        private TraitDef traitDef;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            var traitQuery = CommandParser.Parse(message, TkSettings.Prefix).Skip(2).FirstOrDefault();

            if (traitQuery.NullOrEmpty())
            {
                return false;
            }

            var viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            var buyable = TkUtils.ShopExpansion.Traits.FirstOrDefault(t => TraitHelper.MultiCompare(t, traitQuery));
            var maxTraits = AddTraitSettings.maxTraits > 0 ? AddTraitSettings.maxTraits : 4;
            var traits = viewerPawn.story.traits.allTraits;

            if (buyable == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(traitQuery));
                return false;
            }

            if (!buyable.Enabled)
            {
                return false;
            }

            if (Viewer.GetViewerCoins() < buyable.AddPrice)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.NotEnoughCoins".Translate(
                        buyable.AddPrice.ToString("N0"),
                        Viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            if (traits != null)
            {
                var tally = traits.Count(t => !TraitHelper.IsSexualityTrait(t));
                var canBypassLimit = buyable.BypassLimit;

                if (tally >= maxTraits && !canBypassLimit)
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Responses.BuyTrait.LimitReached".Translate(maxTraits)
                    );
                    return false;
                }
            }

            var def = DefDatabase<TraitDef>.AllDefsListForReading.FirstOrDefault(
                t => t.defName.Equals(buyable.DefName)
            );

            if (def == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(traitQuery));
                return false;
            }

            var traitObj = new Trait(def, buyable.Degree);

            foreach (var t in viewerPawn.story.traits.allTraits.Where(
                t => t.def.ConflictsWith(traitObj) || def.ConflictsWith(t)
            ))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.Conflicts".Translate(t.LabelCap, def.defName)
                );
                return false;
            }

            if (traits?.Find(s => s.def.defName == traitObj.def.defName) != null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.Duplicate".Translate(traitObj.Label)
                );
                return false;
            }

            trait = traitObj;
            traitDef = def;
            buyableTrait = buyable;
            pawn = viewerPawn;

            return traitQuery != null && buyableTrait != null;
        }

        public override void TryExecute()
        {
            pawn.story.traits.GainTrait(trait);
            var val = traitDef.DataAtDegree(buyableTrait.Degree);
            if (val?.skillGains != null)
            {
                foreach (var skillGain in val.skillGains)
                {
                    var skill = pawn.skills.GetSkill(skillGain.Key);
                    var level = TraitHelpers.FinalLevelOfSkill(pawn, skillGain.Key);
                    skill.Level = level;
                }
            }

            Viewer.TakeViewerCoins(buyableTrait.AddPrice);
            Viewer.CalculateNewKarma(storeIncident.karmaType, buyableTrait.AddPrice);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.BuyTrait.Added".Translate(trait.Label));
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Trait.Title".Translate(),
                "TKUtils.Letters.Trait.Description".Translate(Viewer.username, trait.LabelCap),
                LetterDefOf.PositiveEvent,
                new LookTargets(pawn)
            );
        }
    }
}
