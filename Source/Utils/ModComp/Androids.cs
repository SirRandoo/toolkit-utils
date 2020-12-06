using System;
using System.Linq;
using RimWorld;
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
                    TkLogger.Error("Compatibility class for Android Tiers failed!", e);
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

        public static bool IsAndroidSurgery(RecipeDef recipe)
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
