using RimWorld;
using TwitchToolkit.IncidentHelpers.Traits;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class TraitHelper
    {
        public static bool IsSpecialTrait(Trait trait)
        {
            if (!TkSettings.Sexuality)
            {
                return false;
            }

            return trait.def.Equals(TraitDefOf.Gay)
                   || trait.def.Equals(TraitDefOf.Bisexual)
                   || trait.def.Equals(TraitDefOf.Asexual);
        }

        public static bool IsSpecialTrait(TraitDef trait)
        {
            if (!TkSettings.Sexuality)
            {
                return false;
            }
            
            return trait.Equals(TraitDefOf.Gay)
                   || trait.Equals(TraitDefOf.Bisexual)
                   || trait.Equals(TraitDefOf.Asexual);
        }

        public static bool MultiCompare(BuyableTrait trait, string input)
        {
            var label = trait.label;

            if (input.ToToolkit().EqualsIgnoreCase(label.ToToolkit()))
            {
                return true;
            }

            if (!TkSettings.RichText)
            {
                return false;
            }

            label = Unrichify.IsRichText(label) ? Unrichify.StripTags(label) : label;
            input = Unrichify.IsRichText(input) ? Unrichify.StripTags(input) : input;

            return label.ToToolkit().EqualsIgnoreCase(input.ToToolkit());
        }

        public static string ToToolkit(this string t)
        {
            return t.Replace(" ", "").ToLower();
        }
    }
}
