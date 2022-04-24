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
using SirRandoo.CommonLib.Helpers;
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
    /// <summary>
    ///     An <see cref="IAddonMenu"/> used by ToolkitCore to display a set
    ///     of "quick menu options" for users.
    /// </summary>
    [UsedImplicitly]
    public class ToolkitAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options = new List<FloatMenuOption>
        {
            new FloatMenuOption("TKUtils.AddonMenu.Settings".TranslateSimple(), () => LoadedModManager.GetMod<TwitchToolkit.TwitchToolkit>().OpenSettings()),
            new FloatMenuOption("TKUtils.AddonMenu.Events".TranslateSimple(), () => Find.WindowStack.Add(new StoreIncidentsWindow())),
            new FloatMenuOption("TKUtils.AddonMenu.Items".TranslateSimple(), () => Find.WindowStack.Add(new StoreDialog())),
            new FloatMenuOption("TKUtils.AddonMenu.Commands".TranslateSimple(), () => Find.WindowStack.Add(new Window_Commands())),
            new FloatMenuOption("TKUtils.AddonMenu.Viewers".TranslateSimple(), () => Find.WindowStack.Add(new Window_Viewers())),
            new FloatMenuOption("TKUtils.AddonMenu.NameQueue".TranslateSimple(), () => Find.WindowStack.Add(new NameQueueDialog())),
            new FloatMenuOption("TKUtils.AddonMenu.Tracker".TranslateSimple(), () => Find.WindowStack.Add(new Window_Trackers())),
            new FloatMenuOption(
                "TKUtils.AddonMenu.ToggleCoinEarning".TranslateSimple(),
                () =>
                {
                    ToolkitSettings.EarningCoins = !ToolkitSettings.EarningCoins;

                    Messages.Message($"TKUtils.CoinEarning{(ToolkitSettings.EarningCoins ? "Enabled" : "Disabled")}".TranslateSimple(), MessageTypeDefOf.NeutralEvent);
                }
            ),
            new FloatMenuOption(
                "TKUtils.AddonMenu.GiftCoins".TranslateSimple(),
                () =>
                {
                    Command giftCoins = DefDatabase<Command>.GetNamed("GiftCoins");
                    giftCoins.enabled = !giftCoins.enabled;
                    CommandEditor.SaveCopy(giftCoins);

                    Messages.Message($"TKUtils.GiftCoins{(giftCoins.enabled ? "Enabled" : "Disabled")}".TranslateSimple(), MessageTypeDefOf.NeutralEvent);
                }
            ),
            new FloatMenuOption(
                "TKUtils.AddonMenu.DebugFix".TranslateSimple(),
                () =>
                {
                    Helper.playerMessages.Clear();
                    Purchase_Handler.viewerNamesDoingVariableCommands.Clear();
                }
            ),
            new FloatMenuOption("TKUtils.AddonMenu.EditItemSettings".TranslateSimple(), OpenItemSettings),
            new FloatMenuOption("TKUtils.AddonMenu.EditTraitLimit".TranslateSimple(), OpenTraitSettings)
        };

        public List<FloatMenuOption> MenuOptions() => Options;


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
