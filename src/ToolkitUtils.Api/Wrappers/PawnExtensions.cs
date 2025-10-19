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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>Provides extension methods for performing asynchronous operations on RimWorld Pawn objects.</summary>
[PublicAPI]
public static class PawnExtensions
{
    /// <summary>Asynchronously sets the father of the specified pawn.</summary>
    /// <param name="pawn">The pawn for which the father will be set.</param>
    /// <param name="father">The pawn to be set as the father.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SetFatherAsync(this Pawn pawn, Pawn father)
    {
        await MainThreadExtensions.OnMainAsync(ParentRelationUtility.SetFather, pawn, father);
    }

    public static async Task SetMotherAsync(this Pawn pawn, Pawn mother)
    {
        await MainThreadExtensions.OnMainAsync(ParentRelationUtility.SetMother, pawn, mother);
    }

    /// <summary>Asynchronously sets the faction of the specified pawn to the given faction on the main thread.</summary>
    /// <param name="pawn">The pawn whose faction is to be set.</param>
    /// <param name="faction">The new faction to assign to the pawn.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SetFactionDirectAsync(this Pawn pawn, Faction faction)
    {
        await MainThreadExtensions.OnMainAsync(pawn.SetFactionDirect, faction);
    }

    /// <summary>Asynchronously sets the faction of the specified Pawn.</summary>
    /// <param name="pawn">The Pawn whose faction is to be set.</param>
    /// <param name="faction">The Faction to assign to the Pawn.</param>
    /// <param name="recruiter">An optional Pawn who acts as the recruiter for the faction change.</param>
    /// <returns>An asynchronous task representing the operation.</returns>
    public static async Task SetFactionAsync(this Pawn pawn, Faction faction, Pawn? recruiter = null)
    {
        await MainThreadExtensions.OnMainAsync(pawn.SetFaction, faction, recruiter);
    }

    /// <summary>Asynchronously kills the specified pawn with optional damage information and culprit context.</summary>
    /// <param name="pawn">The pawn to be killed.</param>
    /// <param name="info">Optional damage information specifying how the pawn should be killed.</param>
    /// <param name="culprit">Optional Hediff associated with the pawn's death.</param>
    /// <returns>A task representing the asynchronous kill operation.</returns>
    public static async Task KillAsync(this Pawn pawn, DamageInfo? info, Hediff? culprit = null)
    {
        await MainThreadExtensions.OnMainAsync(pawn.Kill, info, culprit);
    }

    /// <summary>Asynchronously retrieves the list of relations a pawn has with another pawn.</summary>
    /// <param name="pawn">The pawn whose relations are being queried.</param>
    /// <param name="other">The other pawn with whom the relations are being evaluated.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains a list of
    ///     <see cref="PawnRelationDef" /> representing the relations between the two pawns.
    /// </returns>
    public static async Task<List<PawnRelationDef>> GetRelationsAsync(this Pawn pawn, Pawn other)
    {
        return await MainThreadExtensions.OnMainAsync(GetRelations, pawn, other);

        List<PawnRelationDef> GetRelations(Pawn subject, Pawn target)
        {
            var container = new List<PawnRelationDef>();

            foreach (PawnRelationDef relation in subject.GetRelations(target)) container.Add(relation);

            return container;
        }
    }
}
