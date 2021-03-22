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
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class TraitHelper
    {
        public static bool IsSexualityTrait([NotNull] this Trait trait)
        {
            return IsSexualityTrait(trait.def);
        }

        public static bool IsSexualityTrait([NotNull] this TraitDef trait)
        {
            if (trait.exclusionTags.Contains("SexualOrientation"))
            {
                return true;
            }

            return trait.Equals(TraitDefOf.Gay)
                   || trait.Equals(TraitDefOf.Bisexual)
                   || trait.Equals(TraitDefOf.Asexual);
        }

        public static bool CompareToInput([NotNull] TraitItem trait, [NotNull] string input)
        {
            return Unrichify.StripTags(trait.Name)
               .ToToolkit()
               .EqualsIgnoreCase(Unrichify.StripTags(input).StripTags().ToToolkit());
        }

        public static bool CompareToInput([NotNull] string traitName, [NotNull] string input)
        {
            return Unrichify.StripTags(traitName)
               .ToToolkit()
               .EqualsIgnoreCase(Unrichify.StripTags(input).StripTags().ToToolkit());
        }

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
                        Name = trait.label.StripTags().ToToolkit(),
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
                        Name = t.label.StripTags().ToToolkit()
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
        #if !RW11
            trait.pawn = pawn;
        #endif
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

            foreach (KeyValuePair<SkillDef, int> skillGain in val.skillGains)
            {
                SkillRecord skill = pawn.skills.GetSkill(skillGain.Key);

                if (skill.TotallyDisabled)
                {
                    continue;
                }

                skill.Level -= skillGain.Value;
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

        public static Backstory IsDisallowedByBackstory(this TraitDef trait, [NotNull] Pawn pawn, int degree)
        {
            return pawn.story.AllBackstories.FirstOrFallback(s => s.DisallowsTrait(trait, degree));
        }

        public static Backstory IsDisallowedByBackstory([NotNull] this Trait trait, [NotNull] Pawn pawn, int degree)
        {
            return trait.def.IsDisallowedByBackstory(pawn, degree);
        }

        public static bool IsDisallowedByKind(this TraitDef trait, Pawn pawn, int degree)
        {
            return AlienRace.Enabled && !AlienRace.IsTraitAllowed(pawn, trait, degree);
        }
    }
}
