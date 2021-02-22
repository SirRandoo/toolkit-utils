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
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CoinConstraint : ComparableConstraint
    {
        private readonly string labelText;
        private string buffer = "0";
        private int coins;

        public CoinConstraint()
        {
            labelText = "TKUtils.PurgeMenu.Coins".Localize().CapitalizeFirst();
        }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.ToForm(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.ToForm(0.25f);

            SettingsHelper.DrawLabel(labelRect, labelText);
            DrawButton(buttonRect);
            Widgets.TextFieldNumeric(inputRect, ref coins, ref buffer);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (Comparison)
            {
                case ComparisonTypes.Equal:
                    return viewer.coins == coins;
                case ComparisonTypes.Greater:
                    return viewer.coins > coins;
                case ComparisonTypes.Less:
                    return viewer.coins < coins;
                case ComparisonTypes.GreaterEqual:
                    return viewer.coins >= coins;
                case ComparisonTypes.LessEqual:
                    return viewer.coins <= coins;
                default:
                    return false;
            }
        }
    }
}
