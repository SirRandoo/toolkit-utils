using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [UsedImplicitly]
    [HarmonyPatch(typeof(Window_Viewers), "DoWindowContents")]
    public static class UnassignPatch
    {
        private static readonly MethodInfo PawnHistoryRemove = AccessTools.Method(
            typeof(Dictionary<string, Pawn>),
            nameof(Dictionary<string, Pawn>.Remove)
        );

        private static readonly MethodInfo RenameAndRemoveMethod = AccessTools.Method(
            typeof(UnassignPatch),
            nameof(RenameAndRemove)
        );

        private static readonly FieldInfo ViewerComponentField = AccessTools.Field(typeof(Window_Viewers), "component");

        [HarmonyTranspiler]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(PawnHistoryRemove))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, ViewerComponentField);
                    instruction.operand = RenameAndRemoveMethod;
                }

                yield return instruction;
            }
        }

        private static void RenameAndRemove(GameComponentPawns component, string username)
        {
            if (component == null)
            {
                return;
            }

            Pawn pawn = component.PawnAssignedToUser(username);

            if (pawn != null)
            {
                var name = pawn.Name as NameTriple;
                pawn.Name = new NameTriple(name?.First, string.Empty, name?.Last);
            }

            component.pawnHistory.Remove(username);
        }
    }
}
