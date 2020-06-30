using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class LeaveCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".TranslateSimple());
                return;
            }

            if (pawn.IsCaravanMember())
            {
                twitchMessage.Reply("TKUtils.Responses.Leave.Caravan".TranslateSimple());
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
                twitchMessage.Reply("TKUtils.Responses.Leave.Thanos".TranslateSimple());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Thanos.Title".TranslateSimple(),
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

                twitchMessage.Reply("TKUtils.Responses.Leave.Generic".TranslateSimple());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Generic.Title".TranslateSimple(),
                    "TKUtils.Letters.Leave.Generic.Description".Translate(pawn.LabelShortCap),
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

            var component = Current.Game.GetComponent<GameComponentPawns>();
            component?.pawnHistory.Remove(twitchMessage.Username);
        }
    }
}
