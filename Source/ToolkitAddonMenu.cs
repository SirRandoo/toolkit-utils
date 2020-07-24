using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
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
                    () => Find.WindowStack.Add(
                        new Window_ModSettings(LoadedModManager.GetMod<TwitchToolkit.TwitchToolkit>())
                    )
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Events".Localize(),
                    () => Find.WindowStack.Add(new StoreIncidentsWindow())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Items".Localize(),
                    () => Find.WindowStack.Add(new StoreDialog())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Commands".Localize(),
                    () => Find.WindowStack.Add(new Window_Commands())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Viewers".Localize(),
                    () => Find.WindowStack.Add(new Window_Viewers())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.NameQueue".Localize(),
                    () => Find.WindowStack.Add(new NameQueueDialog())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Tracker".Localize(),
                    () => Find.WindowStack.Add(new Window_Trackers())
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
