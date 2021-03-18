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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class ResponseHelper
    {
        public const string OuterGroupSeparator = "⎮";
        public const string OuterGroupSeparatorAlt = " | ";
        public const string InfinityGlyph = "∞";
        public const string CoinGlyph = "💰";
        public const string KarmaGlyph = "⚖";
        public const string IncomeGlyph = "📈";
        public const string DebtGlyph = "📉";
        public const string TemperatureGlyph = "🌡";
        public const string BleedingGlyph = "🩸";
        public const string BandageGlyph = "🩹";
        public const string DaggerGlyph = "🗡";
        public const string PanGlyph = "🍳";
        public const string FireGlyph = "🔥";
        public const string DazedGlyph = "💫";
        public const string GhostGlyph = "👻";
        public const string LightningGlyph = "⚡";
        public const string AboutToBreakGlyph = "🤬";
        public const string OnEdgeGlyph = "😠";
        public const string MagicGlyph = "🔮";
        public const string StressedGlyph = "😣";
        public const string NeutralGlyph = "😐";
        public const string ContentGlyph = "🙂";
        public const string HappyGlyph = "😊";
        public const string BleedingSafeGlyphs = "🩸⌛";
        public const string BleedingBadGlyphs = "🩸⏳";
        public const string ForbiddenGlyph = "🚫";
        public const string PrincessGlyph = "👸";
        public const string PrinceGlyph = "🤴";
        public const string CrownGlyph = "👑";
        public const string MaleGlyph = "♂";
        public const string FemaleGlyph = "♀";
        public const string GenderlessGlyph = "⚪";

        [NotNull]
        public static string JoinPair(string key, string value)
        {
            return $"{key}: {value}";
        }

        [NotNull]
        public static string Join([NotNull] this IEnumerable<string> l, string separator)
        {
            return string.Join(separator, l);
        }

        [NotNull]
        public static string GroupedJoin([NotNull] this IEnumerable<string> l)
        {
            return string.Join(OuterGroupSeparator.AltText(OuterGroupSeparatorAlt), l);
        }

        [NotNull]
        public static string SectionJoin([NotNull] this IEnumerable<string> l)
        {
            return string.Join(", ", l);
        }

        public static string Localize(this string key, params NamedArgument[] args)
        {
            if (args.NullOrEmpty())
            {
                return key.TranslateSimple();
            }

            return key.Translate(args);
        }

        public static string LocalizeWithBackup(this string key, string backup)
        {
            return key.CanTranslate() ? key.TranslateSimple() : backup.TranslateSimple();
        }

        public static string Pluralize(this string s)
        {
            return Find.ActiveLanguageWorker?.Pluralize(s) ?? s;
        }

        [CanBeNull]
        public static string AsOperator(this ComparisonTypes type)
        {
            switch (type)
            {
                case ComparisonTypes.Greater:
                    return ">";
                case ComparisonTypes.Equal:
                    return "==";
                case ComparisonTypes.Less:
                    return "<";
                case ComparisonTypes.GreaterEqual:
                    return ">=";
                case ComparisonTypes.LessEqual:
                    return "<=";
                default:
                    return null;
            }
        }
    }
}
