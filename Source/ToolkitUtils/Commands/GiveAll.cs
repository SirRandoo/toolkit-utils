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
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class GiveAll : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            var worker = ArgWorker.CreateInstance(twitchMessage.Message);

            if (!worker.TryGetNextAsInt(out int amount, 1))
            {
                return;
            }

            foreach (string username in Viewers.ParseViewersFromJsonAndFindActiveViewers())
            {
                Viewers.GetViewer(username).GiveViewerCoins(amount);
            }
        }
    }
}
