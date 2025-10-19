// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Core. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RimWorld;
using Verse;

namespace ToolkitUtils.Core.Extensions;

public static class PawnExtensions
{
    public static int CalculateTraits(this Pawn pawn)
    {
        var traitCount = 0;
        List<Trait> traits = pawn.story.traits.allTraits;

        for (var i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];

            if (trait.def.IsSexualityTrait()) continue;

            traitCount++;
        }

        return traitCount;
    }

    public static bool TryGetConflictingTrait(this Pawn pawn, TraitDef def, [NotNullWhen(returnValue: true)] out Trait? conflictingTrait)
    {
        List<Trait> traits = pawn.story.traits.allTraits;

        for (var i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];

            if (!trait.def.ConflictsWith(def) && !def.ConflictsWith(trait)) continue;

            conflictingTrait = trait;

            return true;
        }

        conflictingTrait = null;

        return false;
    }

    public static bool TryGetDuplicateTrait(this Pawn pawn, TraitDef def, [NotNullWhen(returnValue: true)] out Trait? duplicateTrait)
    {
        List<Trait> traits = pawn.story.traits.allTraits;

        for (var i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];

            if (trait.def != def) continue;

            duplicateTrait = trait;

            return true;
        }

        duplicateTrait = null;

        return false;
    }

    public static bool TryGetBackstoryViolation(this Pawn pawn, TraitDef trait, [NotNullWhen(returnValue: true)] out BackstoryDef? backstory, int degree = 0)
    {
        for (var i = 0; i < pawn.story.AllBackstories.Count; i++)
        {
            BackstoryDef def = pawn.story.AllBackstories[i];

            if (!def.DisallowsTrait(trait, degree)) continue;

            backstory = def;

            return true;
        }

        backstory = null;

        return false;
    }

    public static bool TryGetTraitGeneConflictForAddition(this Pawn pawn, TraitDef traitDef, [NotNullWhen(returnValue: true)] out Gene? conflictingGene, int degree = 0)
    {
        if (!ModLister.BiotechInstalled)
        {
            conflictingGene = null;

            return false;
        }

        List<Gene> genes = pawn.genes.GenesListForReading;

        for (var i = 0; i < genes.Count; i++)
        {
            Gene gene = genes[i];

            if (!gene.Active || gene.def.suppressedTraits is not { Count: > 0, }) continue;

            GeneticTraitData geneTraitData = gene.def.suppressedTraits.Find(match: g => g.def == traitDef && g.degree == degree);

            if (geneTraitData == null) continue;

            conflictingGene = gene;

            return true;
        }

        conflictingGene = null;

        return false;
    }

    public static bool TryGetTraitGeneConflictForRemoval(this Pawn pawn, TraitDef traitDef, [NotNullWhen(returnValue: true)] out Gene? conflictingGene, int degree = 0)
    {
        if (!ModLister.BiotechInstalled)
        {
            conflictingGene = null;

            return false;
        }

        List<Gene> genes = pawn.genes.GenesListForReading;

        for (var i = 0; i < genes.Count; i++)
        {
            Gene gene = genes[i];

            GeneticTraitData? geneticTraitData = gene.def.forcedTraits?.Find(match: g => g.def == traitDef && g.degree == degree);

            if (geneticTraitData == null) continue;

            conflictingGene = gene;

            return true;
        }

        conflictingGene = null;

        return false;
    }
}
