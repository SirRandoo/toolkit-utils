using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class AlienRace
    {
        public static bool Enabled;
        private static readonly FieldInfo AlienSettingsField;
        private static readonly FieldInfo AlienGeneralSettingsField;
        private static readonly FieldInfo AlienGeneralSettingsForcedTraits;
        private static readonly FieldInfo TraitEntryDefName;
        private static readonly FieldInfo TraitEntryDegree;
        private static readonly FieldInfo AlienDisallowedTraits;

        static AlienRace()
        {
            foreach (Mod handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("erdelf.HumanoidAlienRaces")
            ))
            {
                try
                {
                    Assembly assembly = handle.GetType().Assembly;

                    Type alienDef = assembly.GetType("AlienRace.ThingDef_AlienRace", false);
                    AlienSettingsField = alienDef.GetField("alienRace");
                    Type alienSettingsType = assembly.GetType("AlienRace.ThingDef_AlienRace.AlienSettings", false);
                    AlienGeneralSettingsField = alienSettingsType.GetField("generalSettings");
                    Type alienGeneralSettingsType = assembly.GetType("AlienRace.GeneralSettings", false);
                    Type alienTraitEntry = assembly.GetType("AlienRace.AlienTraitEntry", false);
                    AlienGeneralSettingsForcedTraits = alienGeneralSettingsType.GetField("forcedRaceTraitEntries");
                    TraitEntryDefName = alienTraitEntry.GetField("defName");
                    TraitEntryDegree = alienTraitEntry.GetField("degree");
                    AlienDisallowedTraits = alienGeneralSettingsType.GetField("disallowedTraits");
                    Enabled = true;
                }
                catch (Exception e)
                {
                    TkLogger.Error("Compatibility class for Humanoid Alien Races failed!", e);
                }
            }
        }

        public static bool IsTraitAllowed(Pawn pawn, TraitDef traitDef, int degree = -10)
        {
            if (!Enabled)
            {
                return true;
            }

            try
            {
                return !(IsTraitDisallowed(pawn, traitDef.defName, degree)
                         || IsTraitForced(pawn, traitDef.defName, degree));
            }
            catch (Exception)
            {
                return true;
            }
        }

        private static bool IsTraitForced(Pawn pawn, string defName, int degree)
        {
            if (!TryGetForcedTraits(pawn, out IList forcedTraits))
            {
                return false;
            }

            foreach (object item in forcedTraits)
            {
                if (!TryGetTraitEntry(item, out Tuple<string, int> pair))
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

        private static bool TryGetRaceSettings(Pawn pawn, out object settings)
        {
            if (pawn.kindDef?.race == null)
            {
                settings = null;
                return false;
            }

            settings = AlienSettingsField.GetValue(pawn.kindDef.race);
            return true;
        }

        private static bool TryGetGeneralSettings(Pawn pawn, out object generalSettings)
        {
            if (!TryGetRaceSettings(pawn, out object settings))
            {
                generalSettings = null;
                return false;
            }

            generalSettings = AlienGeneralSettingsField.GetValue(settings);
            return true;
        }

        private static bool TryGetDisallowedTraits(Pawn pawn, out IList disallowedTraits)
        {
            if (!TryGetGeneralSettings(pawn, out object settings))
            {
                disallowedTraits = null;
                return false;
            }

            disallowedTraits = AlienDisallowedTraits.GetValue(settings) as IList;
            return disallowedTraits != null;
        }

        private static bool TryGetForcedTraits(Pawn pawn, out IList forcedTraits)
        {
            if (!TryGetGeneralSettings(pawn, out object settings))
            {
                forcedTraits = null;
                return false;
            }

            forcedTraits = AlienGeneralSettingsForcedTraits.GetValue(settings) as IList;
            return forcedTraits != null;
        }

        private static bool TryGetTraitEntry(object entry, out Tuple<string, int> result)
        {
            var defName = TraitEntryDefName.GetValue(entry) as string;
            object itemDegree = TraitEntryDegree.GetValue(entry);
            int degree = itemDegree as int? ?? -10;

            result = new Tuple<string, int>(defName, degree);
            return defName != null;
        }

        private static bool IsTraitDisallowed(Pawn pawn, string defName, int degree)
        {
            if (!TryGetDisallowedTraits(pawn, out IList disallowedTraits))
            {
                return false;
            }

            foreach (object item in disallowedTraits)
            {
                if (!TryGetTraitEntry(item, out Tuple<string, int> pair))
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
