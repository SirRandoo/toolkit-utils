using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.IncidentHelpers.Traits;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class TraitHelper
    {
        public static bool IsSexualityTrait(this Trait trait)
        {
            return IsSexualityTrait(trait.def);
        }

        public static bool IsSexualityTrait(this TraitDef trait)
        {
            if (trait.exclusionTags.Contains("SexualOrientation"))
            {
                return true;
            }

            return trait.Equals(TraitDefOf.Gay)
                   || trait.Equals(TraitDefOf.Bisexual)
                   || trait.Equals(TraitDefOf.Asexual);
        }

        public static bool CompareToInput(TraitItem trait, string input)
        {
            return Unrichify.StripTags(trait.Name)
               .ToToolkit()
               .EqualsIgnoreCase(Unrichify.StripTags(input).StripTags().ToToolkit());
        }

        public static IEnumerable<TraitItem> ToTraitItems(this TraitDef trait)
        {
            if (trait.degreeDatas == null)
            {
                return new[]
                {
                    new TraitItem
                    {
                        DefName = trait.defName,
                        Degree = 0,
                        CanAdd = true,
                        CanRemove = true,
                        Name = trait.label.StripTags().ToToolkit(),
                        CostToAdd = 3500,
                        CostToRemove = 5500,
                        Data = new TraitData()
                    }
                };
            }

            return trait.degreeDatas.Select(
                    t => new TraitItem
                    {
                        DefName = trait.defName,
                        Degree = t.degree,
                        CostToAdd = 3500,
                        CostToRemove = 5500,
                        CanAdd = true,
                        CanRemove = true,
                        Name = t.label.StripTags().ToToolkit(),
                        Data = new TraitData()
                    }
                )
               .ToArray();
        }

        public static void GivePawnTrait(Pawn pawn, Trait trait)
        {
            pawn.story.traits.GainTrait(trait);

            TraitDegreeData val = trait.CurrentData;

            if (val?.skillGains == null)
            {
                return;
            }

            foreach (KeyValuePair<SkillDef, int> skillGain in val.skillGains)
            {
                pawn.skills.GetSkill(skillGain.Key).Level = TraitHelpers.FinalLevelOfSkill(pawn, skillGain.Key);
            }

            List<WorkTypeDef> disabledWorkTypes = trait.GetDisabledWorkTypes().ToList();

            if (disabledWorkTypes.Count <= 0)
            {
                return;
            }

            foreach (WorkTypeDef workType in disabledWorkTypes.Where(workType => !pawn.WorkTypeIsDisabled(workType)))
            {
                pawn.workSettings.Disable(workType);
            }

            pawn.Notify_DisabledWorkTypesChanged();
        }

        public static void RemoveTraitFromPawn(Pawn pawn, Trait trait)
        {
            pawn.story.traits.allTraits.Remove(trait);

            TraitDegreeData val = trait.CurrentData;

            if (val?.skillGains == null)
            {
                return;
            }

            foreach (KeyValuePair<SkillDef, int> skillGain in val.skillGains)
            {
                pawn.skills.GetSkill(skillGain.Key).Level -= skillGain.Value;
            }

            List<WorkTypeDef> disabledWorkTypes = trait.GetDisabledWorkTypes().ToList();

            if (disabledWorkTypes.Count <= 0)
            {
                return;
            }

            foreach (WorkTypeDef workType in disabledWorkTypes.Where(pawn.WorkTypeIsDisabled))
            {
                pawn.workSettings.SetPriority(workType, 3);
            }

            pawn.Notify_DisabledWorkTypesChanged();
        }
    }
}
