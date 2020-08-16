using System;
using System.Linq;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class Immortals
    {
        public static readonly bool Active;
        internal static readonly HediffDef ImmortalHediffDef;

        static Immortals()
        {
            foreach (Mod _ in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("fridgeBaron.Immortals")
            ))
            {
                try
                {
                    ImmortalHediffDef =
                        DefDatabase<HediffDef>.AllDefs.FirstOrDefault(h => h.defName.Equals("IH_Immortal"));

                    Active = ImmortalHediffDef != null;
                }
                catch (Exception e)
                {
                    TkLogger.Error("Compatibility class for Immortals failed!", e);
                }
            }
        }

        public static bool TryGrantImmortality(Pawn pawn)
        {
            try
            {
                pawn.health.AddHediff(ImmortalHediffDef);
                return true;
            }
            catch (Exception e)
            {
                TkLogger.Error($"Could not grant immortality to {pawn.LabelCap}", e);
                return false;
            }
        }
    }
}
