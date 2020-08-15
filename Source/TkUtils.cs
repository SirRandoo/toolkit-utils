using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Incidents;
using TwitchToolkit.Settings;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [UsedImplicitly]
    public static class TkUtilsStatic
    {
        static TkUtilsStatic()
        {
            TkSettings.ValidateDynamicSettings();

            TkUtils.Harmony = new HarmonyLib.Harmony("com.sirrandoo.tkutils");
            TkUtils.Harmony.PatchAll(Assembly.GetExecutingAssembly());

            List<StoreIncident> incidents = DefDatabase<StoreIncident>.AllDefsListForReading;
            var wereChanges = false;

            // We're not going to update this to use EventExtension
            // since it appears to wipe previous settings.
            foreach (StoreIncident incident in incidents.Where(
                i => i.defName == "BuyPawn" || i.defName == "AddTrait" || i.defName == "RemoveTrait"
            ))
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                incident.cost = 1;
                wereChanges = true;
            }

            if (wereChanges)
            {
                Store_IncidentEditor.UpdatePriceSheet();
            }
        }
    }

    [UsedImplicitly]
    public class TkUtils : Mod
    {
        public const string Id = "ToolkitUtils";
        internal static HarmonyLib.Harmony Harmony;

        public TkUtils(ModContentPack content) : base(content)
        {
            GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        public static void BuildModList()
        {
            List<ModMetaData> running = ModsConfig.ActiveModsInLoadOrder.ToList();

            Data.Mods = running.Where(m => m.Active)
               .Where(mod => !mod.Official)
               .Where(mod => !File.Exists(Path.Combine(mod.RootDir.ToString(), "About/IgnoreMe.txt")))
               .Select(ModItem.FromMetadata)
               .ToArray();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Id;
        }
    }
}
