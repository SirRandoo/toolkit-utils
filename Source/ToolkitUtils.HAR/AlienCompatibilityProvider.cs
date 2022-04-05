using System.Collections.Generic;
using AlienRace;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace ToolkitUtils.HAR
{
    public class AlienCompatibilityProvider : IAlienCompatibilityProvider
    {
        /// <inheritdoc/>
        [NotNull]
        public string ModId => "erdelf.HumanoidAlienRaces";

        /// <inheritdoc/>
        public bool TryReassignBody(Pawn pawn)
        {
            if (!(pawn.def is ThingDef_AlienRace alienRace))
            {
                return false;
            }

            AlienPartGenerator generator = alienRace.alienRace.generalSettings.alienPartGenerator;


            if (generator.alienbodytypes.NullOrEmpty() || generator.alienbodytypes.Contains(pawn.story.bodyType))
            {
                return true;
            }

            List<BodyTypeDef> bodyTypes = generator.alienbodytypes.ListFullCopy();

            if (bodyTypes.Count > 0)
            {
                switch (pawn.gender)
                {
                    case Gender.Male:
                        bodyTypes.Remove(BodyTypeDefOf.Female);

                        break;
                    case Gender.Female:
                        bodyTypes.Remove(BodyTypeDefOf.Male);

                        break;
                }
            }

            pawn.story.bodyType = bodyTypes.TryRandomElement(out BodyTypeDef newBody) ? newBody : BodyTypeDefOf.Thin;

            return true;
        }

        /// <inheritdoc/>
        public bool IsTraitForced(Pawn pawn, string defName, int degree)
        {
            if (!(pawn.def is ThingDef_AlienRace alienRace) || alienRace.alienRace.generalSettings.forcedRaceTraitEntries.NullOrEmpty())
            {
                return false;
            }

            foreach (AlienTraitEntry entry in alienRace.alienRace.generalSettings.forcedRaceTraitEntries)
            {
                if (string.Equals(entry.defName.defName, defName) && entry.degree == degree)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool IsTraitDisallowed(Pawn pawn, string defName, int degree)
        {
            if (!(pawn.def is ThingDef_AlienRace alienRace) || alienRace.alienRace.generalSettings.disallowedTraits.NullOrEmpty())
            {
                return false;
            }

            foreach (AlienTraitEntry entry in alienRace.alienRace.generalSettings.disallowedTraits)
            {
                if (string.Equals(entry.defName.defName, defName) && entry.degree == degree)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool IsTraitAllowed(Pawn pawn, [NotNull] TraitDef traitDef, int degree = -10) =>
            !IsTraitDisallowed(pawn, traitDef.defName, degree) && !IsTraitForced(pawn, traitDef.defName, degree);
    }
}
