using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_CommandEditor), "PostClose")]
    public static class Window_CommandEditor__PostClose
    {
        [HarmonyPostfix]
        public static void PostClose()
        {
            ShopExpansionHelper.DumpCommands();
        }
    }
}
