using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_CommandEditor), "PostClose")]
    public static class CommandEditorPatch
    {
        [HarmonyPostfix]
        public static void PostClose()
        {
            ShopExpansionHelper.DumpCommands();
        }
    }
}
