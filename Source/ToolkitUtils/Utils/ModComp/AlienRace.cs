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

using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class AlienRace
    {
        public static readonly bool Active = ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null;
        private static readonly FieldInfo AlienSettingsField = AccessTools.Field("AlienRace.ThingDef_AlienRace:alienRace");
        private static readonly FieldInfo AlienGeneralSettingsField = AccessTools.Field("AlienRace.Thing_AlienRace.AlienSettings:generalSettings");
        private static readonly FieldInfo AlienGeneralSettingsForcedTraits = AccessTools.Field("AlienRace.GeneralSettings:forcedRaceTraitEntries");
        private static readonly FieldInfo TraitEntryDefName = AccessTools.Field("AlienRace.AlienTraitEntry:defName");
        private static readonly FieldInfo TraitEntryDegree = AccessTools.Field("AlienRace.AlienTraitEntry:degree");
        private static readonly FieldInfo TraitEntryChance = AccessTools.Field("AlienRace.AlienTraitEntry:chance");
        private static readonly FieldInfo AlienDisallowedTraits = AccessTools.Field("AlienRace.GeneralSettings:disallowedTraits");

        public static bool IsTraitAllowed(Pawn pawn, TraitDef traitDef, int degree = -10)
        {
            if (!Active)
            {
                return true;
            }

            try
            {
                return !(IsTraitDisallowed(pawn, traitDef.defName, degree) || IsTraitForced(pawn, traitDef.defName, degree));
            }
            catch (Exception)
            {
                return true;
            }
        }

        internal static bool IsTraitForced([NotNull] Pawn pawn, string defName, int degree)
        {
            if (!TryGetForcedTraits(pawn, out IList forcedTraits))
            {
                return false;
            }

            foreach (object item in forcedTraits)
            {
                if (!TryGetTraitEntry(item, out Tuple<string, int, float> pair))
                {
                    continue;
                }

                if (!(pair.Item1.Equals(defName) && pair.Item2.Equals(degree)))
                {
                    continue;
                }

                if (pair.Item3 == 0 || pair.Item3 >= 100f)
                {
                    return true;
                }
            }

            return false;
        }

        [ContractAnnotation("=> true,settings:notnull; => false,settings:null")]
        private static bool TryGetRaceSettings([NotNull] Pawn pawn, out object settings)
        {
            if (pawn.kindDef?.race == null)
            {
                settings = null;

                return false;
            }

            try
            {
                settings = AlienSettingsField.GetValue(pawn.kindDef.race);

                return true;
            }
            catch (ArgumentException)
            {
                // The def does not contain any unique settings
                settings = null;

                return false;
            }
        }

        [ContractAnnotation("=> true,generalSettings:notnull; => false,generalSettings:null")]
        private static bool TryGetGeneralSettings([NotNull] Pawn pawn, out object generalSettings)
        {
            if (!TryGetRaceSettings(pawn, out object settings))
            {
                generalSettings = null;

                return false;
            }

            generalSettings = AlienGeneralSettingsField.GetValue(settings);

            return true;
        }

        [ContractAnnotation("=> true,disallowedTraits:notnull; => false,disallowedTraits:null")]
        private static bool TryGetDisallowedTraits([NotNull] Pawn pawn, out IList disallowedTraits)
        {
            if (!TryGetGeneralSettings(pawn, out object settings))
            {
                disallowedTraits = null;

                return false;
            }

            disallowedTraits = AlienDisallowedTraits.GetValue(settings) as IList;

            return disallowedTraits != null;
        }

        [ContractAnnotation("=> true,forcedTraits:notnull; => false,forcedTraits:null")]
        private static bool TryGetForcedTraits([NotNull] Pawn pawn, out IList forcedTraits)
        {
            if (!TryGetGeneralSettings(pawn, out object settings))
            {
                forcedTraits = null;

                return false;
            }

            forcedTraits = AlienGeneralSettingsForcedTraits.GetValue(settings) as IList;

            return forcedTraits != null;
        }

        [ContractAnnotation("=> true,result:notnull; => false,result:null")]
        private static bool TryGetTraitEntry(object entry, out Tuple<string, int, float> result)
        {
            var defName = TraitEntryDefName.GetValue(entry) as string;
            object itemDegree = TraitEntryDegree.GetValue(entry);
            object itemChance = TraitEntryChance.GetValue(entry);
            int degree = itemDegree as int? ?? -10;
            float chance = itemChance as float? ?? 100f;

            result = new Tuple<string, int, float>(defName, degree, chance);

            return defName != null;
        }

        internal static bool IsTraitDisallowed([NotNull] Pawn pawn, string defName, int degree)
        {
            if (!TryGetDisallowedTraits(pawn, out IList disallowedTraits))
            {
                return false;
            }

            foreach (object item in disallowedTraits)
            {
                if (!TryGetTraitEntry(item, out Tuple<string, int, float> pair))
                {
                    continue;
                }

                if (pair.Item1.Equals(defName) && pair.Item2.Equals(degree))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
