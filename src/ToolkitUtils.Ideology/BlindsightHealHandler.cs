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
// with ToolkitUtils.Ideology. If not, see <https://www.gnu.org/licenses/>.
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Ideology;

[UsedImplicitly]
public class BlindsightHealHandler(RouterService router) : IHealProvider
{
    private const string IdeologyId = "Ludeon.Ideology";

    /// <inheritdoc />
    public string Id { get; init; } = IdeologyId;

    /// <inheritdoc />
    public string Name { get; init; } = "Blindsight Compatibility Provider";

    /// <inheritdoc />
    public int Priority => 2;

    /// <inheritdoc />
    public string[] RequiredMods { get; } =
    {
        IdeologyId,
    };

    /// <inheritdoc />
    public async Task<Result> CanHealAsync(Hediff hediff)
    {
        bool hasAnyIdeoWithMeme = await router.RouteToMainAsync(() => Find.FactionManager.OfPlayer.ideos.HasAnyIdeoWithMeme(IdeologyDefs.Blindsight));

        if (!hasAnyIdeoWithMeme || hediff.def != HediffDefOf.MissingBodyPart || hediff.Part.def != BodyPartDefOf.Eye) return Result.Ok();

        // No valid part could be found to heal.
        return Result.Fail();
    }

    /// <inheritdoc />
    public async Task<Result> CanHealAsync(Pawn pawn, BodyPartRecord record)
    {
        bool hasAnyIdeoWithMeme = await router.RouteToMainAsync(() => Find.FactionManager.OfPlayer.ideos.HasAnyIdeoWithMeme(IdeologyDefs.Blindsight));

        if (!hasAnyIdeoWithMeme || record.def != BodyPartDefOf.Eye) return Result.Ok();

        // No valid part could be found to heal.
        return Result.Fail();
    }

    /// <inheritdoc />
    public Task<Result> HealAsync(Hediff hediff) =>
        // The blindsight compatibility provider doesn't support healing pawns.
        Task.FromResult(Result.Fail());

    /// <inheritdoc />
    public Task<Result> HealAsync(Pawn pawn, BodyPartRecord record) =>
        // The blindsight compatibility provider doesn't support healing pawns.
        Task.FromResult(Result.Fail());

    /// <inheritdoc />
    public Task<Result> TryResurrectAsync(Pawn pawn) =>
        // The blindsight compatibility provider doesn't support resurrecting pawns.
        Task.FromResult(Result.Fail());
}
