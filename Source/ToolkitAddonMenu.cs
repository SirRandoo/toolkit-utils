using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using TwitchToolkit;
using TwitchToolkit.Store;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [UsedImplicitly]
    public class ToolkitAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options;

        static ToolkitAddonMenu()
        {
            Options = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Settings".Localize(),
                    () =>
                    {
                        var settings = new Window_ModSettings(LoadedModManager.GetMod<TwitchToolkit.TwitchToolkit>());

                        Find.WindowStack.TryRemove(settings.GetType(), false);
                        Find.WindowStack.Add(settings);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Events".Localize(),
                    () =>
                    {
                        var window = new StoreIncidentsWindow();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Items".Localize(),
                    () =>
                    {
                        var window = new StoreDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Commands".Localize(),
                    () =>
                    {
                        var window = new Window_Commands();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Viewers".Localize(),
                    () =>
                    {
                        var window = new Window_Viewers();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.NameQueue".Localize(),
                    () =>
                    {
                        var window = new NameQueueDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Tracker".Localize(),
                    () =>
                    {
                        var window = new Window_Trackers();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.ToggleCoinEarning".Localize(),
                    () =>
                    {
                        ToolkitSettings.EarningCoins = !ToolkitSettings.EarningCoins;

                        Messages.Message(
                            $"TKUtils.CoinEarning{(ToolkitSettings.EarningCoins ? "Enabled" : "Disabled")}".Localize(),
                            MessageTypeDefOf.NeutralEvent
                        );
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.DebugFix".Localize(),
                    () =>
                    {
                        Helper.playerMessages = new List<string>();
                        Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();
                    }
                )
            };
        }

        public List<FloatMenuOption> MenuOptions()
        {
            return Options;
        }
    }
}
