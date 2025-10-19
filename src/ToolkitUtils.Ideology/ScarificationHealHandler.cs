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
using Verse;

namespace ToolkitUtils.Ideology;

[UsedImplicitly]
public class ScarificationHealHandler : IHealProvider
{
    private const string ModId = "Ludeon.Ideology";

    /// <inheritdoc />
    public string Id { get; init; } = "sirrandoo.tku:compatibility.ideo.scarification";

    /// <inheritdoc />
    public string Name { get; init; } = "Scarification Compatibility Provider";

    /// <inheritdoc />
    public int Priority => 2;

    /// <inheritdoc />
    public string[] RequiredMods { get; } = [ModId,];

    /// <inheritdoc />
    public Task<Result> CanHealAsync(Hediff hediff)
    {
        bool isScarification = hediff.def == HediffDefOf.Scarification;

        if (!isScarification) return Task.FromResult(Result.Ok());

        Ideo ideo = hediff.pawn.Ideo;

        if (ideo.HasPrecept(IdeologyDefs.Scarification_Minor) || ideo.HasPrecept(IdeologyDefs.Scarification_Heavy) || ideo.HasPrecept(IdeologyDefs.Scarification_Extreme))
            return Task.FromResult(Errors.InvalidTarget);

        return Task.FromResult(Result.Ok());
    }

    /// <inheritdoc />
    public Task<Result> CanHealAsync(Pawn pawn, BodyPartRecord record) => Task.FromResult(Errors.CannotHealBodyPart);

    /// <inheritdoc />
    public Task<Result> HealAsync(Hediff hediff) => Task.FromResult(Errors.CannotHeal);

    /// <inheritdoc />
    public Task<Result> HealAsync(Pawn pawn, BodyPartRecord record) => Task.FromResult(Errors.CannotHealBodyPart);

    /// <inheritdoc />
    public Task<Result> TryResurrectAsync(Pawn pawn) => Task.FromResult(Errors.CannotResurrect);

    private static class Errors
    {
        public static readonly Result CannotHeal = Result.Fail("The scarification compatibility provider doesn't support healing.");
        public static readonly Result CannotResurrect = Result.Fail("The scarification compatibility provider doesn't support resurrecting.");
        public static readonly Result CannotHealBodyPart = Result.Fail("The scarification compatibility provider doesn't support healing body parts.");
        public static readonly Result InvalidTarget = Result.Fail("Pawn has no healable injuries, or the colony does not practice scarification.");
    }
}
