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
    ///     The various methods a pawn can leave the colony if a viewer uses
    ///     the "leave" command.
    /// </summary>
    public enum LeaveMethod
    {
        /// <summary>
        ///     If the current active leave method, the pawn will turn into a
        ///     pile of ash when a viewer users the "leave" command.
        /// </summary>
        Thanos,

        /// <summary>
        ///     If the current active leave method, the pawn will leave the
        ///     colony by having their faction unassigned to them.
        /// </summary>
        /// <remarks>
        ///     This leave method's name does not actually apply a mental break
        ///     anymore. This was done to try to prevent users from recapturing
        ///     pawns that their assigned viewer would otherwise not want.
        /// </remarks>
        MentalBreak
    }
}
