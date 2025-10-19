// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Ideology. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Interactions.Commands;
using Verse;

namespace ToolkitUtils.Ideology.Patches;

[HarmonyPatch]
[PublicAPI]
internal static class SpouseSlotPatch
{
    private static readonly PreceptDef[] MarriagePrecepts =
    [
        IdeologyDefs.SpouseCount_Male_MaxTwo,
        IdeologyDefs.SpouseCount_Female_MaxTwo,
        IdeologyDefs.SpouseCount_Male_MaxThree,
        IdeologyDefs.SpouseCount_Female_MaxThree,
        IdeologyDefs.SpouseCount_Male_MaxFour,
        IdeologyDefs.SpouseCount_Female_MaxFour,
        IdeologyDefs.SpouseCount_Male_Unlimited,
        IdeologyDefs.SpouseCount_Female_Unlimited,
    ];

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Marriage), name: "HasOpenSpouseSlot");
    }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
    private static bool Prefix(Pawn pawn, ref bool __result)
    {
        foreach (PreceptDef precept in MarriagePrecepts)
        {
            Gender preceptAffects = GetGenderForPrecept(precept);

            if (!pawn.ideo.Ideo.HasPrecept(precept) || preceptAffects != pawn.gender) continue;

            __result = pawn.GetSpouseCount(false) < GetLimitForPrecept(precept);

            return false;
        }

        return true;
    }

    private static Gender GetGenderForPrecept(PreceptDef precept)
    {
        var comp = precept.comps.Find(c => c is PreceptComp_UnwillingToDo_Gendered) as PreceptComp_UnwillingToDo_Gendered;

        return comp?.gender ?? Gender.None;
    }

    internal static int GetLimitForPrecept(Def precept)
    {
        return precept.defName switch
        {
            nameof(IdeologyDefs.SpouseCount_Male_MaxTwo)      => 2,
            nameof(IdeologyDefs.SpouseCount_Male_MaxThree)    => 3,
            nameof(IdeologyDefs.SpouseCount_Male_MaxFour)     => 4,
            nameof(IdeologyDefs.SpouseCount_Male_Unlimited)   => int.MaxValue,
            nameof(IdeologyDefs.SpouseCount_Female_MaxTwo)    => 2,
            nameof(IdeologyDefs.SpouseCount_Female_MaxThree)  => 3,
            nameof(IdeologyDefs.SpouseCount_Female_MaxFour)   => 4,
            nameof(IdeologyDefs.SpouseCount_Female_Unlimited) => int.MaxValue,
            var _                                             => 1,
        };
    }
}
