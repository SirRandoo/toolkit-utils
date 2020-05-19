using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [StaticConstructorOnStartup]
    public class PawnStatsCommand : CommandBase
    {
        internal static readonly Dictionary<string, string> StatRegistry = new Dictionary<string, string>();

        static PawnStatsCommand()
        {
            var stats = DefDatabase<StatCategoryDef>.AllDefsListForReading;

            foreach (var stat in stats)
            {
                StatRegistry[stat.defName.ToToolkit().ToLowerInvariant()] = stat.label;
                StatRegistry[stat.label.ToToolkit().ToLowerInvariant()] = stat.label;
            }
        }

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var category = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrDefault();

            if (category.NullOrEmpty())
            {
                return;
            }

            if (!StatRegistry.TryGetValue(category.ToToolkit().ToLowerInvariant(), out var categoryDef))
            {
                return;
            }

            var stats = DefDatabase<StatDef>.AllDefsListForReading
                .Where(d => d.showOnHumanlikes && d.showOnPawns)
                .Where(d => d.category != null && d.category.label.EqualsIgnoreCase(categoryDef))
                .ToList();

            if (!stats.Any())
            {
                twitchMessage.Reply("TKUtils.Responses.PawnStats.None".Translate());
                return;
            }

            var parts = stats
                .Select(s => "TKUtils.Formats.KeyValue".Translate(s.LabelCap, s.ValueToString(pawn.GetStatValue(s))))
                .Select(s => s.RawText)
                .ToArray();

            twitchMessage.Reply(string.Join(", ", parts));
        }
    }
}
