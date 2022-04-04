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
        public const string OuterGroupSeparator = "\u23AE";
        public const string OuterGroupSeparatorAlt = " | ";
        public const string InfinityGlyph = "\u221E";
        public const string CoinGlyph = "\uD83D\uDCB0";
        public const string KarmaGlyph = "\u2696";
        public const string IncomeGlyph = "\uD83D\uDCC8";
        public const string DebtGlyph = "\uD83D\uDCC9";
        public const string TemperatureGlyph = "\uD83C\uDF21";
        public const string BleedingGlyph = "\uD83E\uDE78";
        public const string BandageGlyph = "\uD83E\uDE79";
        public const string DaggerGlyph = "\uD83D\uDDE1";
        public const string PanGlyph = "\uD83C\uDF73";
        public const string FireGlyph = "\uD83D\uDD25";
        public const string DazedGlyph = "\uD83D\uDCAB";
        public const string GhostGlyph = "\uD83D\uDC7B";
        public const string LightningGlyph = "\u26A1";
        public const string AboutToBreakGlyph = "\uD83E\uDD2C";
        public const string OnEdgeGlyph = "\uD83D\uDE20";
        public const string MagicGlyph = "\uD83D\uDD2E";
        public const string StressedGlyph = "\uD83D\uDE23";
        public const string NeutralGlyph = "\uD83D\uDE10";
        public const string ContentGlyph = "\uD83D\uDE42";
        public const string HappyGlyph = "\uD83D\uDE0A";
        public const string BleedingSafeGlyphs = "\uD83E\uDE78\u231B";
        public const string BleedingBadGlyphs = "\uD83E\uDE78\u23F3";
        public const string ForbiddenGlyph = "\uD83D\uDEAB";
        public const string PrincessGlyph = "\uD83D\uDC78";
        public const string PrinceGlyph = "\uD83E\uDD34";
        public const string CrownGlyph = "\uD83D\uDC51";
        public const string MaleGlyph = "\u2642";
        public const string FemaleGlyph = "\u2640";
        public const string GenderlessGlyph = "\u26AA";
        public const string NotEqualGlyph = "\u2260";
        public const string ArrowGlyph = "\u2192";

        [NotNull] public static string JoinPair(string key, string value) => $"{key}: {value}";

        [NotNull] public static string Join([NotNull] this IEnumerable<string> l, string separator) => string.Join(separator, l);

        [NotNull] public static string GroupedJoin([NotNull] this IEnumerable<string> l) => string.Join(OuterGroupSeparator.AltText(OuterGroupSeparatorAlt), l);

        [NotNull] public static string SectionJoin([NotNull] this IEnumerable<string> l) => string.Join(", ", l);

        public static string Pluralize(this string s) => Find.ActiveLanguageWorker?.Pluralize(s) ?? s;

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
