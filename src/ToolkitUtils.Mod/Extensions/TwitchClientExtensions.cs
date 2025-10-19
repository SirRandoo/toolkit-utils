// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Threading.Tasks;
using ToolkitCore;
using TwitchLib.Client;

namespace ToolkitUtils.Mod.Extensions;

/// <summary>Provides extension methods for handling operations related to the TwitchClient.</summary>
public static class TwitchClientExtensions
{
    /// Sends a reply to a specific message in the Twitch chat using the TwitchLib Client.
    /// <param name="client">The TwitchClient instance used to send the reply.</param>
    /// <param name="messageId">The ID of the message to which the reply should be sent.</param>
    /// <param name="content">The content of the reply to be sent.</param>
    public static ValueTask<Result> SendReply(this TwitchClient client, string messageId, string content)
    {
        TwitchWrapper.Client?.SendRaw($"@reply-parent-msg-id={messageId} PRIVMSG #{ToolkitCoreSettings.channel_username} :{content}");

        return new ValueTask<Result>(Result.Ok());
    }
}
