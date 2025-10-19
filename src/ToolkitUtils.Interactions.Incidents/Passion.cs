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
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

/// <summary>A command group that allows users to purchase and manage skill passions for their pawns.</summary>
[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PassionBuyGroup : CommandGroup
{
    /// <summary>
    ///     A command group that allows users to manage and modify skill passions for their pawns, including incrementing,
    ///     decrementing, or shuffling passions associated with specific skills.
    /// </summary>
    [Group("passion")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class PassionGroup(ILetterService letterService, PassionSettings settings, ExecutionContext context) : CommandGroup
    {
        /// <summary>Adds a passion for a specified skill to a pawn associated with a viewer.</summary>
        /// <param name="skill">The skill for which the passion is to be added.</param>
        /// <returns>A task representing the IResult of the operation.</returns>
        [Command(name: "increment", "add")]
        public async Task<Result> IncrementPassionAsync(SkillDef skill)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

            SkillRecord record = pawn.skills.skills.Find(s => s.def == skill);

            if (record.TotallyDisabled) return Result.Fail(new Translation($"The skill '{skill.label}' is disabled".MarkNotTranslated()));
            if ((int)record.passion > (int)Passion.Major)
                return Result.Fail(new Translation($"The skill '{skill.label}' has a passion level of {record.passion} which isn't supported.".MarkNotTranslated()));
            if (record.passion is Passion.Major) return Result.Fail(new Translation("The skill is already at the highest passion level.".MarkNotTranslated()));

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.AddPassion.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.AddPassion.cost, context.Invoker.Coins));

            Result result = await PerformPassionIncrementAsync(pawn, record);
            transaction.Value.PostTransaction();

            return result;
        }

        /// <summary>Performs an asynchronous increment of a skill passion for a given pawn based on specified conditions.</summary>
        /// <param name="pawn">The pawn whose skill passion is being incremented.</param>
        /// <param name="record">The skill record of the pawn being incremented.</param>
        /// <returns>A task representing the asynchronous operation, with a IResult indicating success or failure.</returns>
        private async Task<Result> PerformPassionIncrementAsync(Pawn pawn, SkillRecord record)
        {
            if (!settings.UseRandomness)
            {
                record.passion = (Passion)Mathf.Clamp((int)record.passion + 1, min: 0, max: 2);
                await letterService.SendPassionIncreaseLetterAsync(pawn, record.def);

                return Result.Ok(new Translation($"Your passion was in {record.def.label} was increased to {record.passion}.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.FailChance))
            {
                await letterService.SendPassionFailedLetterAsync(pawn, record.def);

                return Result.Ok(new Translation($"Your passion in {record.def.label} didn't increase.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.HopChance) && TryGetSkillCandidate(pawn, record, out SkillRecord? skill))
            {
                skill.passion = (Passion)Mathf.Clamp((int)skill.passion + 1, min: 0, max: 2);
                await letterService.SendPassionIncreaseLetterAsync(pawn, skill.def);

                return Result.Ok(new Translation($"Your passion in {skill.def.label} was increased to {skill.passion}.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.InverseChance) && record.passion is Passion.Minor or Passion.Major)
            {
                record.passion = (Passion)Mathf.Clamp((int)record.passion - 1, (int)Passion.None, (int)Passion.Major);
                await letterService.SendPassionDecreaseLetterAsync(pawn, record.def);

                return Result.Ok(new Translation($"Your passion in {record.def.label} was decreased to {record.passion}.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.InverseChance) && ThreadSafeRandom.Chance(settings.HopChance) && TryGetSkillCandidate(pawn, record, out skill, forDecrease: true))
            {
                record.passion = (Passion)Mathf.Clamp((int)skill.passion - 1, (int)Passion.None, (int)Passion.Major);
                await letterService.SendPassionDecreaseLetterAsync(pawn, skill.def);

                return Result.Ok(new Translation($"Your passion in {skill.def.label} was decreased to {record.passion}.".MarkNotTranslated()));
            }

            record.passion = (Passion)Mathf.Clamp((int)record.passion + 1, min: 0, max: 2);
            await letterService.SendPassionIncreaseLetterAsync(pawn, record.def);

            return Result.Ok(new Translation($"Your passion in {record.def.label} was increased to {record.passion}."));
        }

        /// <summary>Attempts to remove a passion from a specified skill for a given viewer's pawn.</summary>
        /// <param name="skill">The skill from which the passion is to be removed.</param>
        /// <returns>A IResult indicating the success or failure of the passion removal operation.</returns>
        [Command(name: "decrement", "remove")]
        public async Task<IResult> DecrementPassionAsync(SkillDef skill)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

            SkillRecord? record = pawn.skills.skills.Find(r => r.def == skill);

            if (record == null || record.TotallyDisabled) return Result.Fail(new Translation($"Your skill {skill.label} is disabled.".MarkNotTranslated()));

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.RemovePassion.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.RemovePassion.cost, context.Invoker.Coins));

            if (record.passion is not Passion.None && (int)record.passion <= (int)Passion.Major)
            {
                Result result = await PerformPassionDecrementAsync(pawn, record);
                transaction.Value.PostTransaction();

                return result;
            }

            return Result.Fail(new Translation($"Your passion in {skill.label} is {record.passion}, which isn't supported.".MarkNotTranslated()));
        }

        /// <summary>
        ///     Decreases the passion level of a specific skill for a given pawn asynchronously, taking into account various
        ///     settings related to randomness and failure chances. Sends appropriate notifications upon success or failure.
        /// </summary>
        /// <param name="pawn">The pawn whose skill passion level is being decremented.</param>
        /// <param name="record">The skill record with the passion level to be decremented.</param>
        /// <returns>An IResult object indicating the success or failure of the operation.</returns>
        private async Task<Result> PerformPassionDecrementAsync(Pawn pawn, SkillRecord record)
        {
            if (!settings.UseRandomness)
            {
                record.passion = (Passion)Mathf.Clamp((int)record.passion - 1, (int)Passion.Major, (int)Passion.Major + 1);
                await letterService.SendPassionDecreaseLetterAsync(pawn, record.def);

                return Result.Ok(new Translation($"Your passion in {record.def.label} was decreased to {record.passion}.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.FailChance))
            {
                await letterService.SendPassionFailedLetterAsync(pawn, record.def);

                return Result.Ok(new Translation($"Your passion in {record.def.label} didn't decrease.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.HopChance) && TryGetSkillCandidate(pawn, record, out SkillRecord? skill, forDecrease: true))
            {
                skill.passion = (Passion)Mathf.Clamp((int)skill.passion - 1, (int)Passion.None, (int)Passion.Major);
                await letterService.SendPassionDecreaseLetterAsync(pawn, skill.def);

                return Result.Ok(new Translation($"Your passion in {skill.def.label} decreased.".MarkNotTranslated()));
            }

            if (ThreadSafeRandom.Chance(settings.InverseChance) && (int)record.passion < (int)Passion.Major)
            {
                record.passion = (Passion)Mathf.Clamp((int)record.passion + 1, (int)Passion.None, (int)Passion.Major);
                await letterService.SendPassionIncreaseLetterAsync(pawn, record.def);

                return Result.Ok(new Translation($"Your passion in {record.def.label} increased."));
            }

            if (ThreadSafeRandom.Chance(settings.InverseChance) && ThreadSafeRandom.Chance(settings.HopChance) && TryGetSkillCandidate(pawn, record, out skill))
            {
                skill.passion = (Passion)Mathf.Clamp((int)skill.passion + 1, (int)Passion.None, (int)Passion.Major);
                await letterService.SendPassionIncreaseLetterAsync(pawn, skill.def);

                return Result.Ok(new Translation($"Your passion in {skill.def.label} increased."));
            }

            record.passion = (Passion)Mathf.Clamp((int)record.passion - 1, (int)Passion.Major, (int)Passion.Major + 1);
            await letterService.SendPassionDecreaseLetterAsync(pawn, record.def);

            return Result.Ok(new Translation($"Your passion in {record.def.label} decreased."));
        }

        /// <summary>Tries to get a skill candidate for a given pawn and skill record based on specified conditions.</summary>
        /// <param name="pawn">The pawn to search skill records for.</param>
        /// <param name="record">The current skill record to exclude from the search.</param>
        /// <param name="skill">The output parameter that will hold the skill record if a suitable candidate is found.</param>
        /// <param name="forDecrease">A boolean indicating whether the skill search is for a decrease operation.</param>
        /// <returns>True if a suitable skill candidate is found; otherwise, false.</returns>
        private static bool TryGetSkillCandidate(Pawn pawn, SkillRecord record, [NotNullWhen(true)] out SkillRecord? skill, bool forDecrease = false)
        {
            List<SkillRecord> skills = [..pawn.skills.skills,];
            TwitchToolkit.Extensions.Shuffle(skills);

            for (var index = 0; index < skills.Count; index++)
            {
                SkillRecord skillRecord = skills[index];

                if (skillRecord.TotallyDisabled || skillRecord == record) continue;

                switch (forDecrease)
                {
                    case true when skillRecord.passion is not (Passion.None or Passion.Major):
                        skill = skillRecord;

                        return true;
                }
            }

            skill = null;

            return false;
        }

        [Command("shuffle")]
        public async Task<Result> ShufflePassionsAsync(SkillDef? skillBias = null)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

            int totalPassionLevels = pawn.skills.PassionCount;

            if (totalPassionLevels <= 0)
            {
                await context.SendReplyAsync(TranslationService.Instance.FormatNoPassions());

                return Result.Fail(TranslationService.Instance.FormatNoPassions());
            }

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.PassionShuffle.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.PassionShuffle.cost, context.Invoker.Coins));

            Result result = await PerformPassionShuffle(pawn, totalPassionLevels, skillBias);
            transaction.Value.PostTransaction();
            return result;
        }

        private async Task<Result> PerformPassionShuffle(Pawn pawn, int totalPassionLevels, SkillDef? skillBias = null)
        {
            if (skillBias != null)
            {
                SkillRecord? biasedRecord = pawn.skills.GetSkill(skillBias);

                if (biasedRecord == null)
                {
                    await context.SendReplyAsync(TranslationService.Instance.FormatInvalidQuery(skillBias.label));

                    return Result.Fail(TranslationService.Instance.FormatInvalidQuery(skillBias.label));
                }

                biasedRecord.passion = Passion.Minor;
                totalPassionLevels--;
            }

            SkillRecord[] skills = pawn.skills.skills.ToArray();

            for (var i = 0; i < skills.Length; i++)
            {
                SkillRecord record = skills[i];

                if (record.TotallyDisabled || record.def == skillBias) continue;

                record.passion = Passion.None;
            }

            for (var i = 0; i < 250; i++)
            {
                if (totalPassionLevels <= 0) break;

                int randomIndex = ThreadSafeRandom.Next(skills.Length);
                SkillRecord record = skills[randomIndex];

                var previousValue = (int)record.passion;

                record.passion = (Passion)Math.Max((int)Passion.None, Math.Min((int)record.passion + 1, (int)Passion.Major));

                if ((int)record.passion != previousValue) totalPassionLevels--;
            }

            await letterService.SendPassionShuffleLetterAsync(pawn);

            return Result.Ok(new Translation("Your passions have been shuffled".MarkNotTranslated()));
        }
    }
}

file static class PassionLetterExtensions
{
    public static Task SendPassionIncreaseLetterAsync(this ILetterService service, Pawn pawn, SkillDef skill) =>
        service.SendPositiveLetterAsync(
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Title"),
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Increase.Body").Format(
                new
                {
                    SkillName = skill.label, ViewerName = pawn.LabelShort,
                }
            ),
            pawn
        );

    public static Task SendPassionDecreaseLetterAsync(this ILetterService service, Pawn pawn, SkillDef skill) =>
        service.SendNegativeLetterAsync(
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Title"),
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Decrease.Body").Format(
                new
                {
                    SkillName = skill.label, ViewerName = pawn.LabelShort,
                }
            ),
            pawn
        );

    public static Task SendPassionShuffleLetterAsync(this ILetterService service, Pawn pawn) =>
        service.SendNeutralLetterAsync(
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Title"),
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Shuffle.Body").Format(
                new
                {
                    ViewerName = pawn.LabelShort,
                }
            ),
            pawn
        );

    public static Task SendPassionFailedLetterAsync(this ILetterService service, Pawn pawn, SkillDef skill) =>
        service.SendNegativeLetterAsync(
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Title"),
            TranslationExtensions.FromKey("TKUtils.Letters.Passion.Failed.Body").Format(
                new
                {
                    SkillName = skill.label, ViewerName = pawn.LabelShort,
                }
            ),
            pawn
        );
}
