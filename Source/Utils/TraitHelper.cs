using RimWorld;

using TwitchToolkit.IncidentHelpers.Traits;

using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class TraitHelper
    {
        public static bool IsSpecialTrait(Trait trait)
        {
            if(!TKSettings.Sexuality) return false;
            if(trait.def.Equals(TraitDefOf.Gay)) return true;
            if(trait.def.Equals(TraitDefOf.Bisexual)) return true;
            if(trait.def.Equals(TraitDefOf.Asexual)) return true;

            return false;
        }

        public static bool IsSpecialTrait(TraitDef trait)
        {
            if(!TKSettings.Sexuality) return false;
            if(trait.Equals(TraitDefOf.Gay)) return true;
            if(trait.Equals(TraitDefOf.Bisexual)) return true;
            if(trait.Equals(TraitDefOf.Asexual)) return true;

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

            return false;
        }
    }
}
