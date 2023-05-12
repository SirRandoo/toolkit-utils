// ToolkitUtils
// Copyright (C) 2021  SirRandoo
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

using System.Collections.Generic;
using JetBrains.Annotations;
using ToolkitUtils.Interfaces;
using ToolkitUtils.Utils.Basket;

namespace ToolkitUtils
{
    /// <summary>
    ///     A basket for holding easter eggs for certain users.
    /// </summary>
    public static class Basket
    {
        private static readonly Dictionary<string, IEasterEgg> Eggs = new Dictionary<string, IEasterEgg>
        {
            { "scavenging_mechanic", new MechanicalEgg() }, { "crystalroseeve", new CrystallizedEgg() }, { "merl_fox", new MerlEgg() }, { "ericcode", new CodedEgg() }
        };

        /// <summary>
        ///     Registers an egg for a given user.
        /// </summary>
        /// <param name="user">The user to register the egg for</param>
        /// <param name="egg">The egg to register for the user</param>
        /// <returns>Whether the egg was registered to the user</returns>
        public static bool RegisterEggFor([NotNull] string user, [NotNull] IEasterEgg egg)
        {
            if (!Eggs.ContainsKey(user.ToLowerInvariant()))
            {
                Eggs.Add(user.ToLowerInvariant(), egg);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Unregisters an egg for a given user.
        /// </summary>
        /// <param name="user">The user to unregister the egg for</param>
        /// <returns>Whether the egg was unregistered</returns>
        public static bool UnregisterEggFor([NotNull] string user) => Eggs.Remove(user);

        /// <summary>
        ///     Gets an easter egg for a given user.
        /// </summary>
        /// <param name="user">The user to get the egg for</param>
        /// <returns>
        ///     The egg for the given user, or <c>null</c> if the user
        ///     didn't have an easter egg registered for them
        /// </returns>
        [CanBeNull]
        public static IEasterEgg GetEggFor([NotNull] string user) => !Eggs.TryGetValue(user.ToLowerInvariant(), out IEasterEgg egg) ? null : egg;

        /// <summary>
        ///     Gets an easter egg for a given user.
        /// </summary>
        /// <param name="user">The user to get the egg for</param>
        /// <param name="egg">The egg for the given user</param>
        /// <returns>Whether or not the user has an easter egg registered to them</returns>
        [ContractAnnotation("=> false,egg:null; => true,egg:notnull")]
        public static bool TryGetEggFor([NotNull] string user, out IEasterEgg egg)
        {
            egg = GetEggFor(user);

            return egg != null;
        }
    }
}
