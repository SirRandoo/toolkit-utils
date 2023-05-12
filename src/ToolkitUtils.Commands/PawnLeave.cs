// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnLeave : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple());

                return;
            }

            if (pawn!.IsCaravanMember())
            {
                twitchMessage.Reply("TKUtils.Leave.Caravan".TranslateSimple());

                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (CompatRegistry.Magic?.IsUndead(pawn) ?? false)
            {
                twitchMessage.Reply("TKUtils.Leave.Undead".TranslateSimple());
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

        private static void ForceLeave([NotNull] ITwitchMessage twitchMessage, [NotNull] Pawn pawn)
        {
            if (TkSettings.LeaveMethod.EqualsIgnoreCase("Thanos") && FilthMaker.TryMakeFilth(
                pawn.Position,
                pawn.Map,
                ThingDefOf.Filth_Ash,
                pawn.LabelShortCap,
                Mathf.CeilToInt(pawn.BodySize * 0.6f)
            ))
            {
                twitchMessage.Reply("TKUtils.Leave.Thanos".TranslateSimple());

                Find.LetterStack.ReceiveLetter(
                    "TKUtils.LeaveLetter.ThanosTitle".TranslateSimple(),
                    "TKUtils.LeaveLetter.ThanosDescription".Translate(pawn.LabelShortCap),
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

                twitchMessage.Reply("TKUtils.Leave.Generic".TranslateSimple());

                Find.LetterStack.ReceiveLetter(
                    "TKUtils.LeaveLetter.GenericTitle".TranslateSimple(),
                    "TKUtils.LeaveLetter.GenericDescription".Translate(pawn.LabelShortCap),
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
