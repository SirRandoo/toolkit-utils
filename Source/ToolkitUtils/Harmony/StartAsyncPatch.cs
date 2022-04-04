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
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using RestSharp;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class StartAsyncPatch
    {
        private const int ErrorId = 938298212;
        private static byte[] _tokenHash;

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(TwitchWrapper), "StartAsync");
        }

        public static bool Prefix()
        {
            if (!ToolkitCoreSettings.channel_username.NullOrEmpty() && !ToolkitCoreSettings.oauth_token.NullOrEmpty())
            {
                VerifyToken();

                return true;
            }

            Log.ErrorOnce(@"<color=""#ff6b00"">ToolkitUtils :: Could not connect bot -- ToolkitCore isn't fully set up.</color>", ErrorId);

            return false;
        }

        private static void VerifyToken()
        {
            if (!_tokenHash.NullOrEmpty())
            {
                byte[] buffer;

                using (var hasher = SHA256.Create())
                {
                    buffer = hasher.ComputeHash(Encoding.UTF8.GetBytes(ToolkitCoreSettings.oauth_token));
                }

                if (StringComparer.OrdinalIgnoreCase.Compare(_tokenHash, buffer) == 0)
                {
                    return;
                }

                _tokenHash = buffer;
            }

            Task.Run(async () => await VerifyTokenInternal()).ConfigureAwait(false);
        }

        private static async Task VerifyTokenInternal()
        {
            var client = new RestClient("https://id.twitch.tv/");
            var request = new RestRequest("oauth2/validate", Method.GET);
            request.AddHeader("Authorization", $"Bearer: {ToolkitCoreSettings.oauth_token.Replace("oauth:", "")}");
            IRestResponse<TokenValidateResponse> response = await client.ExecuteAsync<TokenValidateResponse>(request);

            if (!response.IsSuccessful)
            {
                TkUtils.Logger.Warn($"Could not validate oauth token. Reason: {response.Content}");

                return;
            }

            if (response.Data.Login.EqualsIgnoreCase(ToolkitCoreSettings.bot_username))
            {
                return;
            }

            TokenValidateResponse data = response.Data;

            TkUtils.Context.Post(
                state =>
                {
                    TkUtils.Logger.Warn($@"The token provided is for the account ""{data.Login}"", not the specified ""{ToolkitCoreSettings.bot_username}""");
                    Log.TryOpenLogWindow();
                },
                null
            );
        }
    }
}
