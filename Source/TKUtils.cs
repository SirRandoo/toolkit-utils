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
    public static class TkUtilsStatic
    {
        static TkUtilsStatic()
        {
            TkUtils.Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");
            TkUtils.Harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.Info("Building mod list cache...");
            TkUtils.ModListCache = TkUtils.GetModListVersioned();
            Logger.Info($"Built mod list cache ({TkUtils.ModListCache.Length} mods loaded)");
        }
    }

    public class TkUtils : Mod
    {
        public const string Id = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;
        internal static Tuple<string, string>[] ModListCache;

        public TkUtils(ModContentPack content) : base(content)
        {
            GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(ToolkitWindow)));
        }

        public static IEnumerable<string> GetModListUnversioned()
        {
            return GetModListVersioned().Select(m => m.Item1);
        }

        public static Tuple<string, string>[] GetModListVersioned()
        {
            if (ModListCache?.Any() ?? true)
            {
                Logger.Debug("Something requested the mod list; we'll use the cached list.");
                return ModListCache;
            }

            var container = new List<Tuple<string, string>>();

            foreach (var handle in LoadedModManager.ModHandles)
            {
                if (container.Any(i => i.Item1.Equals(handle.Content.Name)))
                {
                    continue;
                }

                var version = handle.GetType().Module.Assembly.GetName().Version.ToString();

                container.Add(
                    new Tuple<string, string>(handle.Content.Name, version)
                );
            }

            ModListCache = container.ToArray();

            return ModListCache;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(inRect);
        }

        public override string SettingsCategory() => Id;
    }
}
