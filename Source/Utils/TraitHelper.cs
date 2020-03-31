using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class TraitHelper
    {
        public static bool IsSexualityTrait(Trait trait)
        {
            if (!TkSettings.Sexuality)
            {
                return false;
            }

            return trait.def.Equals(TraitDefOf.Gay)
                   || trait.def.Equals(TraitDefOf.Bisexual)
                   || trait.def.Equals(TraitDefOf.Asexual);
        }

        public static bool IsSexualityTrait(TraitDef trait)
        {
            if (!TkSettings.Sexuality)
            {
                return false;
            }

            return trait.Equals(TraitDefOf.Gay)
                   || trait.Equals(TraitDefOf.Bisexual)
                   || trait.Equals(TraitDefOf.Asexual);
        }

        public static bool MultiCompare(XmlTrait trait, string input)
        {
            var label = trait.Name;

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

        public static IEnumerable<XmlTrait> GetEffectiveTraits(TraitDef trait)
        {
            if (trait.degreeDatas == null)
            {
                return new[]
                {
                    new XmlTrait
                    {
                        DefName = trait.defName,
                        Degree = 0,
                        Enabled = true,
                        Name = trait.label,
                        AddPrice = 3500,
                        RemovePrice = 5500
                    }
                };
            }

            return trait.degreeDatas.Select(
                    t => new XmlTrait
                    {
                        DefName = trait.defName,
                        Degree = t.degree,
                        AddPrice = 3500,
                        RemovePrice = 5500,
                        Enabled = true,
                        Name = t.label
                    }
                )
                .ToArray();
        }
    }
}
