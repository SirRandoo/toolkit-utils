using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
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
            {"DAllergic", "🤧"},
        };

        static Interests()
        {
            Active = ModsConfig.ActiveModsInLoadOrder
                .Any(m => m.PackageId.EqualsIgnoreCase("dame.InterestsFramework"));

            if (!Active)
            {
                return;
            }

            foreach (var handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("dame.InterestsFramework")
            ))
            {
                InterestsClass = handle.GetType().Assembly.GetType("DInterests.InterestBase", false);

                if (InterestsClass != null)
                {
                    InterestsList = AccessTools.Field(InterestsClass, "interestList");
                }

                if (InterestsList != null)
                {
                    InterestListInstance = InterestsList.GetValue(InterestsClass) as IList;
                }

                if (InterestListInstance != null)
                {
                    foreach (var def in InterestListInstance)
                    {
                        if (!(def is Def instance))
                        {
                            TkLogger.Warn($@"Could not cast ""{def.ToStringSafe()}"" to a Def instance!");
                            continue;
                        }

                        UsableInterestList.Add(instance);
                    }
                }

                Active = InterestsClass != null;
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


            var passionValue = skill.passion;
            Def interest;

            try
            {
                interest = UsableInterestList[(int) passionValue];
            }
            catch (Exception e)
            {
                TkLogger.Error($"Could not get interest def for {skill.def}", e);
                return string.Empty;
            }

            if (interest != null)
            {
                return InterestIndex.TryGetValue(interest.defName, string.Empty)
                    .AltText($"{interest.LabelCap.RawText}");
            }

            TkLogger.Info("Interest def was null!");
            return string.Empty;
        }
    }
}
