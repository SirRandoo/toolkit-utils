using System.Collections.Generic;
using System.Linq;
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

            Log.Message($"{TKUtils.ID} :: Building mod list cache...");
            TKUtils.ModListCache = TKUtils.GetModListVersioned();
            Log.Message($"{TKUtils.ID} :: Built mod list cache ({TKUtils.ModListCache.Length} mods loaded)");
        }
    }

    public class TKUtils : Mod
    {
        public const string ID = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;
        internal static KeyValuePair<string, string>[] ModListCache;

        public TKUtils(ModContentPack content) : base(content)
        {
            GetSettings<TKSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(ToolkitWindow)));
        }

        public static string[] GetModListUnversioned() => GetModListVersioned().Select(m => m.Key).ToArray();

        public static KeyValuePair<string, string>[] GetModListVersioned()
        {
            if(ModListCache == null || ModListCache.Any())
            {
                ModListCache = LoadedModManager.ModHandles
                    .Select(m =>
                    {
                        return new KeyValuePair<string, string>(
                            m.Content.Name,
                            m.GetType().Module.Assembly.GetName().Version.ToString()
                        );
                    })
                    .ToArray();
            }

            return ModListCache;
        }

        public override void DoSettingsWindowContents(Rect inRect) => GetSettings<TKSettings>().DoWindowContents(inRect);

        public override string SettingsCategory() => ID;
    }
}
