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

using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum ComparisonTypes { Greater, Equal, Less, GreaterEqual, LessEqual }

    public class ComparableConstraint : ConstraintBase
    {
        private readonly List<FloatMenuOption> _comparisonOptions;
        private ComparisonTypes _comparison;
        private string _comparisonButtonText;
        private string _comparisonText;

        protected ComparableConstraint()
        {
            Comparison = ComparisonTypes.Equal;
            _comparisonOptions = Data.ComparisonTypes.Select(t => new FloatMenuOption($"TKUtils.PurgeMenu.{t.ToString()}".Localize(), () => Comparison = t)).ToList();
        }

        protected ComparisonTypes Comparison
        {
            get => _comparison;
            private set
            {
                if (_comparison != value)
                {
                    _comparisonText = Enum.GetName(typeof(ComparisonTypes), value);
                    _comparisonButtonText = $"TKUtils.PurgeMenu.{_comparisonText}".Localize();
                }

                _comparison = value;
            }
        }

        protected void DrawButton(Rect canvas)
        {
            if (!Widgets.ButtonText(canvas, _comparisonButtonText))
            {
                return;
            }

            Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
        }
    }
}
