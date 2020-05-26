﻿using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
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

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            var segments = CommandFilter.Parse(message).Skip(2).ToArray();

            if (segments.Length < 2)
            {
                return false;
            }

            var toReplace = segments.FirstOrDefault();
            var toReplaceWith = segments.Skip(1).FirstOrDefault();

            if (toReplace.NullOrEmpty() || toReplaceWith.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
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

            if (Viewer.GetViewerCoins() < replaceThisShop.RemovePrice + replaceThatShop.AddPrice
                && !ToolkitSettings.UnlimitedCoins)
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

            var traitDefs = DefDatabase<TraitDef>.AllDefsListForReading;
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

            var traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.RemoveTrait.None".Translate());
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
            var data = replaceThisTrait.def.DataAtDegree(replaceThisTrait.Degree);

            if (data?.skillGains != null)
            {
                foreach (var gain in data.skillGains)
                {
                    var skill = pawn.skills.GetSkill(gain.Key);

                    skill.Level -= gain.Value;
                }
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(replaceThisShop.RemovePrice);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, replaceThisShop.RemovePrice);


            pawn.story.traits.GainTrait(replaceThatTrait);
            var val = replaceThatTraitDef.DataAtDegree(replaceThatShop.Degree);

            if (val?.skillGains != null)
            {
                foreach (var skillGain in val.skillGains)
                {
                    var skill = pawn.skills.GetSkill(skillGain.Key);
                    var level = TraitHelpers.FinalLevelOfSkill(pawn, skillGain.Key);

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
                "TKUtils.Letters.Trait.Title".Translate(),
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