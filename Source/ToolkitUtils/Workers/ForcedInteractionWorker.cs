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

using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers;

/// <summary>
///     A somewhat similar worker to RimWorld's
///     <see cref="InteractionWorker"/>, but without RimWorld's internal
///     limiters.
/// </summary>
public static class ForcedInteractionWorker
{
    /// <summary>
    ///     Instructs a <see cref="Pawn"/> to interact with another
    ///     <see cref="Pawn"/> according to the given
    ///     <see cref="InteractionDef"/>.
    /// </summary>
    /// <param name="pawn">The pawn to do the interaction</param>
    /// <param name="recipient">The pawn being interacted with</param>
    /// <param name="interaction">
    ///     The <see cref="InteractionDef"/> of the
    ///     interaction that will take place
    /// </param>
    /// <returns>
    ///     The interaction string returned by RimWorld's interaction
    ///     worker.
    /// </returns>
    [CanBeNull]
    public static string InteractWith(Pawn pawn, Pawn recipient, InteractionDef interaction)
    {
        if (pawn == recipient)
        {
            return null;
        }

        var extraSentencePacks = new List<RulePackDef>();

        if (interaction.initiatorThought != null)
        {
            Pawn_InteractionsTracker.AddInteractionThought(pawn, recipient, interaction.initiatorThought);
        }

        if (interaction.recipientThought != null && recipient.needs.mood != null)
        {
            Pawn_InteractionsTracker.AddInteractionThought(recipient, pawn, interaction.recipientThought);
        }

        bool isSocialFight = recipient.RaceProps.Humanlike && recipient.interactions.CheckSocialFightStart(interaction, pawn);

        string letterText = null;
        string letterLabel = null;
        LetterDef letterDef = null;
        LookTargets lookTargets = null;

        if (!isSocialFight)
        {
            interaction.Worker.Interacted(pawn, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
        }

        MoteMaker.MakeInteractionBubble(
            pawn,
            recipient,
            interaction.interactionMote,
            interaction.GetSymbol(pawn.Faction, pawn.Ideo),
            interaction.GetSymbolColor(pawn.Faction)
        );

        if (isSocialFight)
        {
            extraSentencePacks.Add(RulePackDefOf.Sentence_SocialFightStarted);
        }

        var entry = new PlayLogEntry_Interaction(interaction, pawn, recipient, extraSentencePacks);
        Find.PlayLog.Add(entry);

        string text = RichTextHelper.StripTags(entry.ToGameStringFromPOV(pawn));

        if (letterDef == null)
        {
            return MakeFirstPerson(pawn.LabelShort, text);
        }

        if (!letterText.NullOrEmpty())
        {
            text = text + "\n\n" + RichTextHelper.StripTags(letterText);
        }

        Find.LetterStack.ReceiveLetter(letterLabel, text, letterDef, lookTargets ?? pawn);

        return MakeFirstPerson(pawn.LabelShort, text.Replace("\n\n", " "));
    }

    private static string MakeFirstPerson(string username, string text)
    {
        var builder = new StringBuilder();
        string you = "TKUtils.Interaction.You".Localize();
        var shouldCapitalize = false;

        foreach (string word in text.Split(' '))
        {
            bool isUser = word.EqualsIgnoreCase(username);

            if (!isUser)
            {
                builder.Append(word).Append(" ");

                if (word.EndsWith("!") || word.EndsWith("?") || word.EndsWith("."))
                {
                    shouldCapitalize = true;
                }

                continue;
            }

            if (builder.Length <= 0)
            {
                shouldCapitalize = true;
            }

            if (shouldCapitalize)
            {
                builder.Append(you.CapitalizeFirst());
                shouldCapitalize = false;
            }
            else
            {
                builder.Append(you);
            }

            builder.Append(" ");
        }

        return builder.ToString();
    }
}