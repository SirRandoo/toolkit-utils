// MIT License
// 
// Copyright (c) 2023 SirRandoo
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
using NetEscapades.EnumGenerators;

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     A flag definition containing the various types of users that can
    ///     exist on Twitch.
    /// </summary>
    [Flags]
    [EnumExtensions]
    public enum UserTypes : short
    {
        /// <summary>
        ///     Represents a user with no special types. This value encompasses
        ///     the majority of users the mod will encounter.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Represents a viewer with an active subscription to the channel.
        /// </summary>
        Subscriber = 1,

        /// <summary>
        ///     Represents a viewer given the vip badge by the broadcaster.
        /// </summary>
        Vip = 2,

        /// <summary>
        ///     Represents a viewer given the moderator badge by the broadcaster.
        /// </summary>
        Moderator = 3,

        /// <summary>
        ///     Represents the broadcaster themselves.
        /// </summary>
        Broadcaster = 4
    }
}
