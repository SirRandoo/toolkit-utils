using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class RationalRomance
    {
        public static readonly bool Active;

        private static readonly List<string> TraitDefs =
            new List<string> {"Polyamorous", "Straight", "Gay", "Bisexual", "Asexual"};

        static RationalRomance()
        {
            Active = ModLister.GetModWithIdentifier("Mlie.RationalRomance") != null;
        }

        public static bool IsTraitDisabled(TraitDef traitDef)
        {
            return TraitDefs.Contains(traitDef.defName);
        }
    }
}
