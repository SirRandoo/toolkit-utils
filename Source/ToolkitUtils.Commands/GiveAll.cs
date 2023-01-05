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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class GiveAll : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(twitchMessage.Message).Skip(1));

            if (!worker.TryGetNextAsInt(out int amount, 1))
            {
                return;
            }

            List<string> viewers = Viewers.ParseViewersFromJsonAndFindActiveViewers();

            if (viewers == null || viewers.Count <= 0)
            {
                return;
            }

            var count = 0;

            foreach (string username in viewers)
            {
                Viewers.GetViewer(username).GiveViewerCoins(amount);
                count++;
            }

            twitchMessage.Reply("TKUtils.GiveAll".LocalizeKeyed(amount.ToString("N0"), count.ToString("N0")));
        }
    }
}
