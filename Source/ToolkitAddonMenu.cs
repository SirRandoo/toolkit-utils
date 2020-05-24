using System;
using System.Collections.Generic;
using RimWorld;
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
    public class ToolkitAddonMenu : IAddonMenu
    {
        private static readonly Type NameQueueType;

        static ToolkitAddonMenu()
        {
            var assembly = typeof(TwitchToolkit.TwitchToolkit).Assembly;
            NameQueueType = assembly.GetType("TwitchToolkit.PawnQueue.QueueWindow");
        }

        public List<FloatMenuOption> MenuOptions()
        {
            return new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "Settings",
                    () =>
                    {
                        var settings = new Window_ModSettings(LoadedModManager.GetMod<TwitchToolkit.TwitchToolkit>());

                        Find.WindowStack.TryRemove(settings.GetType(), false);
                        Find.WindowStack.Add(settings);
                    }
                ),
                new FloatMenuOption(
                    "Events",
                    () =>
                    {
                        var window = new StoreIncidentsWindow();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Items",
                    () =>
                    {
                        var window = new StoreDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Commands",
                    () =>
                    {
                        var window = new Window_Commands();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Viewers",
                    () =>
                    {
                        var window = new Window_Viewers();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Name Queue",
                    () =>
                    {
                        var window = new NameQueueDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Tracker",
                    () =>
                    {
                        var window = new Window_Trackers();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Toggle Earning Coins",
                    () =>
                    {
                        ToolkitSettings.EarningCoins = !ToolkitSettings.EarningCoins;

                        Messages.Message(
                            $"Earning Coins is {(ToolkitSettings.EarningCoins ? "Enabled" : "Disabled")}",
                            MessageTypeDefOf.NeutralEvent
                        );
                    }
                ),
                new FloatMenuOption(
                    "Debug Fix",
                    () =>
                    {
                        Helper.playerMessages = new List<string>();
                        Purchase_Handler.viewerNamesDoingVariableCommands = new List<string>();
                    }
                )
            };
        }
    }
}
