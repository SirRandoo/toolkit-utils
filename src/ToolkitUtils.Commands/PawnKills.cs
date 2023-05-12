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
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnKills : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple());

                return;
            }

            int totalKills = pawn!.records.GetAsInt(RecordDefOf.Kills);
            int animalKills = pawn.records.GetAsInt(RecordDefOf.KillsAnimals);
            int humanLikeKills = pawn.records.GetAsInt(RecordDefOf.KillsHumanlikes);
            int mechanoidKills = pawn.records.GetAsInt(RecordDefOf.KillsMechanoids);

            string container = ResponseHelper.JoinPair("TKUtils.PawnKills.Total".TranslateSimple().CapitalizeFirst(), totalKills.ToString("N0"));

            container += ResponseHelper.OuterGroupSeparator;

            container += string.Join(
                ", ",
                ResponseHelper.JoinPair("TKUtils.PawnKills.Humanlike".TranslateSimple().CapitalizeFirst(), humanLikeKills.ToString("N0")),
                ResponseHelper.JoinPair("TKUtils.PawnKills.Animals".TranslateSimple().CapitalizeFirst(), animalKills.ToString("N0")),
                ResponseHelper.JoinPair("TKUtils.PawnKills.Mechanoids".TranslateSimple().CapitalizeFirst(), mechanoidKills.ToString("N0"))
            );


            twitchMessage.Reply(container);
        }
    }
}
