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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.CommonLib.Windows;
using ToolkitCore;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Windows
{
    public class CoreSettingsWindow : ProxySettingsWindow
    {
        private const string TokenUrl = "https://www.twitchapps.com/tmi/";
        private const float LineHeight = 32f;
        private const float LineSpacing = 10f;

        private readonly float _mediumFontHeight;
        private string _allowWhispersTooltip;
        private string _autoConnectTooltip;
        private string _connectionMessageTooltip;
        private string _forcedWhispersTooltip;
        private bool _showingToken;
        private string _newTokenTooltip;
        private string _channelTooltip;
        private string _botUsernameTooltip;
        private string _tokenTooltip;
        private string _tokenVisibleTooltip;
        private string _tokenHiddenTooltip;
        private string _sameAsChannelTooltip;
        private bool _tokenValid;

        private string _connectedText;
        private string _disconnectedText;
        private string _statusText;
        private string _channelDetailsHeader;
        private string _connectionHeader;
        private string _sendConnectionMessageText;
        private string _allowWhispersText;
        private string _tmiConfirmationText;
        private string _forceWhispersText;
        private string _autoConnectText;
        private string _tokenText;
        private string _botUsernameText;
        private string _channelText;
        private string _newTokenText;
        private string _sameAsChannelText;
        private string _connectText;
        private string _disconnectText;

        /// <inheritdoc/>
        public CoreSettingsWindow() : base(LoadedModManager.GetMod<ToolkitCore.ToolkitCore>())
        {
            _mediumFontHeight = Text.LineHeightOf(GameFont.Medium);
            _tokenValid = ToolkitCoreSettings.oauth_token.StartsWith("oauth:", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        protected override void GetTranslations()
        {
            _channelTooltip = "TKUtils.CoreTooltips.ChannelField".TranslateSimple();
            _botUsernameTooltip = "TKUtils.CoreTooltips.BotUsernameField".TranslateSimple();
            _tokenTooltip = "TKUtils.CoreTooltips.TokenField".TranslateSimple();
            _tokenVisibleTooltip = "TKUtils.CoreTooltips.ToggleTokenHidden".TranslateSimple();
            _tokenHiddenTooltip = "TKUtils.CoreTooltips.ToggleTokenVisible".TranslateSimple();
            _sameAsChannelTooltip = "TKUtils.CoreTooltips.SameAsChannel".TranslateSimple();
            _newTokenTooltip = "TKUtils.CoreTooltips.NewToken".TranslateSimple();

            _connectText = "TKUtils.AddonMenu.Connect".TranslateSimple();
            _disconnectText = "TKUtils.AddonMenu.Disconnect".TranslateSimple();

            _newTokenText = "TKUtils.Buttons.NewToken".TranslateSimple();
            _sameAsChannelText = "TKUtils.Buttons.SameAsChannel".TranslateSimple();

            _channelText = "TKUtils.Fields.Channel".TranslateSimple();
            _botUsernameText = "TKUtils.Fields.BotUsername".TranslateSimple();
            _tokenText = "TKUtils.Fields.Token".TranslateSimple();
            _autoConnectText = "TKUtils.Fields.AutoConnect".TranslateSimple();
            _allowWhispersText = "TKUtils.Fields.AllowWhispers".TranslateSimple();
            _forceWhispersText = "TKUtils.Fields.ForceWhispers".TranslateSimple();
            _sendConnectionMessageText = "TKUtils.Fields.SendConnectionMessage".TranslateSimple();

            _connectedText = "TKUtils.CoreSettings.Connected".TranslateSimple();
            _disconnectedText = "TKUtils.CoreSettings.Disconnected".TranslateSimple();
            _statusText = "TKUtils.CoreSettings.Status".TranslateSimple();
            _channelDetailsHeader = "TKUtils.CoreSettings.ChannelDetails".TranslateSimple();
            _connectionHeader = "TKUtils.CoreSettings.Connection".TranslateSimple();
        
            _autoConnectTooltip = "TKUtils.CoreTooltips.AutoConnect".TranslateSimple();
            _allowWhispersTooltip = "TKUtils.CoreTooltips.AllowWhispers".TranslateSimple();
            _forcedWhispersTooltip = "TKUtils.CoreTooltips.ForcedWhispers".TranslateSimple();
            _connectionMessageTooltip = "TKUtils.CoreTooltips.SendMessage".TranslateSimple();

            _tmiConfirmationText = "TKUtils.CoreSettings.TmiConfirmation".Translate(TokenUrl);
        }

        /// <inheritdoc/>
        protected override void DrawSettings(Rect region)
        {
            var authSectionHeader = new Rect(0f, 0f, 550f, _mediumFontHeight);
            var authSectionRegion = new Rect(0f, authSectionHeader.height, 550f, (LineSpacing + LineHeight) * 3f);

            float connectionY = authSectionRegion.y + authSectionRegion.height + StandardMargin * 4f;
            var connectionSectionHeader = new Rect(0f, connectionY, 550f, _mediumFontHeight);

            var connectionSectionRegion = new Rect(
                0f,
                connectionSectionHeader.y + connectionSectionHeader.height,
                550f,
                region.height - connectionY - connectionSectionHeader.height
            );

            GUI.BeginGroup(region);

            UiHelper.Label(authSectionHeader, _channelDetailsHeader, TextAnchor.MiddleLeft, GameFont.Medium);
            UiHelper.Label(connectionSectionHeader, _connectionHeader, TextAnchor.MiddleLeft, GameFont.Medium);

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
            var listing = new Listing_Standard();

            listing.Begin(region);

            var channelLabel = new Rect(0f, 0f, 200f, LineHeight);
            var channelField = new Rect(channelLabel.width + 5f, 0f, 200f, LineHeight);

            Rect usernameLabel = channelLabel.Shift(Direction8Way.South, LineSpacing);
            Rect usernameField = channelField.Shift(Direction8Way.South, LineSpacing);

            var sameAsChannelBtn = new Rect(usernameField.x + usernameField.width + 5f, usernameField.y, 150f, channelLabel.height);

            Rect tokenLabel = usernameLabel.Shift(Direction8Way.South, LineSpacing);
            Rect tokenField = usernameField.Shift(Direction8Way.South, LineSpacing);
            Rect newTokenBtn = sameAsChannelBtn.Shift(Direction8Way.South, LineSpacing);

            UiHelper.Label(channelLabel, _channelText);
            TooltipHandler.TipRegion(channelLabel, _channelTooltip);

            if (UiHelper.TextField(channelField, ToolkitCoreSettings.channel_username, out string newUsername))
            {
                ToolkitCoreSettings.channel_username = newUsername;
            }

            UiHelper.Label(usernameLabel, _botUsernameText);
            TooltipHandler.TipRegion(usernameLabel, _botUsernameTooltip);

            if (UiHelper.TextField(usernameField, ToolkitCoreSettings.bot_username, out string newBotUsername))
            {
                ToolkitCoreSettings.bot_username = newBotUsername;
            }

            UiHelper.Label(tokenLabel, _tokenText);
            TooltipHandler.TipRegion(tokenLabel, _tokenTooltip);
            DrawTokenField(tokenField);

            if (UiHelper.FieldButton(tokenLabel, _showingToken ? Textures.Visible : Textures.Hidden, _showingToken ? _tokenVisibleTooltip : _tokenHiddenTooltip))
            {
                _showingToken = !_showingToken;
            }

            if (Widgets.ButtonText(sameAsChannelBtn, _sameAsChannelText))
            {
                ToolkitCoreSettings.bot_username = ToolkitCoreSettings.channel_username;
            }
            
            TooltipHandler.TipRegion(sameAsChannelBtn, _sameAsChannelTooltip);

            if (Widgets.ButtonText(newTokenBtn, _newTokenText))
            {
                Find.WindowStack.Add(new ConfirmationDialog(_tmiConfirmationText, () => Application.OpenURL(TokenUrl)));
            }
            
            TooltipHandler.TipRegion(newTokenBtn, _newTokenTooltip);

            listing.End();
        }

        private void DrawConnectionSection(Rect region)
        {
            GUI.BeginGroup(region);

            bool isConnected = TwitchWrapper.Client?.IsConnected == true;
            var statusLabelRegion = new Rect(0f, 0f, 200f, LineHeight);
            var statusRegion = new Rect(statusLabelRegion.width, 0f, statusLabelRegion.width, LineHeight);
            var statusBtnRegion = new Rect(statusRegion.x + statusRegion.width + 5f, 0f, 150f, LineHeight);

            var autoConnectRegion = new Rect(0f, statusLabelRegion.height, statusRegion.x + 24f, LineHeight);
            Rect whisperRegion = autoConnectRegion.Shift(Direction8Way.South, 0f);
            Rect forceWhisperRegion = whisperRegion.Shift(Direction8Way.South, 0f);
            Rect sendMessageRegion = forceWhisperRegion.Shift(Direction8Way.South, 0f);

            UiHelper.Label(statusLabelRegion, _statusText);

            UiHelper.Label(
                statusRegion,
                isConnected ? _connectedText : _disconnectedText,
                isConnected ? Color.green : ColorLibrary.RedReadable,
                TextAnchor.MiddleLeft,
                GameFont.Small
            );

            if (Widgets.ButtonText(statusBtnRegion, isConnected ? _disconnectText : _connectText))
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

            Widgets.CheckboxLabeled(autoConnectRegion, _autoConnectText, ref ToolkitCoreSettings.connectOnGameStartup);
            TooltipHandler.TipRegion(autoConnectRegion, _autoConnectTooltip);

            Widgets.CheckboxLabeled(whisperRegion, _allowWhispersText, ref ToolkitCoreSettings.allowWhispers);
            TooltipHandler.TipRegion(whisperRegion, _allowWhispersTooltip);

            Widgets.CheckboxLabeled(forceWhisperRegion, _forceWhispersText, ref ToolkitCoreSettings.forceWhispers);
            TooltipHandler.TipRegion(forceWhisperRegion, _forcedWhispersTooltip);

            Widgets.CheckboxLabeled(sendMessageRegion, _sendConnectionMessageText, ref ToolkitCoreSettings.sendMessageToChatOnStartup);
            TooltipHandler.TipRegion(sendMessageRegion, _connectionMessageTooltip);

            GUI.EndGroup();
        }

        private void DrawTokenField(Rect region)
        {
            if (!_tokenValid)
            {
                GUI.color = Color.red;
            }

            if (!DrawTokenFieldInternal(region, out string newToken) || newToken == null)
            {
                GUI.color = Color.white;

                return;
            }

            GUI.color = Color.white;
            ToolkitCoreSettings.oauth_token = newToken;

            if (newToken.StartsWith("oauth:", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _tokenValid = false;
        }

        private bool DrawTokenFieldInternal(Rect region, [CanBeNull] out string newToken)
        {
            if (_showingToken)
            {
                return UiHelper.TextField(region, ToolkitCoreSettings.oauth_token, out newToken);
            }

            return DrawPasswordField(region, ToolkitCoreSettings.oauth_token, out newToken);
        }

        private static bool DrawPasswordField(Rect region, string text, [NotNull] out string newText)
        {
            newText = GUI.PasswordField(region, text, '*', Text.CurTextFieldStyle);

            return !newText.Equals(text);
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
