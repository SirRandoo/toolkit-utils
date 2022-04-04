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

using System;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public abstract class ConsensualCommand : CommandBase
    {
        private protected static readonly ConsentWorker ConsentWorker = new ConsentWorker();

        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(twitchMessage.Message).Skip(1));
            string argument = worker.GetNext();

            switch (argument.ToLowerInvariant())
            {
                case "accept":
                    ProcessAccept(twitchMessage.Username, worker.GetNextAsViewer());

                    return;
                case "decline":
                    ProcessDecline(twitchMessage.Username, worker.GetNextAsViewer());

                    return;
                default:
                    if (worker.TryGetNextAsViewer(out Viewer viewer))
                    {
                        ProcessRequest(twitchMessage.Username, viewer);
                    }
                    else
                    {
                        MessageHelper.ReplyToUser(twitchMessage.Username, "TKUtils.InvalidViewerQuery".LocalizeKeyed(worker.GetLast()));
                    }

                    return;
            }
        }

        protected virtual void ProcessAccept([NotNull] string username, [CanBeNull] Viewer viewer)
        {
            lock (ConsentWorker)
            {
                string asker = null;

                if (viewer == null)
                {
                    asker = ConsentWorker.GetAsker(username);
                }
                else
                {
                    foreach (string user in ConsentWorker.GetAllOffersFor(username))
                    {
                        if (!string.Equals(user, viewer.username, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        asker = user;

                        break;
                    }
                }

                if (string.IsNullOrEmpty(asker))
                {
                    return;
                }

                ConsentWorker.ClearContext(asker);
                ConsentWorker.ClearContext(username);
                ProcessAcceptInternal(asker, username);
            }
        }

        protected virtual void ProcessDecline([NotNull] string username, [CanBeNull] Viewer viewer)
        {
            lock (ConsentWorker)
            {
                string asker = null;

                if (viewer == null)
                {
                    asker = ConsentWorker.GetAsker(username);
                }
                else
                {
                    foreach (string user in ConsentWorker.GetAllOffersFor(username))
                    {
                        if (!string.Equals(user, viewer.username, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        asker = user;

                        break;
                    }
                }

                if (string.IsNullOrEmpty(asker))
                {
                    return;
                }

                ConsentWorker.ClearContext(asker);
                ConsentWorker.ClearContext(username);
                ProcessDeclineInternal(asker, username);
            }
        }

        protected virtual void ProcessDeclineInternal([NotNull] string asker, [NotNull] string askee)
        {
            MessageHelper.ReplyToUser(asker, "TKUtils.RequestDeclined".LocalizeKeyed(askee));
        }

        protected virtual void ProcessAcceptInternal([NotNull] string asker, [NotNull] string askee)
        {
            MessageHelper.ReplyToUser(asker, "TKUtils.RequestApproved".LocalizeKeyed(askee));
        }

        protected virtual void ProcessRequest([NotNull] string username, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(username, out Pawn _))
            {
                MessageHelper.ReplyToUser(username, "TKUtils.NoPawn".LocalizeKeyed(username));

                return;
            }

            if (!PurchaseHelper.TryGetPawn(viewer.username, out Pawn _))
            {
                MessageHelper.ReplyToUser(username, "TKUtils.PawnNotFound".LocalizeKeyed(viewer.username));

                return;
            }

            lock (ConsentWorker)
            {
                ConsentWorker.Create(username, viewer.username!);
            }
        }
    }
}
