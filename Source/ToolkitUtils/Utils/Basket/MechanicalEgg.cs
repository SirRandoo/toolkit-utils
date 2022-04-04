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

using System;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class MechanicalEgg : EasterEgg
    {
        public override bool IsPossible(StoreIncident incident, Viewer viewer) =>
            incident == IncidentDefOf.BuyPawn && viewer.username.Equals("scavenging_mechanic", StringComparison.InvariantCultureIgnoreCase);

        public override void Execute([NotNull] Viewer viewer, Pawn pawn)
        {
            if (!viewer.username.Equals("scavenging_mechanic", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (!pawn.story.traits.HasTrait(TraitDefOf.Transhumanist))
            {
                if (pawn.story.traits.allTraits.Count >= AddTraitSettings.maxTraits)
                {
                    Trait trait = pawn.story.traits.allTraits.RandomElement();
                    TraitHelper.RemoveTraitFromPawn(pawn, trait);
                }

                TraitHelper.GivePawnTrait(pawn, new Trait(TraitDefOf.Transhumanist));
            }

            if (ResearchProjectDef.Named("Bionics")?.IsFinished == true)
            {
                ApplyBionics(pawn);

                return;
            }

            if (ResearchProjectDef.Named("SimpleProsthetics")?.IsFinished == true)
            {
                ApplyProsthetics(pawn);

                return;
            }

            ApplyMedieval(pawn);
        }

        private static void ApplyBionics([NotNull] Pawn pawn)
        {
            var chance = 1f;

            foreach (BodyPartRecord part in pawn.RaceProps.body.AllParts.InRandomOrder())
            {
                if (!Rand.Chance(chance))
                {
                    continue;
                }

                ApplyBionicPart(pawn, part.def);
                chance *= Rand.Range(0.4f, 0.8f);
            }
        }

        private static void ApplyProsthetics([NotNull] Pawn pawn)
        {
            var chance = 1f;

            foreach (BodyPartRecord part in pawn.RaceProps.body.AllParts.InRandomOrder())
            {
                if (!Rand.Chance(chance))
                {
                    continue;
                }

                ApplySimplePart(pawn, part.def);
                chance *= Rand.Range(0.4f, 0.8f);
            }
        }

        private static void ApplyMedieval([NotNull] Pawn pawn)
        {
            var chance = 1f;

            foreach (BodyPartRecord part in pawn.RaceProps.body.AllParts.InRandomOrder())
            {
                if (!Rand.Chance(chance))
                {
                    continue;
                }

                ApplyMedievalPart(pawn, part.def);
                chance *= Rand.Range(0.4f, 0.8f);
            }
        }

        private static void ApplyBionicPart([NotNull] Pawn pawn, [NotNull] BodyPartDef bodyPart)
        {
            switch (bodyPart.defName)
            {
                case "Arm":
                    ApplyPart(pawn, HediffDefOf.BionicArm, bodyPart);

                    return;
                case "Leg":
                    ApplyPart(pawn, HediffDefOf.BionicLeg, bodyPart);

                    return;
                case "Eye":
                    ApplyPart(pawn, HediffDefOf.BionicEye, bodyPart);

                    return;
                case "Ear":
                    ApplyPart(pawn, HediffDef.Named("BionicEar"), bodyPart);

                    return;
                case "Spine":
                    ApplyPart(pawn, HediffDef.Named("BionicSpine"), bodyPart);

                    return;
                case "Heart":
                    ApplyPart(pawn, HediffDef.Named("BionicHeart"), bodyPart);

                    return;
                case "Stomach":
                    ApplyPart(pawn, HediffDef.Named("BionicStomach"), bodyPart);

                    return;
            }
        }

        private static void ApplySimplePart([NotNull] Pawn pawn, [NotNull] BodyPartDef bodyPart)
        {
            switch (bodyPart.defName)
            {
                case "Arm":
                    ApplyPart(pawn, HediffDefOf.SimpleProstheticArm, bodyPart);

                    return;
                case "Leg":
                    ApplyPart(pawn, HediffDefOf.SimpleProstheticLeg, bodyPart);

                    return;
                case "Heart":
                    ApplyPart(pawn, HediffDef.Named("SimpleProstheticHeart"), bodyPart);

                    return;
                case "Ear":
                    ApplyPart(pawn, HediffDef.Named("CochlearImplant"), bodyPart);

                    return;
            }

            if (bodyPart == BodyPartDefOf.Arm)
            {
                ApplyPart(pawn, HediffDefOf.SimpleProstheticArm, bodyPart);
            }
            else if (bodyPart == BodyPartDefOf.Leg)
            {
                ApplyPart(pawn, HediffDefOf.SimpleProstheticLeg, bodyPart);
            }
        }

        private static void ApplyMedievalPart([NotNull] Pawn pawn, [NotNull] BodyPartDef bodyPart)
        {
            switch (bodyPart.defName)
            {
                case "Leg":
                    ApplyPart(pawn, HediffDef.Named("PegLeg"), bodyPart);

                    return;
            }
        }

        private static void ApplyPart([NotNull] Pawn pawn, [CanBeNull] HediffDef hediff, BodyPartDef bodyPart)
        {
            if (hediff == null)
            {
                return;
            }

            BodyPartRecord part = pawn.RaceProps.body.GetPartsWithDef(bodyPart)
               .InRandomOrder()
               .FirstOrDefault(p => !pawn.health.hediffSet.HasHediff(HediffDefOf.BionicArm, p));

            if (part == null)
            {
                return;
            }

            pawn.health.AddHediff(hediff, part);
        }
    }
}
