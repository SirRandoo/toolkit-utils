using RimWorld;
using RimWorld.Planet;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class LeaveCommand : CommandBase
    {
        private Pawn pawn;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            pawn = GetOrFindPawn(twitchCommand.Message);

            if (pawn == null)
            {
                twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            if (!pawn.IsCaravanMember())
            {
                return true;
            }

            twitchCommand.Reply("TKUtils.Responses.Leave.Caravan".Translate());
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
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
                twitchCommand.Reply("TKUtils.Responses.Leave.Thanos".Translate());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Thanos.Title".Translate(),
                    "TKUtils.Letters.Leave.Thanos.Description".Translate(pawn.LabelShortCap),
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

                twitchCommand.Reply("TKUtils.Responses.Leave.Generic".Translate());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Generic.Title".Translate(),
                    "TKUtils.Letters.Leave.Generic.Description".Translate(pawn.LabelShortCap),
                    LetterDefOf.NeutralEvent,
                    new LookTargets(pawn.Position, pawn.Map)
                );

                pawn.jobs.StopAll();
                pawn.health.surgeryBills.Clear();

                if (pawn.Faction != null)
                {
                    pawn.SetFaction(null);
                }
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();
            component?.pawnHistory.Remove(twitchCommand.Username);
        }

        public LeaveCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
