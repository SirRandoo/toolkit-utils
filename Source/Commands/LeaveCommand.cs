using RimWorld;
using RimWorld.Planet;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class LeaveCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            if (pawn.IsCaravanMember())
            {
                message.Reply("TKUtils.Responses.Leave.Caravan".Translate());
                return;
            }

            if (TkSettings.LeaveMethod.EqualsIgnoreCase("Thanos")
                && FilthMaker.TryMakeFilth(
                    pawn.Position,
                    pawn.Map,
                    ThingDefOf.Filth_Ash,
                    pawn.LabelShortCap,
                    Mathf.CeilToInt(pawn.BodySize * 0.6f)
                ))
            {
                message.Reply("TKUtils.Responses.Leave.Thanos".Translate());
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

                message.Reply("TKUtils.Responses.Leave.Generic".Translate());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Generic.Title".Translate(),
                    "TKUtils.Letters.Leave.Generic.Description".Translate(pawn.LabelShortCap),
                    LetterDefOf.NeutralEvent,
                    new LookTargets(pawn.Position, pawn.Map)
                );

                if (pawn.Faction != null)
                {
                    pawn.SetFaction(null);
                }

                pawn.jobs.StopAll();
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();
            component?.pawnHistory.Remove(message.User);
        }
    }
}
