using System.Reflection;

using TwitchToolkit.Settings;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class TKUtils_Static
    {
        static TKUtils_Static()
        {
            TKUtils.Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");

            TKUtils.Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class TKUtils : Mod
    {
        public const string ID = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;

        public TKUtils(ModContentPack content) : base(content)
        {
            GetSettings<TKSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(ToolkitWindow)));
        }

        public override void DoSettingsWindowContents(Rect inRect) => GetSettings<TKSettings>().DoWindowContents(inRect);

        public override string SettingsCategory() => ID;
    }
}
