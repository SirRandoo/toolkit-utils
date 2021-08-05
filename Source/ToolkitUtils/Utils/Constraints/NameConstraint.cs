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
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum NameStrategies { Is, Not, Contains, EndsWith, StartsWith }

    public class NameConstraint : ConstraintBase
    {
        private readonly string labelText;
        private readonly List<FloatMenuOption> strategyOptions;

        private NameStrategies nameStrategy = NameStrategies.Is;
        private string nameStrategyButtonText;
        private string nameStrategyText = nameof(NameStrategies.Is);

        public NameConstraint()
        {
            labelText = "TKUtils.PurgeMenu.Name".Localize().CapitalizeFirst();
            nameStrategyButtonText = $"TKUtils.PurgeMenu.{nameof(NameStrategies.Is)}".Localize();

            strategyOptions = Enum.GetNames(typeof(NameStrategies))
               .Select(
                    t => new FloatMenuOption(
                        $"TKUtils.PurgeMenu.{t}".Localize(),
                        () => NameStrategy = (NameStrategies)Enum.Parse(typeof(NameStrategies), t)
                    )
                )
               .ToList();
        }

        public NameStrategies NameStrategy
        {
            get => nameStrategy;
            set
            {
                if (nameStrategy != value)
                {
                    nameStrategyText = Enum.GetName(typeof(NameStrategies), value);
                    nameStrategyButtonText = $"TKUtils.PurgeMenu.{nameStrategyText}".Localize();
                }

                nameStrategy = value;
            }
        }

        public string Username { get; set; }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.ToForm(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.ToForm(0.25f);

            SettingsHelper.DrawLabel(labelRect, labelText);

            if (Widgets.ButtonText(buttonRect, nameStrategyButtonText))
            {
                Find.WindowStack.Add(new FloatMenu(strategyOptions));
            }

            Username = Widgets.TextField(inputRect, Username);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (NameStrategy)
            {
                case NameStrategies.Is:
                    return viewer.username.EqualsIgnoreCase(Username);
                case NameStrategies.Not:
                    return !viewer.username.EqualsIgnoreCase(Username);
                case NameStrategies.Contains:
                    return viewer.username.ToLower().Contains(Username.ToLower());
                case NameStrategies.StartsWith:
                    return viewer.username.ToLower().StartsWith(Username.ToLower());
                case NameStrategies.EndsWith:
                    return viewer.username.ToLower().EndsWith(Username.ToLower());
                default:
                    return false;
            }
        }
    }
}
