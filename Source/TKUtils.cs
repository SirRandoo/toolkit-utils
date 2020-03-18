using System;
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

            Logger.Info("Building mod list cache...");
            TKUtils.ModListCache = TKUtils.GetModListVersioned();
            Logger.Info($"Built mod list cache ({TKUtils.ModListCache.Length} mods loaded)");
        }
    }

    public class TKUtils : Mod
    {
        public const string ID = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;
        internal static Tuple<string, string>[] ModListCache;

        public TKUtils(ModContentPack content) : base(content)
        {
            GetSettings<TKSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(ToolkitWindow)));
        }

        public static string[] GetModListUnversioned() => GetModListVersioned().Select(m => m.Item1).ToArray();

        public static Tuple<string, string>[] GetModListVersioned()
        {
            if(ModListCache == null || !ModListCache.Any())
            {
                var container = new List<Tuple<string, string>>();

                foreach(var handle in LoadedModManager.ModHandles)
                {
                    if(container.Any(i => i.Item1.Equals(handle.Content.Name)))
                    {
                        continue;
                    }

                    var version = handle.GetType().Module.Assembly.GetName().Version.ToString();

                    container.Add(
                        new Tuple<string, string>(handle.Content.Name, version)
                    );
                }

                ModListCache = container.ToArray();
            }

            return ModListCache;
        }

        public override void DoSettingsWindowContents(Rect inRect) => GetSettings<TKSettings>().DoWindowContents(inRect);

        public override string SettingsCategory() => ID;
    }
}
