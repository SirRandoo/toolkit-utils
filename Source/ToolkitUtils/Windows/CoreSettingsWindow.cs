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
using System.Threading;
using System.Threading.Tasks;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.CommonLib.Windows;
using ToolkitCore;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class CoreSettingsWindow : ProxySettingsWindow
    {
        private readonly float _mediumFontHeight;
        private bool _showingToken;

        /// <inheritdoc/>
        public CoreSettingsWindow() : base(LoadedModManager.GetMod<ToolkitCore.ToolkitCore>())
        {
            _mediumFontHeight = Text.LineHeightOf(GameFont.Medium);
        }

        /// <inheritdoc/>
        protected override void DrawSettings(Rect region)
        {
            var authSectionHeader = new Rect(0f, 0f, region.width, _mediumFontHeight);
            var authSectionRegion = new Rect(0f, authSectionHeader.height, region.width, Text.SmallFontHeight * 3f);

            float connectionY = authSectionRegion.y + authSectionRegion.height + StandardMargin * 4f;
            var connectionSectionHeader = new Rect(0f, connectionY, region.width, _mediumFontHeight);

            var connectionSectionRegion = new Rect(
                0f,
                connectionSectionHeader.y + connectionSectionHeader.height,
                region.width,
                region.height - connectionY - connectionSectionHeader.height
            );

            GUI.BeginGroup(region);

            UiHelper.Label(authSectionHeader, "Channel Details", TextAnchor.MiddleLeft, GameFont.Medium);
            UiHelper.Label(connectionSectionHeader, "Connection", TextAnchor.MiddleLeft, GameFont.Medium);

            GUI.BeginGroup(authSectionRegion);
            DrawAuthSection(authSectionRegion.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(connectionSectionRegion);
            DrawConnectionSection(connectionSectionRegion.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawAuthSection(Rect region)
        {
            var channelLabel = new Rect(0f, 0f, Mathf.FloorToInt(region.width * 0.2f), Text.SmallFontHeight);
            var channelField = new Rect(channelLabel.width + 5f, 0f, Mathf.FloorToInt(region.width * 0.4f), Text.SmallFontHeight);

            var usernameLabel = new Rect(0f, channelLabel.height, channelLabel.width, Text.SmallFontHeight);
            var usernameField = new Rect(channelField.x, usernameLabel.y, channelField.width, Text.SmallFontHeight);
            var sameAsChannelBtn = new Rect(usernameField.x + usernameField.width, usernameField.y, Mathf.FloorToInt(region.width * 0.2f), Text.SmallFontHeight);

            var tokenLabel = new Rect(0f, usernameField.y + usernameField.height, usernameLabel.width, Text.SmallFontHeight);
            var tokenField = new Rect(usernameField.x, tokenLabel.y, usernameField.width, Text.SmallFontHeight);
            var newTokenBtn = new Rect(sameAsChannelBtn.x, tokenField.y, sameAsChannelBtn.width, Text.SmallFontHeight);
            var fromClipboardBtn = new Rect(newTokenBtn.x + newTokenBtn.width, newTokenBtn.y, newTokenBtn.width, Text.SmallFontHeight);

            UiHelper.Label(channelLabel, "Channel:");

            if (UiHelper.TextField(channelField, ToolkitCoreSettings.channel_username, out string newUsername))
            {
                ToolkitCoreSettings.channel_username = newUsername;
            }

            UiHelper.Label(usernameLabel, "Bot username:");

            if (UiHelper.TextField(usernameField, ToolkitCoreSettings.bot_username, out string newBotUsername))
            {
                ToolkitCoreSettings.bot_username = newBotUsername;
            }

            UiHelper.Label(tokenLabel, "OAuth token:");

            if (_showingToken && UiHelper.TextField(tokenField, ToolkitCoreSettings.oauth_token, out string newToken))
            {
                ToolkitCoreSettings.oauth_token = newToken;
            }
            else if (!_showingToken)
            {
                ToolkitCoreSettings.oauth_token = GUI.PasswordField(tokenField, ToolkitCoreSettings.oauth_token, '*');
            }

            if (UiHelper.FieldButton(tokenField, _showingToken ? Textures.Visible : Textures.Hidden, "Click to toggle token visibility."))
            {
                _showingToken = !_showingToken;
            }
            
            if (Widgets.ButtonText(sameAsChannelBtn, "Same as channel"))
            {
                ToolkitCoreSettings.bot_username = ToolkitCoreSettings.channel_username;
            }

            if (Widgets.ButtonText(newTokenBtn, "New token"))
            {
                Application.OpenURL("https://www.twitchapps.com/tmi/");
            }

            if (Widgets.ButtonText(fromClipboardBtn, "From clipboard"))
            {
                ToolkitCoreSettings.oauth_token = GUIUtility.systemCopyBuffer;
            }
        }

        private void DrawConnectionSection(Rect region)
        {
            bool isConnected = TwitchWrapper.Client?.IsConnected == true;
            var statusLabelRegion = new Rect(0f, 0f, Mathf.FloorToInt(region.width * 0.1f), Text.SmallFontHeight);
            var statusRegion = new Rect(statusLabelRegion.width, 0f, Mathf.FloorToInt(region.width * 0.1f), Text.SmallFontHeight);
            var statusBtnRegion = new Rect(statusRegion.x + statusRegion.width + 5f, 0f, Mathf.FloorToInt(region.width * 0.1f), Text.SmallFontHeight);
            var autoConnectRegion = new Rect(0f, statusLabelRegion.height, Mathf.FloorToInt(region.width * 0.5f), Text.SmallFontHeight);
            
            Rect whisperRegion = autoConnectRegion.Shift(Direction8Way.South, 0f);
            Rect forceWhisperRegion = whisperRegion.Shift(Direction8Way.South, 0f);
            Rect sendMessageRegion = forceWhisperRegion.Shift(Direction8Way.South, 0f);

            UiHelper.Label(statusLabelRegion, "Status");

            UiHelper.Label(
                statusRegion,
                isConnected ? "Connected" : "Disconnected",
                isConnected ? Color.green : ColorLibrary.RedReadable,
                TextAnchor.MiddleLeft,
                GameFont.Small
            );

            if (Widgets.ButtonText(statusBtnRegion, isConnected ? "Disconnect" : "Connect"))
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(ReconnectClient);
                }
                catch (NotSupportedException)
                {
                    // Ignored
                }
            }
            
            Widgets.CheckboxLabeled(autoConnectRegion, "Auto connect on startup", ref ToolkitCoreSettings.connectOnGameStartup);
            Widgets.CheckboxLabeled(whisperRegion , "Allow viewers to whisper", ref ToolkitCoreSettings.allowWhispers);
            Widgets.CheckboxLabeled(forceWhisperRegion, "Force viewers to whisper", ref ToolkitCoreSettings.forceWhispers);
            Widgets.CheckboxLabeled(sendMessageRegion, "Send connection message", ref ToolkitCoreSettings.sendMessageToChatOnStartup);
        }

        private static void ReconnectClient(object state)
        {
            if (TwitchWrapper.Client != null)
            {
                TwitchWrapper.Client.Disconnect();
            }
            
            TwitchWrapper.StartAsync();
        }
    }
}
