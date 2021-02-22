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
    public class TraitItemProxy
    {
        private readonly TraitItem traitItem;

        [MoonSharpHidden]
        public TraitItemProxy(TraitItem traitItem)
        {
            this.traitItem = traitItem;
        }

        public bool CanAdd => traitItem.CanAdd;
        public bool CanRemove => traitItem.CanRemove;
        public int CostToAdd => traitItem.CostToAdd;
        public int CostToRemove => traitItem.CostToRemove;
        public string DefName => traitItem.DefName;
        public int Degree => traitItem.Degree;
        public string Name => traitItem.Name;
        public string[] Conflicts => traitItem.Data.Conflicts;
        public bool CanBypassLimit => traitItem.Data.CanBypassLimit;
        public string AddKarmaType => traitItem.Data.KarmaTypeForAdding.ToString();
        public string RemoveKarmaType => traitItem.Data.KarmaTypeForRemoving.ToString();
        public bool HasCustomName => traitItem.Data.CustomName;
    }
}
