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

using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class Marriage : ConsensualCommand
    {
        /// <inheritdoc/>
        protected override void ProcessAcceptInternal(string asker, string askee)
        {
            if (!PurchaseHelper.TryGetPawn(asker, out Pawn askerPawn))
            {
                MessageHelper.ReplyToUser(asker, "TKUtils.NoPawn".LocalizeKeyed(asker));

                return;
            }

            if (!PurchaseHelper.TryGetPawn(askee, out Pawn askeePawn))
            {
                MessageHelper.ReplyToUser(asker, "TKUtils.PawnNotFound".LocalizeKeyed(askee));

                return;
            }

            if (!GameHelper.CanPawnsMarry(askerPawn, askeePawn))
            {
                MessageHelper.ReplyToUser(asker, "TKUtils.Marriage.NoSlots".Localize());

                return;
            }

            TkUtils.Context.Post(c => PerformMarriage(askerPawn, askeePawn), null);
        }

        /// <inheritdoc />
        protected override void ProcessRequestPost(string username, Viewer viewer)
        {
            MessageHelper.ReplyToUser(
                viewer.username,
                $"{username} wants to marry you. You can accept it with {TkSettings.Prefix}{command.command} accept {username}, or decline it with {TkSettings.Prefix}{command.command} decline {username}."
            );
        }

        private static void PerformMarriage(Pawn askerPawn, Pawn askeePawn)
        {
            MarriageCeremonyUtility.Married(askerPawn, askeePawn);
        }
    }
}
