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

using System.Linq;
using JetBrains.Annotations;
using ToolkitUtils.Helpers;
using ToolkitUtils.Models.Tables;

namespace ToolkitUtils.Models.Selectors
{
    public class CategoriesSelector : CategorySelector
    {
        public override string Label => base.Label.Pluralize();

        public override bool IsVisible([NotNull] TableSettingsItem<ThingItem> item)
        {
            bool shouldShow = Category.Split(',').Select(i => i.Trim().ToLowerInvariant()).ToList().Contains(item.Data.Category.ToLowerInvariant());

            return Exclude ? !shouldShow : shouldShow;
        }
    }
}
