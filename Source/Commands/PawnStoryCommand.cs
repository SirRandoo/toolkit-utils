using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnStoryCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabCharacter".Translate()));
                return;
            }

            var parts = new List<string>
            {
                $"{"Backstory".Translate().RawText}: {string.Join(", ", pawn.story.AllBackstories.Select(b => b.title.CapitalizeFirst()).ToArray())}"
            };

            var isRoyal = pawn.royalty?.MostSeniorTitle != null;
            switch (pawn.gender)
            {
                case Gender.Female:
                    parts.Add((isRoyal ? "👸" : "♀").AltText("Female".Translate()));
                    break;
                case Gender.Male:
                    parts.Add((isRoyal ? "🤴" : "♂").AltText("Male".Translate()));
                    break;
                case Gender.None:
                    parts.Add((isRoyal ? "👑" : "⚪").AltText("NoneLower".Translate()));
                    break;
                default:
                    parts.Add(isRoyal ? "👑" : "");
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
                $"{"Traits".Translate().RawText}: {string.Join(", ", pawn.story.traits.allTraits.Select(t => t.LabelCap.StripTags()).ToArray())}"
            );

            twitchMessage.Reply(string.Join("⎮", parts.ToArray()).WithHeader("TabCharacter".Translate()));
        }
    }
}
