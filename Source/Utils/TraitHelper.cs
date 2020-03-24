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

        public static bool IsSpecialTrait(TraitDef trait)
        {
            if(trait.Equals(TraitDefOf.Gay)) return true;
            if(trait.Equals(TraitDefOf.Bisexual)) return true;

            return false;
        }

        public static bool MultiCompare(BuyableTrait trait, string input)
        {
            var label = trait.label;

            if(input.Equals(label)) return true;

            if(TKSettings.RichText)
            {
                label = RichTextUnparser.IsRichText(label) ? RichTextUnparser.StripTags(label) : label;
                input = RichTextUnparser.IsRichText(input) ? RichTextUnparser.StripTags(input) : input;

                if(label.EqualsIgnoreCase(input))
                {
                    return true;
                }
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
