using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    public class ReplaceTraitHelper : IncidentHelperVariables
    {
        private Pawn pawn;
        private XmlTrait replaceThatShop;

        private Trait replaceThatTrait;
        private TraitDef replaceThatTraitDef;
        private XmlTrait replaceThisShop;

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
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".TranslateSimple());
                return false;
            }

            replaceThisShop = TkUtils.ShopExpansion.Traits.FirstOrDefault(t => TraitHelper.MultiCompare(t, toReplace));
            replaceThatShop = TkUtils.ShopExpansion.Traits
                .FirstOrDefault(t => TraitHelper.MultiCompare(t, toReplaceWith));

            if (replaceThisShop == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(toReplace));
                return false;
            }

            if (replaceThatShop == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.TraitQueryInvalid".Translate(toReplaceWith)
                );
                return false;
            }

            if (!replaceThisShop.CanRemove)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.RemoveDisabled".Translate(replaceThisShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (!replaceThatShop.CanAdd)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.AddDisabled".Translate(replaceThatShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (replaceThisShop.BypassLimit
                && !replaceThatShop.BypassLimit
                && pawn.story.traits.allTraits.Count > AddTraitSettings.maxTraits)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.ReplaceTrait.Violation".Translate(replaceThisShop.Name, replaceThatShop.Name)
                );
                return false;
            }

            if (!Viewer.CanAfford(replaceThisShop.RemovePrice + replaceThatShop.AddPrice))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.NotEnoughCoins".Translate(
                        (replaceThisShop.RemovePrice + replaceThatShop.AddPrice).ToString("N0"),
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
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(toReplace));
                return false;
            }

            if (replaceThatTraitDef == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.TraitQueryInvalid".Translate(toReplaceWith)
                );
                return false;
            }

            foreach (Backstory backstory in pawn.story.AllBackstories)
            {
                if (backstory.DisallowsTrait(replaceThisTraitDef, replaceThisShop.Degree))
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Responses.BuyTrait.Backstory".Translate(backstory.identifier, toReplace)
                    );
                    return false;
                }

                if (backstory.DisallowsTrait(replaceThatTraitDef, replaceThatShop.Degree))
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Responses.BuyTrait.Backstory".Translate(backstory.identifier, toReplaceWith)
                    );
                    return false;
                }
            }

            if (pawn.kindDef.disallowedTraits.Any(t => t.defName.Equals(replaceThisTraitDef.defName)))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.Race".Translate(pawn.kindDef.LabelCap, toReplace)
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits.Any(t => t.defName.Equals(replaceThatTraitDef.defName)))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.Race".Translate(pawn.kindDef.LabelCap, toReplaceWith)
                );
                return false;
            }

            List<Trait> traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.RemoveTrait.None".TranslateSimple());
                return false;
            }

            replaceThisTrait = traits?.FirstOrDefault(t => TraitHelper.MultiCompare(replaceThisShop, t.Label));

            if (replaceThisTrait == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.RemoveTrait.Missing".Translate(toReplace)
                );
                return false;
            }

            if (traits?.Find(s => TraitHelper.MultiCompare(replaceThatShop, s.Label)) == null)
            {
                replaceThatTrait = new Trait(replaceThatTraitDef, replaceThatShop.Degree);
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Responses.BuyTrait.Duplicate".Translate(toReplaceWith)
            );
            return false;
        }

        public override void TryExecute()
        {
            if (pawn == null || replaceThisTrait == null)
            {
                return;
            }

            pawn.story.traits.allTraits.Remove(replaceThisTrait);
            TraitDegreeData data = replaceThisTrait.def.DataAtDegree(replaceThisTrait.Degree);

            if (data?.skillGains != null)
            {
                foreach (KeyValuePair<SkillDef, int> gain in data.skillGains)
                {
                    SkillRecord skill = pawn.skills.GetSkill(gain.Key);

                    skill.Level -= gain.Value;
                }
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(replaceThisShop.RemovePrice);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, replaceThisShop.RemovePrice);


            pawn.story.traits.GainTrait(replaceThatTrait);
            TraitDegreeData val = replaceThatTraitDef.DataAtDegree(replaceThatShop.Degree);

            if (val?.skillGains != null)
            {
                foreach (KeyValuePair<SkillDef, int> skillGain in val.skillGains)
                {
                    SkillRecord skill = pawn.skills.GetSkill(skillGain.Key);
                    int level = TraitHelpers.FinalLevelOfSkill(pawn, skillGain.Key);

                    skill.Level = level;
                }
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(replaceThatShop.AddPrice);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, replaceThatShop.AddPrice);


            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Responses.ReplaceTrait.Done".Translate(
                        replaceThisTrait.LabelCap,
                        replaceThatTrait.LabelCap
                    )
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Trait.Title".TranslateSimple(),
                "TKUtils.Letters.TraitReplace.Description".Translate(
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
