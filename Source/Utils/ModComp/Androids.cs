using System;
using System.Linq;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public class Androids
    {
        public static bool Active;
        public static Type AndroidSurgery;
        public static readonly FleshTypeDef AndroidFlesh;
        public static readonly FleshTypeDef MechFlesh;

        static Androids()
        {
            foreach (var handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("atlas.androidtiers")
            ))
            {
                AndroidSurgery = handle.GetType().Assembly.GetType("MOARANDROIDS.Recipe_SurgeryAndroids", false);
                AndroidFlesh = DefDatabase<FleshTypeDef>.GetNamedSilentFail("Android");
                MechFlesh = DefDatabase<FleshTypeDef>.GetNamedSilentFail("MechanisedInfantry");

                Active = AndroidSurgery != null;
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

        public static bool IsSurgeryUsable(Pawn pawn, RecipeDef recipe)
        {
            if (!Active)
            {
                return false;
            }

            if (!IsAndroid(pawn))
            {
                return false;
            }

            return recipe.Worker.GetType().IsSubclassOf(AndroidSurgery);
        }
    }
}
