using System.Collections;
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
            return "TKUtils.Formats.KeyValue".Translate(key, value);
        }

        public static string Join(this IEnumerable l, string separator)
        {
            return string.Join(separator, l);
        }

        public static string GroupedJoin(this IEnumerable l)
        {
            return string.Join(OuterGroupSeparator, l);
        }

        public static string SectionJoin(this IEnumerable l)
        {
            return string.Join(", ", l);
        }
    }
}
