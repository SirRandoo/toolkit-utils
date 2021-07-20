// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Commands;
using TwitchToolkit.Store;
using TwitchToolkit.Windows;
using Verse;
using Command = TwitchToolkit.Command;
using StoreIncidentEditor = SirRandoo.ToolkitUtils.Windows.StoreIncidentEditor;

namespace SirRandoo.ToolkitUtils
{
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
                    SettingsHelper.OpenSettingsMenuFor<TwitchToolkit.TwitchToolkit>
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
                    "TKUtils.AddonMenu.GiftCoins".Localize(),
                    () =>
                    {
                        Command giftCoins = DefDatabase<Command>.GetNamed("GiftCoins");
                        giftCoins.enabled = !giftCoins.enabled;
                        CommandEditor.SaveCopy(giftCoins);

                        Messages.Message(
                            $"TKUtils.GiftCoins{(giftCoins.enabled ? "Enabled" : "Disabled")}".Localize(),
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
                ),
                new FloatMenuOption("TKUtils.AddonMenu.EditItemSettings".Localize(), OpenItemSettings),
                new FloatMenuOption("TKUtils.AddonMenu.EditTraitLimit".Localize(), OpenTraitSettings)
            };
        }

        public List<FloatMenuOption> MenuOptions()
        {
            return Options;
        }


        private static void OpenItemSettings()
        {
            Find.WindowStack.Add(new StoreIncidentEditor(IncidentDefOf.Item));
            IncidentDefOf.Item.settings.EditSettings();
        }

        private static void OpenTraitSettings()
        {
            Find.WindowStack.Add(new StoreIncidentEditor(IncidentDefOf.AddTrait));
            IncidentDefOf.AddTrait.settings.EditSettings();
        }
    }
}
