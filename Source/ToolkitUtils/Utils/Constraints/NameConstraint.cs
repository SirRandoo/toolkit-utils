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
using CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum NameStrategies { Is, Not, Contains, EndsWith, StartsWith }

    public class NameConstraint : ConstraintBase
    {
        private readonly string _labelText;
        private readonly List<FloatMenuOption> _strategyOptions;

        private NameStrategies _nameStrategy = NameStrategies.Is;
        private string _nameStrategyButtonText;

        public NameConstraint()
        {
            _labelText = "TKUtils.PurgeMenu.Name".TranslateSimple().CapitalizeFirst();
            _nameStrategyButtonText = $"TKUtils.PurgeMenu.{nameof(NameStrategies.Is)}".Localize();

            _strategyOptions = Enum.GetNames(typeof(NameStrategies))
               .Select(t => new FloatMenuOption($"TKUtils.PurgeMenu.{t}".Localize(), () => NameStrategy = (NameStrategies)Enum.Parse(typeof(NameStrategies), t)))
               .ToList();
        }

        public NameStrategies NameStrategy
        {
            get => _nameStrategy;
            set
            {
                if (_nameStrategy != value)
                {
                    string nameStrategyText = Enum.GetName(typeof(NameStrategies), value);
                    _nameStrategyButtonText = $"TKUtils.PurgeMenu.{nameStrategyText}".TranslateSimple();
                }

                _nameStrategy = value;
            }
        }

        public string Username { get; set; }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.Split(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.Split(0.25f);

            UiHelper.Label(labelRect, _labelText);

            if (Widgets.ButtonText(buttonRect, _nameStrategyButtonText))
            {
                Find.WindowStack.Add(new FloatMenu(_strategyOptions));
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
