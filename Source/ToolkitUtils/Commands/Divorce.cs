// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class Divorce : CommandBase
    {
        /// <inheritdoc/>
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                MessageHelper.ReplyToUser(twitchMessage.Username, "TKUtils.NoPawn".Localize());

                return;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(twitchMessage.Message).Skip(1));

            if (!worker.TryGetNextAsViewer(out Viewer viewer) || !PurchaseHelper.TryGetPawn(viewer.username, out Pawn target))
            {
                MessageHelper.ReplyToUser(twitchMessage.Username, "TKUtils.PawnNotFound".LocalizeKeyed(worker.GetLast()));

                return;
            }

            TkUtils.Context.Post(c => PerformDivorce(pawn, target), null);
        }

        private static void PerformDivorce(Pawn askerPawn, Pawn askeePawn)
        {
            foreach (Pawn spouse in askerPawn.GetSpouses(false))
            {
                if (spouse != askeePawn)
                {
                    continue;
                }

                SpouseRelationUtility.DoDivorce(askerPawn, askeePawn);

                break;
            }
        }
    }
}
