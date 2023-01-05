﻿// ToolkitUtils.TMagic
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
using SirRandoo.ToolkitUtils.Models;
using TorannMagic;
using Verse;

namespace SirRandoo.ToolkitUtils.TMagic.Models
{
    public class ClassPower : PawnPower
    {
        public ClassPower()
        {
        }
        public ClassPower(Def def, int minimumLevel) : base(def, minimumLevel)
        {
        }
        public ClassPower(string name, int minimumLevel) : base(name, minimumLevel)
        {
        }

        public TMAbilityDef AbilityDef { get; private set; }

        [NotNull] public static ClassPower From(TMAbilityDef def, int minimumLevel) => new ClassPower(def, minimumLevel) { AbilityDef = def };
    }
}
