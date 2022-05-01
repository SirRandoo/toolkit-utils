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
