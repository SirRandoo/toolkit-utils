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
        public static readonly bool Active = ModLister.GetActiveModWithIdentifier("dame.InterestsFramework") != null;
        private static readonly List<Def> UsableInterestList = new List<Def>();

        private static readonly Dictionary<string, string> InterestIndex = new Dictionary<string, string>
        {
            { "DMinorPassion", "\uD83D\uDD25" },
            { "DMajorPassion", "\uD83D\uDD25\uD83D\uDD25" },
            { "DMinorAversion", "\u2744" },
            { "DMajorAversion", "\u2744\u2744" },
            { "DCompulsion", "\uD83C\uDFB2" },
            { "DInvigorating", "\u2615" },
            { "DInspiring", "\uD83D\uDCA1" },
            { "DStagnant", "\uD83D\uDD12" },
            { "DForgetful", "\uD83D\uDCAD" },
            { "DVocalHatred", "\uD83D\uDCE2" },
            { "DNaturalGenius", "\uD83E\uDDE0" },
            { "DBored", "\uD83D\uDCA4" },
            { "DAllergic", "\uD83E\uDD27" }
        };

        static Interests()
        {
            if (!Active)
            {
                return;
            }

            try
            {
                Type interestsClass = AccessTools.TypeByName("DInterests.InterestBase");
                FieldInfo interestsList = AccessTools.Field("DInterests.InterestBase:interestList");
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
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Compatibility class for Interests failed!", e);
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
                return InterestIndex.TryGetValue(interest.defName, string.Empty).AltText($"{interest.LabelCap.RawText}");
            }

            return string.Empty;
        }
    }
}
