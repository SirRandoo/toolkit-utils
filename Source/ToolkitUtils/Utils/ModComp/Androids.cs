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
using HarmonyLib;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp;

[StaticConstructorOnStartup]
public static class Androids
{
    public static readonly bool Active;
    private static readonly Type? AndroidSurgery;
    private static readonly FleshTypeDef? AndroidFlesh;
    private static readonly FleshTypeDef? MechFlesh;

    static Androids()
    {
        if (ModLister.GetActiveModWithIdentifier("atlas.androidtiers") == null)
        {
            return;
        }

        // ReSharper disable once StringLiteralTypo
        AndroidSurgery = AccessTools.TypeByName("MOARANDROIDS.Recipe_SurgeryAndroids");
        AndroidFlesh = DefDatabase<FleshTypeDef>.GetNamedSilentFail("Android");
        MechFlesh = DefDatabase<FleshTypeDef>.GetNamedSilentFail("MechanisedInfantry");

        Active = AndroidSurgery is not null;
    }

    public static bool IsAndroid(Pawn pawn)
    {
        if (!Active)
        {
            return false;
        }

        FleshTypeDef fleshType = pawn.RaceProps.FleshType;

        return fleshType.defName.Equals(AndroidFlesh!.defName) || fleshType.defName.Equals(MechFlesh!.defName);
    }

    public static bool IsAndroidSurgery(RecipeDef recipe) => recipe.workerClass is not null && AndroidSurgery!.IsAssignableFrom(recipe.workerClass);

    public static bool IsSurgeryUsable(Pawn pawn, RecipeDef recipe) => Active && IsAndroid(pawn) && IsAndroidSurgery(recipe);
}
