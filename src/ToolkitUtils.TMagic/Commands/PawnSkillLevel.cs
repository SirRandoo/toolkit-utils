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
// with ToolkitUtils.TMagic. If not, see <https://www.gnu.org/licenses/>.
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using Mono.Reflection;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.TMagic.Extensions;
using TorannMagic;
using Verse;
using ExecutionContext = ToolkitUtils.Mod.ExecutionContext;
using IResult = ToolkitUtils.Mod.IResult;

namespace ToolkitUtils.TMagic.Commands;

[Group("levelskills")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PawnSkillLevel(ExecutionContext context, TranslationService service) : CommandGroup
{
    private static PropertyInfo _mightDataAbilityPointsProperty = AccessTools.Property(typeof(MightData), nameof(MightData.MightAbilityPoints));
    private static FieldInfo _mightDataAbilityPointsBackingField = _mightDataAbilityPointsProperty.GetBackingField();
    private static PropertyInfo _magicDataAbilityPointsProperty = AccessTools.Property(typeof(MagicData), nameof(MagicData.MagicAbilityPoints));
    private static FieldInfo _magicDataAbilityPointsBackingField = _magicDataAbilityPointsProperty.GetBackingField();
    private static AccessTools.FieldRef<MagicData, int> _magicDataAbilityPointsRef = AccessTools.FieldRefAccess<MagicData, int>(_magicDataAbilityPointsBackingField);
    private static AccessTools.FieldRef<MightData, int> _mightDataAbilityPointsRef = AccessTools.FieldRefAccess<MightData, int>(_mightDataAbilityPointsBackingField);

    /// <summary>
    ///     Attempts to level up the specified might skill for the invoking player's pawn. The method checks various
    ///     conditions such as the availability of the pawn, affinity for might, skill points, and whether the skill has
    ///     already reached its maximum level. If all conditions are met, the skill is leveled up, and the appropriate skill
    ///     points are deducted from the pawn.
    /// </summary>
    /// <param name="power">The target might power to level up.</param>
    /// <returns>
    ///     A task containing an <see cref="IResult" /> indicating whether the operation succeeded or the reason for
    ///     failure. Potential failure results include: no pawn is associated with the invoker, the pawn lacks might affinity,
    ///     insufficient skill points, or the skill is already at the maximum level.
    /// </returns>
    [Command("might")]
    public async Task<Result> LevelMightSkillAsync(MightPower power)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(service.GetPawnRequiredError(context.Invoker));

        var comp = pawn.TryGetComp<CompAbilityUserMight>();

        if (comp is not { IsMightUser: true, }) return Result.Fail(service.GetNoMightAffinityError());
        if (comp.MightData.MightAbilityPoints <= 0) return Result.Fail(service.GetNoAbilityPointsError());
        if (power.level >= power.maxLevel) return Result.Fail(service.GetAbilityMaxedError(power));
        if (comp.MightData.MightAbilityPoints < power.costToLevel) return Result.Fail(service.GetInsufficientAbilityPointsError(power, comp.MightData.MightAbilityPoints));

        int oldMightAbilityPoints = comp.MightData.MightAbilityPoints;
        ref int mightAbilityPoints = ref _mightDataAbilityPointsRef(comp.MightData);

        Interlocked.CompareExchange(ref mightAbilityPoints, oldMightAbilityPoints - power.costToLevel, oldMightAbilityPoints);
        Interlocked.Increment(ref power.level);

        return await context.SendReplyAsync(service.FormatAbilityPowerGrew(power));
    }

    /// <summary>
    ///     Attempts to level up the specified might skill for the invoking player's pawn. This method verifies a series
    ///     of prerequisites such as the presence of an associated pawn, the pawn's might affinity, availability of skill
    ///     points, and whether the skill has reached its maximum level. If all conditions are satisfied, the skill is leveled
    ///     up and the associated skill points are deducted.
    /// </summary>
    /// <param name="power">
    ///     The might skill to be leveled up. Includes details such as the current level, maximum level, and
    ///     the skill point cost required to level up.
    /// </param>
    /// <returns>
    ///     A task that resolves to an <see cref="IResult" /> indicating the outcome of the operation. Possible failure
    ///     scenarios include: the absence of an associated pawn, lack of might affinity, not having enough skill points to
    ///     level up, or the skill already being at its maximum level.
    /// </returns>
    [Command("might")]
    public async Task<Result> LevelMightSkillAsync(MightPowerSkill power)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(service.GetPawnRequiredError(context.Invoker));

        var comp = pawn.TryGetComp<CompAbilityUserMight>();

        if (comp is not { IsMightUser: true, }) return Result.Fail(service.GetNoMightAffinityError());
        if (comp.MightData.MightAbilityPoints <= 0) return Result.Fail(service.GetNoAbilityPointsError());
        if (power.level >= power.levelMax) return Result.Fail(service.GetAbilityMaxedError(power));
        if (comp.MightData.MightAbilityPoints < power.costToLevel) return Result.Fail(service.GetInsufficientAbilityPointsError(power, comp.MightData.MightAbilityPoints));

        int oldMightAbilityPoints = comp.MightData.MightAbilityPoints;
        ref int mightAbilityPoints = ref _mightDataAbilityPointsRef(comp.MightData);

        Interlocked.CompareExchange(ref mightAbilityPoints, oldMightAbilityPoints - power.costToLevel, oldMightAbilityPoints);
        Interlocked.Increment(ref power.level);

        await context.SendReplyAsync(service.FormatAbilityPowerGrew(power));

        return Result.Ok();
    }

    /// <summary>
    ///     Attempts to level up the specified magic skill for the invoking player's pawn. The method validates several
    ///     conditions including the presence of the pawn, the pawn's affinity for magic, the availability of skill points, and
    ///     whether the selected skill has reached its maximum level. If all conditions are satisfied, the skill is leveled up,
    ///     and the appropriate skill points are deducted.
    /// </summary>
    /// <param name="power">The target magic power to level up.</param>
    /// <returns>
    ///     A task containing an <see cref="IResult" /> indicating the success or failure of the operation. Possible
    ///     failure reasons include: the pawn is not associated with the invoker, the pawn lacks magic affinity, insufficient
    ///     skill points, or the skill is already at its maximum level.
    /// </returns>
    [Command("magic")]
    public async Task<IResult> LevelMagicSkillAsync(MagicPower power)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(service.GetPawnRequiredError(context.Invoker));

        var comp = pawn.TryGetComp<CompAbilityUserMagic>();

        if (comp is not { IsMagicUser: true, }) return Result.Fail(service.GetNoMagicAffinityError());
        if (comp.MagicData.MagicAbilityPoints <= 0) return Result.Fail(service.GetNoAbilityPointsError());
        if (power.level >= power.maxLevel) return Result.Fail(service.GetAbilityMaxedError(power));
        if (comp.MagicData.MagicAbilityPoints < power.costToLevel) return Result.Fail(service.GetInsufficientAbilityPointsError(power, comp.MagicData.MagicAbilityPoints));

        int oldMagicAbilityPoints = comp.MagicData.MagicAbilityPoints;
        ref int magicAbilityPoints = ref _magicDataAbilityPointsRef(comp.MagicData);

        Interlocked.CompareExchange(ref magicAbilityPoints, oldMagicAbilityPoints - power.costToLevel, oldMagicAbilityPoints);
        Interlocked.Increment(ref power.level);

        await context.SendReplyAsync(service.FormatAbilityPowerGrew(power));

        return Result.Ok();
    }

    /// <summary>
    ///     Attempts to level up the specified magic skill for the invoking player's pawn. The method evaluates several
    ///     conditions, including the presence of the pawn, their affinity for magic, the number of available skill points, and
    ///     whether the skill has already reached its maximum allowable level. If all requirements are satisfied, the skill is
    ///     leveled up and the necessary skill points are deducted.
    /// </summary>
    /// <param name="power">The target magic power skill to level up.</param>
    /// <returns>
    ///     A task containing an <see cref="IResult" /> that indicates success or specifies the reason for failure.
    ///     Possible failure cases include: no associated pawn for the invoker, lack of magic affinity, insufficient skill
    ///     points, or the skill already being at its maximum level.
    /// </returns>
    [Command("magic")]
    public async Task<IResult> LevelMagicSkillAsync(MagicPowerSkill power)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(service.GetPawnRequiredError(context.Invoker));

        var comp = pawn.TryGetComp<CompAbilityUserMagic>();

        if (comp is not { IsMagicUser: true, }) return Result.Fail(service.GetNoMagicAffinityError());
        if (comp.MagicData.MagicAbilityPoints <= 0) return Result.Fail(service.GetNoAbilityPointsError());
        if (power.level >= power.levelMax) return Result.Fail(service.GetAbilityMaxedError(power));
        if (comp.MagicData.MagicAbilityPoints < power.costToLevel) return Result.Fail(service.GetInsufficientAbilityPointsError(power, comp.MagicData.MagicAbilityPoints));

        int oldMagicAbilityPoints = comp.MagicData.MagicAbilityPoints;
        ref int magicAbilityPoints = ref _magicDataAbilityPointsRef(comp.MagicData);

        Interlocked.CompareExchange(ref magicAbilityPoints, oldMagicAbilityPoints - power.costToLevel, oldMagicAbilityPoints);
        Interlocked.Increment(ref power.level);

        await context.SendReplyAsync(service.FormatAbilityPowerGrew(power));

        return Result.Ok();
    }
}
