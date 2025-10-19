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
using System;
using NetEscapades.EnumGenerators;

namespace ToolkitUtils.Mod.Data;

/// <summary>
///     Enum representing various types of users that can exist on Twitch. Provides a flag-based system to categorize
///     users into multiple roles such as Subscriber, VIP, Moderator, and Broadcaster.
/// </summary>
[Flags]
[EnumExtensions]
public enum UserTypes : short
{
    /// <summary>
    ///     Indicates the absence of any specific user type, typically representing a regular user without special
    ///     permissions or roles.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Indicates a user who has subscribed to a Twitch channel, typically providing them with various perks and
    ///     access to subscriber-only content.
    /// </summary>
    Subscriber = 1,

    /// <summary>
    ///     Represents a VIP user on Twitch who has special status and access to additional features but is not a
    ///     moderator or broadcaster.
    /// </summary>
    Vip = 2,

    /// <summary>
    ///     Represents a user with moderation privileges, allowing them to manage content and users within a Twitch
    ///     channel.
    /// </summary>
    Moderator = 3,

    /// <summary>Represents a user type indicating that the user is the broadcaster of the Twitch channel.</summary>
    Broadcaster = 4,
}
