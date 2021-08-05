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
        private static readonly List<Def> UsableInterestList = new List<Def>();

        private static readonly Dictionary<string, string> InterestIndex = new Dictionary<string, string>
        {
            { "DMinorPassion", "🔥" },
            { "DMajorPassion", "🔥🔥" },
            { "DMinorAversion", "❄" },
            { "DMajorAversion", "❄❄" },
            { "DCompulsion", "🎲" },
            { "DInvigorating", "☕" },
            { "DInspiring", "💡" },
            { "DStagnant", "🔒" },
            { "DForgetful", "💭" },
            { "DVocalHatred", "📢" },
            { "DNaturalGenius", "🧠" },
            { "DBored", "💤" },
            { "DAllergic", "🤧" }
        };

        static Interests()
        {
            foreach (Mod handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("dame.InterestsFramework")
            ))
            {
                try
                {
                    Type interestsClass = handle.GetType().Assembly.GetType("DInterests.InterestBase");

                    FieldInfo interestsList = AccessTools.Field(interestsClass, "interestList");
                    var interestListInstance = interestsList.GetValue(interestsClass) as IList;

                    // ReSharper disable once PossibleNullReferenceException
                    foreach (object def in interestListInstance)
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
                    LogHelper.Error("Compatibility class for Interests failed!", e);
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
                interest = UsableInterestList[(int)passionValue];
            }
            catch (Exception)
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
