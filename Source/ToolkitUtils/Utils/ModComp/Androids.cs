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
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class Androids
    {
        public static bool Active;
        private static readonly Type AndroidSurgery;
        private static readonly FleshTypeDef AndroidFlesh;
        private static readonly FleshTypeDef MechFlesh;

        static Androids()
        {
            foreach (Mod handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("atlas.androidtiers")
            ))
            {
                try
                {
                    AndroidSurgery = handle.GetType().Assembly.GetType("MOARANDROIDS.Recipe_SurgeryAndroids", false);
                    AndroidFlesh = DefDatabase<FleshTypeDef>.GetNamedSilentFail("Android");
                    MechFlesh = DefDatabase<FleshTypeDef>.GetNamedSilentFail("MechanisedInfantry");

                    Active = AndroidSurgery != null;
                }
                catch (Exception e)
                {
                    LogHelper.Error("Compatibility class for Android Tiers failed!", e);
                }
            }
        }

        public static bool IsAndroid(Pawn pawn)
        {
            if (!Active)
            {
                return false;
            }

            return pawn.RaceProps.FleshType.defName.EqualsIgnoreCase(AndroidFlesh.defName)
                   || pawn.RaceProps.FleshType.defName.EqualsIgnoreCase(MechFlesh.defName);
        }

        public static bool IsAndroidSurgery([NotNull] RecipeDef recipe)
        {
            return recipe.workerClass == AndroidSurgery || recipe.workerClass.IsSubclassOf(AndroidSurgery);
        }

        public static bool IsSurgeryUsable(Pawn pawn, RecipeDef recipe)
        {
            if (!Active)
            {
                return false;
            }

            return IsAndroid(pawn) && IsAndroidSurgery(recipe);
        }
    }
}
