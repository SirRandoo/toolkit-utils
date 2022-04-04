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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnLeave : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            if (!PurchaseHelper.TryGetPawn(msg.Username, out Pawn pawn))
            {
                msg.Reply("TKUtils.NoPawn".Localize());

                return;
            }

            if (pawn!.IsCaravanMember())
            {
                msg.Reply("TKUtils.Leave.Caravan".Localize());

                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (CompatRegistry.Magic?.IsUndead(pawn) ?? false)
            {
                msg.Reply("TKUtils.Leave.Undead".Localize());
                component?.pawnHistory.Remove(msg.Username);

                if (pawn.Name is NameTriple name)
                {
                    pawn.Name = new NameTriple(name.First ?? "", name.Last ?? "", name.Last ?? "");
                }

                return;
            }

            ForceLeave(msg, pawn);
            component.pawnHistory.Remove(msg.Username);
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
                twitchMessage.Reply("TKUtils.Leave.Thanos".Localize());

                Find.LetterStack.ReceiveLetter(
                    "TKUtils.LeaveLetter.ThanosTitle".Localize(),
                    "TKUtils.LeaveLetter.ThanosDescription".LocalizeKeyed(pawn.LabelShortCap),
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
                    "TKUtils.LeaveLetter.GenericDescription".LocalizeKeyed(pawn.LabelShortCap),
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
