using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_CommandEditor), "PostClose")]
    [UsedImplicitly]
    public static class CommandEditorPatch
    {
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void PostClose()
        {
            ShopExpansionHelper.DumpCommands();
        }
    }
}
