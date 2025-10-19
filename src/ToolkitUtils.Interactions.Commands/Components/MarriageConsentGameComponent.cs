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
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Concurrent;
using JetBrains.Annotations;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using Verse;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToolkitUtils.Interactions.Commands.Components;

/// <summary>Represents a component that manages marriage-related interactions within the game.</summary>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class MarriageGameComponent(Game _) : GameComponent
{
    private readonly ConcurrentDictionary<string, string> _proposalTracker = new();

    /// <summary>Determines if the specified person has a pending marriage proposal.</summary>
    /// <param name="viewerId">The name of the person to check for a pending marriage proposal.</param>
    /// <returns>
    ///     A <see cref="Result{FiancePair}" /> containing the fiancés involved in a pending proposal if one exists, or an
    ///     error result if no proposal is pending for the specified person.
    /// </returns>
    public Result<FiancePair> HasPendingProposal(string viewerId) =>
        _proposalTracker.TryGetValue(viewerId, out string? fianceId)
            ? Result.Ok(
                new FiancePair
                {
                    ProposerId = viewerId, ProposedId = fianceId,
                }
            )
            : Result.Fail<FiancePair>(new Translation("You haven't been proposed to.".MarkNotTranslated()));

    /// <summary>Initiates a marriage proposal between two individuals in the game.</summary>
    /// <param name="proposerId">The name of the person making the proposal.</param>
    /// <param name="proposedId">The name of the person being proposed to.</param>
    public void Propose(string proposerId, string proposedId)
    {
        _proposalTracker.AddOrUpdate(proposerId, addValueFactory: static (_, arg) => arg, updateValueFactory: static (_, _, newValue) => newValue, proposedId);
        _proposalTracker.AddOrUpdate(proposedId, addValueFactory: static (_, arg) => arg, updateValueFactory: static (_, _, newValue) => newValue, proposerId);
    }

    /// <summary>Declines a pending marriage proposal for the specified person.</summary>
    /// <param name="proposerId">The name of the person declining the proposal.</param>
    public void DeclineProposal(string proposerId)
    {
        Result<FiancePair> pair = HasPendingProposal(proposerId);

        if (pair.IsSuccess) _proposalTracker.TryRemove(pair.Value.ProposedId, out string _);

        _proposalTracker.TryRemove(proposerId, out string _);
    }

    /// <summary>Represents a pair of individuals involved in a marriage proposal process.</summary>
    public record struct FiancePair(string ProposerId, string ProposedId);
}
