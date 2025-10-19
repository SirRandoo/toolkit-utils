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
// with ToolkitUtils.Core. If not, see <https://www.gnu.org/licenses/>.
using ToolkitCore;
using ToolkitUtils.Mod.Data;
using TwitchLib.Client;

namespace ToolkitUtils.Core.Extensions;

public static class TwitchClientExtensions
{
    public static void NotifyPawnRequired(this TwitchClient? client, Viewer viewer)
    {
        // client?.SendMessageTo(viewer, Translations.NoPawn);
    }

    public static void NotifyInsufficientBalance(this TwitchClient? client, Viewer viewer, int requiredAmount)
    {
        // client?.SendMessageTo(
        //     viewer,
        //     string.Format(Translations.InsufficientBalance, requiredAmount.ToString(format: "N0"),
        //                   viewer.Coins.ToString(format: "N0")));
    }

    public static void SendMessageTo(this TwitchClient? client, Viewer viewer, string message)
    {
        client?.SendMessage(ToolkitCoreSettings.channel_username, $"@{viewer.Name} -> {message}");
    }

    public static void SendChannelMessage(this TwitchClient? client, string message)
    {
        client?.SendMessage(ToolkitCoreSettings.channel_username, message);
    }
}
