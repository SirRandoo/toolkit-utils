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
                object raceSettings = AlienSettingsField.GetValue(pawn.kindDef.race);
                object generalSettings = AlienGeneralSettingsField.GetValue(raceSettings);

                if (!(AlienDisallowedTraits.GetValue(generalSettings) is IList disallowedTraits))
                {
                    return true;
                }

                foreach (object item in disallowedTraits)
                {
                    var defName = TraitEntryDefName.GetValue(item) as string;
                    int itemDegree = TraitEntryDegree.GetValue(item) is int
                        ? (int) TraitEntryDegree.GetValue(item)
                        : -10;

                    if ((defName?.Equals(traitDef.defName) ?? false) && itemDegree.Equals(degree))
                    {
                        return false;
                    }
                }

                if (!(AlienGeneralSettingsForcedTraits.GetValue(generalSettings) is IList forcedTraits))
                {
                    return true;
                }

                foreach (object item in forcedTraits)
                {
                    var defName = TraitEntryDefName.GetValue(item) as string;
                    int itemDegree = TraitEntryDegree.GetValue(item) is int
                        ? (int) TraitEntryDegree.GetValue(item)
                        : -10;

                    if ((defName?.Equals(traitDef.defName) ?? false) && itemDegree.Equals(degree))
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return true;
            }

            return true;
        }
    }
}
