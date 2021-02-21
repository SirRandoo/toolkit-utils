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
    [HarmonyPatch(typeof(Window_Viewers), "DoWindowContents")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class UnassignPatch
    {
        private static readonly MethodInfo PawnHistoryRemove;
        private static readonly FieldInfo PawnHistoryField;
        private static readonly MethodInfo RenameAndRemoveMethod;
        private static readonly FieldInfo ViewerComponentField;

        static UnassignPatch()
        {
            ViewerComponentField = AccessTools.Field(typeof(Window_Viewers), "component");
            RenameAndRemoveMethod = AccessTools.Method(typeof(UnassignPatch), nameof(RenameAndRemove));
            PawnHistoryField = AccessTools.Field(typeof(GameComponentPawns), nameof(GameComponentPawns.pawnHistory));
            PawnHistoryRemove = AccessTools.Method(
                typeof(Dictionary<string, Pawn>),
                nameof(Dictionary<string, Pawn>.Remove),
                new[] {typeof(string)}
            );
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodFound = false;
            var componentFound = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(ViewerComponentField))
                {
                    componentFound = true;
                }

                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(PawnHistoryField) && componentFound)
                {
                    instruction.opcode = OpCodes.Nop;
                    componentFound = false;
                }

                if (instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(PawnHistoryRemove))
                {
                    instruction.operand = RenameAndRemoveMethod;
                    methodFound = true;
                }

                if (instruction.opcode == OpCodes.Pop && methodFound)
                {
                    instruction.opcode = OpCodes.Nop;
                    methodFound = false;
                }

                yield return instruction;
            }
        }

        private static void RenameAndRemove(GameComponentPawns component, string username)
        {
            if (username == null || component == null)
            {
                return;
            }

            Pawn pawn = component.PawnAssignedToUser(username);

            if (pawn?.Name is NameTriple name)
            {
                pawn.Name = new NameTriple(name.First, name.Last, name.Last);
            }

            component.pawnHistory.Remove(username);
        }
    }
}
