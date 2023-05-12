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
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using ToolkitUtils.Workers;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;
using TaskExtensions = ToolkitUtils.Wrappers.Async.TaskExtensions;

namespace ToolkitUtils.Commands
{
    public class Divorce : CommandBase
    {
        /// <inheritdoc/>
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                MessageHelper.ReplyToUser(twitchMessage.Username, "TKUtils.NoPawn".TranslateSimple());

                return;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(twitchMessage.Message).Skip(1));

            if (!worker.TryGetNextAsViewer(out Viewer viewer) || !PurchaseHelper.TryGetPawn(viewer.username, out Pawn target))
            {
                MessageHelper.ReplyToUser(twitchMessage.Username, "TKUtils.PawnNotFound".Translate(worker.GetLast()));

                return;
            }

            Task.Run(
                async () =>
                {
                    await TaskExtensions.OnMainAsync(PerformDivorce, pawn, target);
                }
            );
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
