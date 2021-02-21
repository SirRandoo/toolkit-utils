using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_CommandEditor), "PostClose")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class CommandEditorPatch
    {
        public static void Postfix()
        {
            if (TkSettings.Offload)
            {
                Task.Run(Data.DumpCommands).ConfigureAwait(false);
            }
            else
            {
                Data.DumpCommands();
            }
        }
    }
}
