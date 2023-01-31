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
                    string username = argument.StartsWith("@") ? argument.Substring(1) : argument;

                    Viewer viewer = Viewers.All.Find(v => string.Equals(v.username, username, StringComparison.InvariantCultureIgnoreCase));

                    if (viewer != null)
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

            ProcessRequestPost(username, viewer);
        }

        protected virtual void ProcessRequestPost([NotNull] string username, [NotNull] Viewer viewer)
        {
        }
    }
}
