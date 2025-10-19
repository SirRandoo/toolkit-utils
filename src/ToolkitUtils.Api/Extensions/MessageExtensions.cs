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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Mono.Reflection;
using TwitchLib.Client.Models.Interfaces;

namespace ToolkitUtils.Api.Extensions;

/// <summary>A collection of extension methods for <see cref="ITwitchMessage" /> types.</summary>
[PublicAPI]
public static class MessageExtensions
{
    private static readonly PropertyInfo TwitchMessageProperty = AccessTools.Property(typeof(ITwitchMessage), nameof(ITwitchMessage.Message));

    private static readonly FieldInfo TwitchMessageField = TwitchMessageProperty.GetBackingField();

    /// <summary>
    ///     Reassigns the value of a <see cref="ITwitchMessage" />'s <see cref="ITwitchMessage.Message" /> property to the
    ///     value passed via <see cref="newMessage" />.
    /// </summary>
    /// <param name="message">The message being modified.</param>
    /// <param name="newMessage">The new value to assign the message's <see cref="ITwitchMessage.Message" /> property.</param>
    public static void SetMessage(this ITwitchMessage message, string newMessage)
    {
        TwitchMessageField.SetValue(message, newMessage);
    }

    /// <summary>Returns whether a user has a certain subset of badges.</summary>
    /// <param name="message">The message containing badge information</param>
    /// <param name="badgeIds">The badge ids to match against</param>
    public static bool HasBadges(this ITwitchMessage message, params string[] badgeIds)
    {
        List<KeyValuePair<string, string>> badges = message.ChatMessage.Badges;

        for (var i = 0; i < badges.Count; i++)
        {
            for (var j = 0; j < badgeIds.Length; j++)
            {
                if (!string.Equals(badges[i].Key, badgeIds[j], StringComparison.OrdinalIgnoreCase)) return false;
            }
        }

        return true;
    }
}
