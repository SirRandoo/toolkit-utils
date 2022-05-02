// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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
