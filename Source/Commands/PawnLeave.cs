using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnLeave : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            if (pawn.IsCaravanMember())
            {
                twitchMessage.Reply("TKUtils.Leave.Caravan".Localize());
                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (pawn.IsUndead())
            {
                twitchMessage.Reply("TKUtils.Leave.Undead".Localize());
                component?.pawnHistory.Remove(twitchMessage.Username);

                if (pawn.Name is NameTriple name)
                {
                    pawn.Name = new NameTriple(name.First ?? "", name.Last ?? "", name.Last ?? "");
                }

                return;
            }

            ForceLeave(twitchMessage, pawn);
            component.pawnHistory.Remove(twitchMessage.Username);
        }

        private static void ForceLeave(ITwitchMessage twitchMessage, Pawn pawn)
        {
            if (TkSettings.LeaveMethod.EqualsIgnoreCase("Thanos")
                && FilthMaker.TryMakeFilth(
                    pawn.Position,
                    pawn.Map,
                    ThingDefOf.Filth_Ash,
                    pawn.LabelShortCap,
                    Mathf.CeilToInt(pawn.BodySize * 0.6f)
                ))
            {
                twitchMessage.Reply("TKUtils.Leave.Thanos".Localize());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.LeaveLetter.ThanosTitle".Localize(),
                    "TKUtils.LeaveLetter.ThanosDescription".Localize(pawn.LabelShortCap),
                    LetterDefOf.NeutralEvent,
                    new LookTargets(pawn.Position, pawn.Map)
                );
                pawn.Destroy();
            }
            else
            {
                if (TkSettings.DropInventory && pawn.AnythingToStrip())
                {
                    pawn.Strip();
                }

                twitchMessage.Reply("TKUtils.Leave.Generic".Localize());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.LeaveLetter.GenericTitle".Localize(),
                    "TKUtils.LeaveLetter.GenericDescription".Localize(pawn.LabelShortCap),
                    LetterDefOf.NeutralEvent,
                    new LookTargets(pawn.Position, pawn.Map)
                );

                if (pawn.Faction != null)
                {
                    pawn.SetFaction(null);
                }

                pawn.jobs.StopAll();
                pawn.health.surgeryBills.Clear();
            }
        }
    }
}
