using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class TkAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options;

        static TkAddonMenu()
        {
            Options = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Settings".Localize(),
                    () => Find.WindowStack.Add(new Window_ModSettings(LoadedModManager.GetMod<TkUtils>()))
                ),
            #if DEBUG
                new FloatMenuOption("TKUtils.AddonMenu.Editor".Localize(), () => Find.WindowStack.Add(new Editor())),
            #endif
                new FloatMenuOption(
                    "TKUtils.AddonMenu.PawnKind".Localize(),
                    () => Find.WindowStack.Add(new PawnKindConfigDialog())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Trait".Localize(),
                    () => Find.WindowStack.Add(new TraitConfigDialog())
                ),
                new FloatMenuOption(
                    "TKUtils.AddonMenu.Purge".Localize(),
                    () => Find.WindowStack.Add(new PurgeViewersDialog())
                ),
                new FloatMenuOption(
                    "Wiki".Localize(),
                    () => Application.OpenURL("https://sirrandoo.github.io/toolkit-utils")
                )
            };
        }

        public List<FloatMenuOption> MenuOptions() => Options;
    }
}
