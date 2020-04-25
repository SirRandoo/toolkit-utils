using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class TkAddonMenu : IAddonMenu
    {
        public List<FloatMenuOption> MenuOptions()
        {
            return new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.AddonMenu.Settings".Translate(),
                    () =>
                    {
                        var window = new Window_ModSettings(LoadedModManager.GetMod<TkUtils>());
                        
                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption("TKUtils.AddonMenu.RaceConfig".Translate(),
                    () =>
                    {
                        var window = new RaceConfigDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption("TKUtils.AddonMenu.TraitConfig".Translate(),
                    () =>
                    {
                        var window = new TraitConfigDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption("TKUtils.AddonMenu.Purge".Translate(),
                    () =>
                    {
                        var window = new PurgeViewersDialog();

                        Find.WindowStack.TryRemove(window.GetType(), false);
                        Find.WindowStack.Add(window);
                    }
                )
            };
        }
    }
}
