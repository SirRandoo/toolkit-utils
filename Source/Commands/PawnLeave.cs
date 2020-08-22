﻿using JetBrains.Annotations;
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
    [UsedImplicitly]
    public class PawnLeave : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            if (pawn.IsCaravanMember())
            {
                twitchMessage.Reply("TKUtils.Leave.Caravan".Localize());
                return;
            }

            GameComponentPawns component;
            if (MagicComp.Active && pawn.IsUndead())
            {
                twitchMessage.Reply("TKUtils.Leave.Undead".Localize());

                component = Current.Game.GetComponent<GameComponentPawns>();
                component?.pawnHistory.Remove(twitchMessage.Username);
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
                twitchMessage.Reply("TKUtils.Responses.Leave.Thanos".Localize());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Thanos.Title".Localize(),
                    "TKUtils.Letters.Leave.Thanos.Description".Localize(pawn.LabelShortCap),
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

                twitchMessage.Reply("TKUtils.Responses.Leave.Generic".Localize());
                Find.LetterStack.ReceiveLetter(
                    "TKUtils.Letters.Leave.Generic.Title".Localize(),
                    "TKUtils.Letters.Leave.Generic.Description".Localize(pawn.LabelShortCap),
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

            component = Current.Game.GetComponent<GameComponentPawns>();
            component?.pawnHistory.Remove(twitchMessage.Username);
        }
    }
}