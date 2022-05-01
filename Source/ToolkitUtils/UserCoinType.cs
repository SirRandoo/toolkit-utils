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

namespace SirRandoo.ToolkitUtils
{
    /// <summary>
    ///     The various coin types a broadcaster can be within the mod.
    /// </summary>
    public enum UserCoinType
    {
        /// <summary>
        ///     If the current active broadcaster coin type, the broadcaster will
        ///     receive every bonus and coin rate available through Twitch
        ///     Toolkit.
        /// </summary>
        Broadcaster,

        /// <summary>
        ///     If the current active broadcaster coin type, the broadcaster will
        ///     receive only the subscriber bonus and coin rate available in
        ///     Twitch Toolkit.
        /// </summary>
        Subscriber,

        /// <summary>
        ///     If the current active broadcaster coin type, the broadcaster will
        ///     receive only the vip bonus and coin rate available in Twitch
        ///     Toolkit.
        /// </summary>
        Vip,

        /// <summary>
        ///     If the current active broadcaster coin type, the broadcaster will
        ///     receive only the moderator bonus and coin rate available in
        ///     Twitch Toolkit.
        /// </summary>
        Moderator,

        /// <summary>
        ///     If the current active broadcaster coin type, the broadcaster will
        ///     not receive any bonus or coin rate available through Twitch
        ///     Toolkit.
        /// </summary>
        None
    }
}
