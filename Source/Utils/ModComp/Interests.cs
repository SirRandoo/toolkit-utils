using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class Interests
    {
        public static readonly bool Active;
        private static readonly Type InterestsClass;
        private static readonly FieldInfo InterestsList;
        private static readonly IList InterestListInstance;
        private static readonly List<Def> UsableInterestList = new List<Def>();

        private static readonly Dictionary<string, string> InterestIndex = new Dictionary<string, string>
        {
            {"DMinorPassion", "🔥"},
            {"DMajorPassion", "🔥🔥"},
            {"DMinorAversion", "❄"},
            {"DMajorAversion", "❄❄"},
            {"DCompulsion", "🎲"},
            {"DInvigorating", "☕"},
            {"DInspiring", "💡"},
            {"DStagnant", "🔒"},
            {"DForgetful", "💭"},
            {"DVocalHatred", "📢"},
            {"DNaturalGenius", "🧠"},
            {"DBored", "💤"},
            {"DAllergic", "🤧"}
        };

        static Interests()
        {
            foreach (Mod handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("dame.InterestsFramework")
            ))
            {
                try
                {
                    InterestsClass = handle.GetType().Assembly.GetType("DInterests.InterestBase");

                    InterestsList = AccessTools.Field(InterestsClass, "interestList");
                    InterestListInstance = InterestsList.GetValue(InterestsClass) as IList;

                    // ReSharper disable once PossibleNullReferenceException
                    foreach (object def in InterestListInstance)
                    {
                        if (!(def is Def instance))
                        {
                            continue;
                        }

                        UsableInterestList.Add(instance);
                    }

                    Active = true;
                }
                catch (Exception e)
                {
                    TkLogger.Error("Compatibility class for Interests failed!", e);
                }
            }
        }

        public static string GetIconForPassion(SkillRecord skill)
        {
            if (!Active)
            {
                return null;
            }

            if (!UsableInterestList.Any())
            {
                return null;
            }


            Passion passionValue = skill.passion;
            Def interest;

            try
            {
                interest = UsableInterestList[(int) passionValue];
            }
            catch (Exception e)
            {
                return string.Empty;
            }

            if (interest != null)
            {
                return InterestIndex.TryGetValue(interest.defName, string.Empty)
                    .AltText($"{interest.LabelCap.RawText}");
            }

            return string.Empty;
        }
    }
}
