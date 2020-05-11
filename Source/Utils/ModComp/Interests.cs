using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public class Interests
    {
        public static readonly bool Active;
        private static Type interestsClass;
        private static FieldInfo interestsList;

        static Interests()
        {
            Active = ModsConfig.ActiveModsInLoadOrder
                .Any(m => m.PackageId.EqualsIgnoreCase("dame.interestsframework"));
        }

        public static string GetIconForPassion(SkillRecord skill)
        {
            if (!Active)
            {
                return null;
            }

            if (interestsClass == null)
            {
                TkLogger.Info("Interests class is null! Fetching...");

                var modHandle = LoadedModManager.ModHandles.FirstOrDefault(
                    h => h.Content.PackageId.EqualsIgnoreCase("dame.interestsframework")
                );

                if (modHandle == null)
                {
                    return string.Empty;
                }

                interestsClass = modHandle.GetType().Assembly.GetType("DInterests.InterestBase", false);
            }

            if (interestsClass == null)
            {
                TkLogger.Info("Interests class is still null! Returning an empty string...");
                return string.Empty;
            }

            if (interestsList == null)
            {
                TkLogger.Info("Interests list is null! Fetching...");
                interestsList = AccessTools.Field(interestsClass, "interestList");
            }

            if (interestsList == null)
            {
                TkLogger.Info("Interests list is still null! Returning an empty string...");
                return string.Empty;
            }

            TkLogger.Info("Attempting to get list instance...");
            var value = interestsList.GetValue(interestsList);

            if (value == null)
            {
                TkLogger.Info("List instance is null! Returning an empty string...");
                return string.Empty;
            }

            if (!(value is IList valueAsList))
            {
                TkLogger.Info("List instance could not be casted to a list!");
                return string.Empty;
            }

            var passionValue = skill.passion;
            Def interest;

            try
            {
                interest = valueAsList[(int) passionValue] as Def;
            }
            catch (Exception e)
            {
                TkLogger.Error($"Could not get interest def for {skill.def}", e);
                return string.Empty;
            }

            if (interest == null)
            {
                TkLogger.Info("Interest def was null!");
                return string.Empty;
            }

            switch (interest.defName)
            {
                case "DMinorPassion":
                    return "🔥".AltText("+");

                case "DMajorPassion":
                    return "🔥🔥".AltText("++");

                case "DMinorAversion":
                    return "❄️".AltText("");

                case "DMajorAversion":
                    return "❄️❄️".AltText("");

                case "DCompulsion":
                    return "🎲".AltText("");

                case "DInvigorating":
                    return "☕".AltText("");

                case "DInspiring":
                    return "💡".AltText("");

                case "DStagnant":
                    return "🔒".AltText("");

                case "DForgetful":
                    return "💭".AltText("");

                case "DVocalHatred":
                    return "📢".AltText("");

                case "DNaturalGenius":
                    return "🧠".AltText("");

                case "DBored":
                    return "💤".AltText("");

                case "DAllergic":
                    return "🤧".AltText("");

                default:
                    return string.Empty;
            }
        }
    }
}
