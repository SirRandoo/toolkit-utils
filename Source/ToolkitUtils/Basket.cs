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
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;

namespace SirRandoo.ToolkitUtils
{
    public static class Basket
    {
        private static readonly Dictionary<string, IEasterEgg> Eggs = new Dictionary<string, IEasterEgg>
        {
            { "scavenging_mechanic", new MechanicalEgg() }, { "crystalroseeve", new CrystallizedEgg() }, { "merl_fox", new MerlEgg() }, { "ericcode", new CodedEgg() }
        };

        public static void RegisterEggFor([NotNull] string user, [NotNull] IEasterEgg egg) => Eggs.Add(user.ToLowerInvariant(), egg);

        public static bool UnregisterEggFor([NotNull] string user) => Eggs.Remove(user);

        [CanBeNull] public static IEasterEgg GetEggFor([NotNull] string user) => !Eggs.TryGetValue(user.ToLowerInvariant(), out IEasterEgg egg) ? null : egg;

        [ContractAnnotation("=> false,egg:null; => true,egg:notnull")]
        public static bool TryGetEggFor([NotNull] string user, out IEasterEgg egg)
        {
            egg = GetEggFor(user);

            return egg != null;
        }
    }
}
