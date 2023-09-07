// ToolkitUtils.TMagic
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

using SirRandoo.ToolkitUtils.Models;
using TorannMagic;

namespace SirRandoo.ToolkitUtils.TMagic.Models;

public class ClassPower : PawnPower
{
    public ClassPower(TMAbilityDef def, int minimumLevel) : base(def, minimumLevel)
    {
        AbilityDef = def;
    }

    public ClassPower(TMAbilityDef abilityDef, string name, int minimumLevel) : base(name, minimumLevel)
    {
        AbilityDef = abilityDef;
    }

    public TMAbilityDef AbilityDef { get; private set; }

    public static ClassPower From(TMAbilityDef def, int minimumLevel) => new(def, minimumLevel);
}
