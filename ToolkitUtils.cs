using System.Reflection;

using TwitchToolkit.Settings;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class ToolkitUtils_Static
    {
        static ToolkitUtils_Static()
        {
            ToolkitUtils.Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");

            ToolkitUtils.Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class ToolkitUtils : Mod
    {
        public const string ID = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;

        public ToolkitUtils(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(ToolkitWindow)));
        }

        public override void DoSettingsWindowContents(Rect inRect) => GetSettings<Settings>().DoWindowContents(inRect);

        public override string SettingsCategory() => ID;
    }
}
