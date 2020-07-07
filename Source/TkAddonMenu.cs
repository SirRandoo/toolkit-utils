using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class TkAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options;

        static TkAddonMenu()
        {
            Options = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Settings".Localize(),
                    () =>
                    {
                        var window = new Window_ModSettings(LoadedModManager.GetMod<TkUtils>());

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.PawnKind".Localize(),
                    () =>
                    {
                        var window = new PawnKindConfigDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Trait".Localize(),
                    () =>
                    {
                        var window = new TraitConfigDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Purge".Localize(),
                    () =>
                    {
                        var window = new PurgeViewersDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "Wiki".Localize(),
                    () => Application.OpenURL("https://sirrandoo.github.io/toolkit-utils")
                )
            };
        }

        public List<FloatMenuOption> MenuOptions()
        {
            return Options;
        }
    }
}
