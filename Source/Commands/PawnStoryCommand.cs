using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnStoryCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabCharacter".Translate()));
                return;
            }

            var parts = new List<string>
            {
                string.Join(
                    ", ".Translate(),
                    pawn.story.AllBackstories.Select(b => b.title.CapitalizeFirst()).ToArray()
                )
            };

            switch (pawn.gender)
            {
                case Gender.Female:
                    parts.Add("♀".AltText("Female".Translate()));
                    break;
                case Gender.Male:
                    parts.Add("♂".AltText("Male".Translate()));
                    break;
                case Gender.None:
                    parts.Add("⚪".AltText("NoneLower".Translate()));
                    break;
            }

            var workTags = pawn.story.DisabledWorkTagsBackstoryAndTraits;

            if (workTags == WorkTags.None)
            {
                parts.Add($"{"IncapableOf".Translate().RawText}: {"NoneLower".Translate().RawText}");
            }
            else
            {
                var filteredTags = pawn.story.DisabledWorkTagsBackstoryAndTraits.GetAllSelectedItems<WorkTags>()
                    .Where(t => t != WorkTags.None)
                    .Select(t => t.LabelTranslated().CapitalizeFirst())
                    .ToArray();

                parts.Add($"{"IncapableOf".Translate().RawText}: {string.Join(", ", filteredTags)}");
            }

            parts.Add(
                $"{"Traits".Translate().RawText}: {string.Join(", ", pawn.story.traits.allTraits.Select(t => t.LabelCap).ToArray())}"
            );

            message.Reply(string.Join("⎮", parts.ToArray()).WithHeader("TabCharacter".Translate()));
        }
    }
}
