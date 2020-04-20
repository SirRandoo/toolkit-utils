using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnStoryCommand : CommandBase
    {
        private Pawn pawn;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (base.CanExecute(twitchCommand))
            {
                return false;
            }

            pawn = GetOrFindPawn(twitchCommand.Username);
            
            if (pawn != null)
            {
                return true;
            }
            
            twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabCharacter".Translate()));
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
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

            twitchCommand.Reply(string.Join("⎮", parts.ToArray()).WithHeader("TabCharacter".Translate()));
        }

        public PawnStoryCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
