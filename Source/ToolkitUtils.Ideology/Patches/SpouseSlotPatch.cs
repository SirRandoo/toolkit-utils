// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;
using PreceptDefOf = SirRandoo.ToolkitUtils.Ideology.Defs.PreceptDefOf;

namespace SirRandoo.ToolkitUtils.Ideology.Patches
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class SpouseSlotPatch1
    {
        private static readonly PreceptDef[] MarriagePrecepts =
        {
            PreceptDefOf.SpouseCount_Male_MaxTwo,
            PreceptDefOf.SpouseCount_Female_MaxTwo,
            PreceptDefOf.SpouseCount_Male_MaxThree,
            PreceptDefOf.SpouseCount_Female_MaxThree,
            PreceptDefOf.SpouseCount_Male_MaxFour,
            PreceptDefOf.SpouseCount_Female_MaxFour,
            PreceptDefOf.SpouseCount_Male_Unlimited,
            PreceptDefOf.SpouseCount_Female_Unlimited
        };

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(GameHelper), nameof(GameHelper.HasOpenSpouseSlot));
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static bool Prefix(Pawn pawn, ref bool __result)
        {
            foreach (PreceptDef precept in MarriagePrecepts)
            {
                Gender preceptAffects = GetGenderForPrecept(precept);

                if (!pawn.ideo.Ideo.HasPrecept(precept) || preceptAffects != pawn.gender)
                {
                    continue;
                }

                __result = pawn.GetSpouseCount(false) < GetLimitForPrecept(precept);

                return false;
            }

            return true;
        }

        private static Gender GetGenderForPrecept([NotNull] PreceptDef precept)
        {
            var comp = precept.comps.Find(c => c is PreceptComp_UnwillingToDo_Gendered) as PreceptComp_UnwillingToDo_Gendered;

            return comp?.gender ?? Gender.None;
        }

        internal static int GetLimitForPrecept([NotNull] Def precept)
        {
            switch (precept.defName)
            {
                case nameof(PreceptDefOf.SpouseCount_Male_MaxTwo):
                    return 2;
                case nameof(PreceptDefOf.SpouseCount_Male_MaxThree):
                    return 3;
                case nameof(PreceptDefOf.SpouseCount_Male_MaxFour):
                    return 4;
                case nameof(PreceptDefOf.SpouseCount_Male_Unlimited):
                    return int.MaxValue;
                case nameof(PreceptDefOf.SpouseCount_Female_MaxTwo):
                    return 2;
                case nameof(PreceptDefOf.SpouseCount_Female_MaxThree):
                    return 3;
                case nameof(PreceptDefOf.SpouseCount_Female_MaxFour):
                    return 4;
                case nameof(PreceptDefOf.SpouseCount_Female_Unlimited):
                    return int.MaxValue;
                default:
                    return 1;
            }
        }
    }
}
