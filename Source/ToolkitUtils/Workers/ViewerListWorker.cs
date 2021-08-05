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

using System;
using System.Threading.Tasks;
using RestSharp;
using SirRandoo.ToolkitUtils.Helpers;
using ToolkitCore;
# if !RW12
using JetBrains.Annotations;
#endif

namespace SirRandoo.ToolkitUtils.Workers
{
    public static class ViewerListWorker
    {
        private static RestClient _client;

    #if !RW12
        [ItemCanBeNull]
    #endif
        public static async Task<string> GetChatters()
        {
            _client ??= new RestClient("https://tmi.twitch.tv/group/user");

            var request = new RestRequest(
                $"{ToolkitCoreSettings.channel_username.ToLowerInvariant()}/chatters",
                DataFormat.None
            );

            string response = null;
            try
            {
                response = await _client.GetAsync<string>(request);
            }
            catch (Exception e)
            {
                LogHelper.Error("Could not refresh viewer list", e);
            }

            return response;
        }
    }
}
