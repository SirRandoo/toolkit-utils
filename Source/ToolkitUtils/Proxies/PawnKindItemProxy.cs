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
using MoonSharp.Interpreter;
using SirRandoo.ToolkitUtils.Models;

namespace SirRandoo.ToolkitUtils.Proxies
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class PawnKindItemProxy
    {
        private PawnKindItem pawnKindItem;

        [MoonSharpHidden]
        public PawnKindItemProxy(PawnKindItem pawnKindItem)
        {
            this.pawnKindItem = pawnKindItem;
        }

        public string DefName => pawnKindItem.DefName;
        public string Name => pawnKindItem.Name;
        public bool Enabled => pawnKindItem.Enabled;
        public int Cost => pawnKindItem.Cost;
        public bool HasCustomName => pawnKindItem.PawnData.CustomName;
        public string KarmaType => pawnKindItem.Data.KarmaType.ToString();
    }
}
