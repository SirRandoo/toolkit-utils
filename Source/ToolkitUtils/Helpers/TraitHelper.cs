// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class TraitHelper
    {
        public static bool IsSexualityTrait([NotNull] this Trait trait) => IsSexualityTrait(trait.def);

        public static bool IsSexualityTrait([NotNull] this TraitDef trait)
        {
            if (trait.exclusionTags.Contains("SexualOrientation"))
            {
                return true;
            }

            return trait.Equals(TraitDefOf.Gay) || trait.Equals(TraitDefOf.Bisexual) || trait.Equals(TraitDefOf.Asexual);
        }

        public static bool CompareToInput([NotNull] TraitItem trait, [NotNull] string input) => RichTextHelper.StripTags(trait.Name)
           .ToToolkit()
           .EqualsIgnoreCase(RichTextHelper.StripTags(input).ToToolkit());

        public static bool CompareToInput([NotNull] string traitName, [NotNull] string input) => RichTextHelper.StripTags(traitName)
           .ToToolkit()
           .EqualsIgnoreCase(RichTextHelper.StripTags(input).ToToolkit());

        [NotNull]
        public static IEnumerable<TraitItem> ToTraitItems([NotNull] this TraitDef trait)
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
                        Name = ColoredText.StripTags(trait.label).ToToolkit(),
                        CostToAdd = 3500,
                        CostToRemove = 5500
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
                        Name = ColoredText.StripTags(t.label).ToToolkit()
                    }
                )
               .ToArray();
        }

        // god dammit HAR
        private static void ForciblyGivePawnTrait([NotNull] Pawn pawn, [NotNull] Trait trait)
        {
            if (pawn.story.traits.HasTrait(trait.def))
            {
                return;
            }

            pawn.story.traits.allTraits.Add(trait);
            pawn.Notify_DisabledWorkTypesChanged();
            pawn.skills?.Notify_SkillDisablesChanged();

            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood?.thoughts.situational.Notify_SituationalThoughtsDirty();
            }

            MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
        }

        public static void GivePawnTrait([NotNull] Pawn pawn, [NotNull] Trait trait)
        {
            ForciblyGivePawnTrait(pawn, trait);

            TraitDegreeData val = trait.CurrentData;

            if (val?.skillGains == null)
            {
                return;
            }

            foreach (KeyValuePair<SkillDef, int> skillGain in val.skillGains)
            {
                SkillRecord skill = pawn.skills.GetSkill(skillGain.Key);

                if (skill.TotallyDisabled)
                {
                    continue;
                }

                skill.Level += skillGain.Value;
            }

            List<WorkTypeDef> disabledWorkTypes = trait.GetDisabledWorkTypes().ToList();

            if (disabledWorkTypes.Count <= 0)
            {
                return;
            }

            pawn.Notify_DisabledWorkTypesChanged();

            foreach (WorkTypeDef workType in disabledWorkTypes.Where(workType => !pawn.WorkTypeIsDisabled(workType)))
            {
                pawn.workSettings.Disable(workType);
            }
        }

        public static void RemoveTraitFromPawn([NotNull] Pawn pawn, [NotNull] Trait trait)
        {
            pawn.story.traits.allTraits.Remove(trait);

            TraitDegreeData val = trait.CurrentData;

            if (val?.skillGains == null)
            {
                return;
            }

            foreach (KeyValuePair<SkillDef, int> pair in val.skillGains)
            {
                SkillDef skillDef = pair.Key;
                int value = pair.Value;
                SkillRecord skill = pawn.skills.GetSkill(skillDef);

                if (skill.TotallyDisabled)
                {
                    continue;
                }

                skill.Level -= value;
            }

            List<WorkTypeDef> disabledWorkTypes = trait.GetDisabledWorkTypes().ToList();

            if (disabledWorkTypes.Count <= 0)
            {
                return;
            }

            pawn.Notify_DisabledWorkTypesChanged();

            foreach (WorkTypeDef workType in disabledWorkTypes)
            {
                pawn.workSettings.SetPriority(workType, 3);
            }
        }

        [ContractAnnotation("trait:notnull,pawn:notnull => true,backstory:notnull; trait:notnull,pawn:notnull => false,backstory:null")]
        public static bool IsDisallowedByBackstory(this TraitDef trait, Pawn pawn, int degree, out BackstoryDef backstory)
        {
            backstory = pawn.story.AllBackstories.FirstOrFallback(s => s.DisallowsTrait(trait, degree));

            return backstory != null;
        }

        [ContractAnnotation("trait:notnull,pawn:notnull => true,backstory:notnull; trait:notnull,pawn:notnull => false,backstory:null")]
        public static bool IsDisallowedByBackstory(this Trait trait, Pawn pawn, int degree, out BackstoryDef backstory) =>
            IsDisallowedByBackstory(trait.def, pawn, degree, out backstory);

        public static bool IsDisallowedByKind(this TraitDef trait, Pawn pawn, int degree) =>
            CompatRegistry.Alien != null && !CompatRegistry.Alien.IsTraitAllowed(pawn, trait, degree);

        public static int GetTotalTraits([NotNull] Pawn pawn)
        {
            if (pawn.story.traits?.allTraits?.NullOrEmpty() == true)
            {
                return 0;
            }

            var total = 0;
            List<Trait> traits = pawn.story.traits!.allTraits!;

            for (var index = 0; index < pawn.story.traits!.allTraits!.Count; index++)
            {
                Trait trait = traits![index];
                TraitItem item = Data.Traits.Find(t => t.TraitDef == trait.def && t.Degree == trait.Degree);

                if (item?.TraitData == null)
                {
                    total += 1;
                }
                else
                {
                    total += item.TraitData?.CanBypassLimit == true ? 0 : 1;
                }
            }

            return total;
        }

        public static bool IsRemovalAllowedByGenes(Pawn pawn, TraitDef trait, int degree)
        {
            if (!ModLister.BiotechInstalled)
            {
                return true;
            }

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                Gene target = gene.Overridden ? gene.overriddenByGene : gene;
                
                if (!target.Active || target.def.forcedTraits == null)
                {
                    continue;
                }

                GeneticTraitData geneTrait = target.def.forcedTraits.Find(g => g.def == trait && g.degree == degree);

                if (geneTrait != null)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsAdditionAllowedByGenes(Pawn pawn, TraitDef trait, int degree)
        {
            if (!ModLister.BiotechInstalled)
            {
                return true;
            }

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                Gene target = gene.Overridden ? gene.overriddenByGene : gene;
                
                if (!target.Active || target.def.suppressedTraits == null)
                {
                    continue;
                }

                GeneticTraitData geneTrait = target.def.suppressedTraits.Find(g => g.def == trait && g.degree == degree);

                if (geneTrait != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
