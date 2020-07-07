using System.Collections.Generic;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ResponseHelper
    {
        public const string OuterGroupSeparator = "⎮";
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
        public const string MaleGlyph = "♀";
        public const string FemaleGlyph = "♂";
        public const string GenderlessGlyph = "⚪";

        public static string JoinPair(string key, string value)
        {
            return $"{key}: {value}";
        }

        public static string Join(this IEnumerable<string> l, string separator)
        {
            return string.Join(separator, l);
        }

        public static string GroupedJoin(this IEnumerable<string> l)
        {
            return string.Join(OuterGroupSeparator, l);
        }

        public static string SectionJoin(this IEnumerable<string> l)
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
    }
}
