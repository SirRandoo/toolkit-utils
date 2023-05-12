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

using System.Linq;
using JetBrains.Annotations;
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using ToolkitUtils.Workers;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class GiveCoins : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(twitchMessage.Message).Skip(1));

            if (!worker.TryGetNextAsViewer(out Viewer viewer) || !worker.TryGetNextAsInt(out int amount))
            {
                return;
            }

            viewer.GiveViewerCoins(amount);
            Store_Logger.LogGiveCoins(twitchMessage.Username, viewer.username, amount);
            twitchMessage.Reply("TKUtils.GiveCoins.Done".Translate(amount.ToString("N0"), viewer.username, viewer.GetViewerCoins().ToString("N0")));
        }
    }
}
