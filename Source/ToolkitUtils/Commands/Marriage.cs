// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
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

        private static void PerformMarriage(Pawn askerPawn, Pawn askeePawn)
        {
            MarriageCeremonyUtility.Married(askerPawn, askeePawn);
        }
    }
}
