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
using ToolkitUtils.Mod.Data;
using TwitchLib.Client;

namespace ToolkitUtils.Mod.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="Message" /> interface, enabling additional functionality such as
///     replying to a message.
/// </summary>
public static class MessageExtensions
{
    /// <summary>Sends a reply to the specified message with the given content.</summary>
    /// <param name="message">The message to which the reply will be sent.</param>
    /// <param name="content">The content of the reply.</param>
    public static ValueTask<Result> SendReply(this Message message, string content)
    {
        TwitchClient twitchClient = TwitchWrapper.Client;

        return twitchClient?.SendReply(message.Id, content) ?? new ValueTask<Result>(Result.Ok());
    }
}
