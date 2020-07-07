using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class TraitHelper
    {
        public static bool IsSexualityTrait(Trait trait)
        {
            return IsSexualityTrait(trait.def);
        }

        public static bool IsSexualityTrait(TraitDef trait)
        {
            if (trait.exclusionTags.Contains("SexualOrientation"))
            {
                return true;
            }

            return trait.Equals(TraitDefOf.Gay)
                   || trait.Equals(TraitDefOf.Bisexual)
                   || trait.Equals(TraitDefOf.Asexual);
        }

        public static bool MultiCompare(XmlTrait trait, string input)
        {
            string label = trait.Name;

            if (input.ToToolkit().EqualsIgnoreCase(label.ToToolkit()))
            {
                return true;
            }

            return Unrichify.StripTags(label)
                .ToToolkit()
                .EqualsIgnoreCase(Unrichify.StripTags(input).StripTags().ToToolkit());
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
                        CanAdd = true,
                        CanRemove = true,
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
                        CanAdd = true,
                        CanRemove = true,
                        Name = t.label
                    }
                )
                .ToArray();
        }
    }
}
