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
            labelText = "TKUtils.Windows.Purge.Constraints.Name".Localize();
            nameStrategyButtonText =
                $"TKUtils.Windows.Purge.NameComparisonTypes.{nameof(NameStrategies.Is)}".Localize();

            strategyOptions = Enum.GetNames(typeof(NameStrategies))
               .Select(
                    t => new FloatMenuOption(
                        $"TKUtils.Windows.Purge.NameComparisonTypes.{t}".Localize(),
                        () => NameStrategy = (NameStrategies) Enum.Parse(typeof(NameStrategies), t)
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
                    nameStrategyButtonText = $"TKUtils.Windows.Purge.NameComparisonTypes.{nameStrategyText}".Localize();
                }

                nameStrategy = value;
            }
        }

        public string Username { get; set; }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.ToForm(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.ToForm(0.25f);

            Widgets.Label(labelRect, labelText);

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
