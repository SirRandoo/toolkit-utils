using System.Reflection;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class PatchRunner
    {
        static PatchRunner()
        {
            new HarmonyLib.Harmony("com.sirrandoo.tkutils").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
