using System.Reflection;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class PatchRunner
    {
        internal static readonly HarmonyLib.Harmony Harmony;

        static PatchRunner()
        {
            Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
