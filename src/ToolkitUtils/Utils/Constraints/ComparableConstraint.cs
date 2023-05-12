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
using ToolkitUtils.Api;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Utils.Constraints
{
    public abstract class ComparableConstraint : ConstraintBase
    {
        private readonly List<FloatMenuOption> _comparisonOptions = new List<FloatMenuOption>();

        protected ComparableConstraint()
        {
            Comparison = ComparisonType.Equal;

            foreach (string name in ComparisonTypeExtensions.GetNames())
            {
                _comparisonOptions.Add(new FloatMenuOption(name, () => Comparison = ComparisonTypeExtensions.TryParse(name, out ComparisonType type) ? type : default));
            }
        }

        protected ComparisonType Comparison { get; private set; }

        protected void DrawButton(Rect region)
        {
            if (!Widgets.ButtonText(region, Comparison.ToStringFast()))
            {
                return;
            }

            Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
        }
    }
}
