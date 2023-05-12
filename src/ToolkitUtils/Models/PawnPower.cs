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

using JetBrains.Annotations;
using ToolkitUtils.Interfaces;
using Verse;

namespace ToolkitUtils.Models
{
    public class PawnPower : IPawnPower
    {
        public PawnPower()
        {
        }

        public PawnPower([NotNull] Def def, int minimumLevel)
        {
            MinimumLevel = minimumLevel;
            Name = def.label ?? def.defName;
        }

        public PawnPower(string name, int minimumLevel)
        {
            Name = name;
            MinimumLevel = minimumLevel;
        }

        public virtual string Name { get; }

        public virtual int MinimumLevel { get; }

        [NotNull] public static PawnPower From(string name, int minimumLevel) => new PawnPower(name, minimumLevel);

        [NotNull] public static IPawnPower From([NotNull] Def def, int minimumLevel) => new PawnPower(def, minimumLevel);
    }
}
