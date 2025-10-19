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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using ToolkitUtils.Mod.Services;
using TwitchToolkit;
using UnityEngine;
using Verse;
using Logger = NLog.Logger;
using Viewer = ToolkitUtils.Mod.Data.Viewer;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnInteraction(ExecutionContext context) : CommandGroup
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger<PawnInteraction>();

    [Command("insult")]
    public async Task<Result> InsultAsync(Viewer viewer)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        Pawn viewerPawn = ViewerPawnRegistry.Get(viewer.Id) ?? await GetRandomPawnAsync(pawn);

        return await ExecuteInteractionAsync(pawn, viewerPawn, InteractionDefOf.Insult, isBad: true);
    }

    [Command("chat")]
    public async Task<Result> ChatAsync(Viewer viewer)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        Pawn viewerPawn = ViewerPawnRegistry.Get(viewer.Id) ?? await GetRandomPawnAsync(pawn);

        return await ExecuteInteractionAsync(pawn, viewerPawn, InteractionDefOf.Chitchat);
    }

    [Command("flirt")]
    public async Task<Result> FlirtAsync(Viewer viewer)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        Pawn viewerPawn = ViewerPawnRegistry.Get(viewer.Id) ?? await GetRandomPawnAsync(pawn);

        return await ExecuteInteractionAsync(pawn, viewerPawn, InteractionDefOf.RomanceAttempt);
    }

    [Command("deepchat")]
    public async Task<Result> DeepChatAsync(Viewer viewer)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        Pawn viewerPawn = ViewerPawnRegistry.Get(viewer.Id) ?? await GetRandomPawnAsync(pawn);

        return await ExecuteInteractionAsync(pawn, viewerPawn, InteractionDefOf.DeepTalk);
    }

    private async Task<Result> ExecuteInteractionAsync(Pawn pawn, Pawn otherPawn, InteractionDef interaction, bool isBad = false)
    {
        string? result = await MainThreadExtensions.OnMainAsync(ForcedInteractionWorker.InteractWith, pawn, otherPawn, interaction);

        if (string.IsNullOrWhiteSpace(result)) return Result.Fail();

        if (isBad)
        {
            int originalKarma = context.Invoker.Karma;
            int newKarma = Mathf.Max(context.Invoker.Karma - (int)Mathf.Ceil(context.Invoker.Karma * 0.1f), ToolkitSettings.KarmaMinimum);

            context.Invoker.RemoveKarma(originalKarma - newKarma);
        }

        return Result.Ok(result!);
    }

    private async Task<Pawn> GetRandomPawnAsync(Pawn invokerPawn)
    {
        return await RouterService.Instance.RouteToMainAsync(func: p => Find.ColonistBar.Entries.Where(e => e.pawn != p).RandomElement().pawn, invokerPawn);
    }

    /// <summary>
    ///     Provides methods for managing forced interactions between pawns in RimWorld, bypassing the game's internal
    ///     interaction limiters.
    /// </summary>
    public static class ForcedInteractionWorker
    {
        /// <summary>
        ///     Instructs a <see cref="Pawn" /> to interact with another <see cref="Pawn" /> according to the given
        ///     <see cref="InteractionDef" />.
        /// </summary>
        /// <param name="pawn">The pawn to do the interaction</param>
        /// <param name="recipient">The pawn being interacted with</param>
        /// <param name="interaction">The <see cref="InteractionDef" /> of the interaction that will take place</param>
        /// <returns>The interaction string returned by RimWorld's interaction worker.</returns>
        public static string? InteractWith(Pawn pawn, Pawn recipient, InteractionDef interaction)
        {
            if (pawn == recipient) return null;

            var extraSentencePacks = new List<RulePackDef>();

            if (interaction.initiatorThought != null) Pawn_InteractionsTracker.AddInteractionThought(pawn, recipient, interaction.initiatorThought);
            if (interaction.recipientThought != null && recipient.needs.mood != null) Pawn_InteractionsTracker.AddInteractionThought(recipient, pawn, interaction.recipientThought);

            bool isSocialFight = recipient.RaceProps.Humanlike && recipient.interactions.CheckSocialFightStart(interaction, pawn);

            string? letterText = null;
            string? letterLabel = null;
            LetterDef? letterDef = null;
            LookTargets? lookTargets = null;

            if (!isSocialFight) interaction.Worker.Interacted(pawn, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);

            MoteMaker.MakeInteractionBubble(pawn, recipient, interaction.interactionMote, interaction.GetSymbol(pawn.Faction, pawn.Ideo), interaction.GetSymbolColor(pawn.Faction));

            if (isSocialFight) extraSentencePacks.Add(RulePackDefOf.Sentence_SocialFightStarted);

            var entry = new PlayLogEntry_Interaction(interaction, pawn, recipient, extraSentencePacks);
            Find.PlayLog.Add(entry);

            string text = RichTextHelper.StripTags(entry.ToGameStringFromPOV(pawn));

            if (letterDef == null) return MakeFirstPerson(pawn.LabelShort, text);
            if (!letterText.NullOrEmpty()) text = text + "\n\n" + RichTextHelper.StripTags(letterText!);

            Find.LetterStack.ReceiveLetter(letterLabel, text, letterDef, lookTargets ?? pawn);

            return MakeFirstPerson(pawn.LabelShort, text.Replace(oldValue: "\n\n", newValue: " "));
        }

        private static string MakeFirstPerson(string username, string text)
        {
            var builder = new StringBuilder();
            string you = TranslationExtensions.FromKey("TKUtils.Responses.PawnInteraction.You");
            var shouldCapitalize = false;

            foreach (string word in text.Split(' '))
            {
                bool isUser = word.EqualsIgnoreCase(username);

                if (!isUser)
                {
                    builder.Append(word).Append(" ");

                    if (word.EndsWith("!") || word.EndsWith("?") || word.EndsWith(".")) shouldCapitalize = true;

                    continue;
                }

                if (builder.Length <= 0) shouldCapitalize = true;

                if (shouldCapitalize)
                {
                    builder.Append(you.CapitalizeFirst());
                    shouldCapitalize = false;
                }
                else
                    builder.Append(you);

                builder.Append(" ");
            }

            return builder.ToString();
        }
    }
}
