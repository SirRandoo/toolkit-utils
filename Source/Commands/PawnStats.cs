using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [StaticConstructorOnStartup]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnStats : CommandBase
    {
        internal static readonly Dictionary<string, string> StatRegistry = new Dictionary<string, string>();

        static PawnStats()
        {
            List<StatCategoryDef> stats = DefDatabase<StatCategoryDef>.AllDefsListForReading;

            foreach (StatCategoryDef stat in stats)
            {
                StatRegistry[stat.defName.ToToolkit().ToLowerInvariant()] = stat.label;
                StatRegistry[stat.label.ToToolkit().ToLowerInvariant()] = stat.label;
            }
        }

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            string category = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrDefault();

            if (category.NullOrEmpty())
            {
                category = "combat";
            }

            if (!StatRegistry.TryGetValue(category.ToToolkit().ToLowerInvariant(), out string categoryDef))
            {
                return;
            }

            List<StatDef> stats = DefDatabase<StatDef>.AllDefsListForReading
               .Where(d => d.showOnHumanlikes && d.showOnPawns)
               .Where(d => d.category != null && d.category.label.EqualsIgnoreCase(categoryDef))
               .ToList();

            if (!stats.Any())
            {
                twitchMessage.Reply("TKUtils.PawnStats.None".Localize());
                return;
            }

            HideDisabledStats(stats);

            string[] parts = stats
               .Select(s => ResponseHelper.JoinPair(s.LabelCap, s.ValueToString(pawn.GetStatValue(s))))
               .ToArray();

            twitchMessage.Reply(parts.SectionJoin());
        }

        private static void HideDisabledStats(IList<StatDef> stats)
        {
            for (var index = 0; index < stats.Count; index++)
            {
                StatDef stat = stats[index];
                TkSettings.StatSetting setting =
                    TkSettings.StatSettings.FirstOrDefault(s => s.StatDef.EqualsIgnoreCase(stat.defName));

                if (setting == null || setting.Enabled)
                {
                    continue;
                }

                try
                {
                    stats.RemoveAt(index);
                }
                catch (IndexOutOfRangeException) { }
            }
        }
    }
}
