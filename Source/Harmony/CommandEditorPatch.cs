using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_CommandEditor), "PostClose")]
    public static class CommandEditorPatch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            if (TkSettings.Offload)
            {
                Task.Run(ShopExpansion.DumpCommands);
            }
            else
            {
                ShopExpansion.DumpCommands();
            }
        }
    }
}
