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
        private static readonly List<string> DefaultStats;

        static PawnStats()
        {
            DefaultStats = new List<string>
            {
                "MeleeDPS",
                "MeleeHitChance",
                "MeleeArmorPenetration",
                "MeleeDodgeChance",
                "ShootingAccuracyPawn",
                "AimingDelayFactor",
                "IncomingDamageFactor"
            };
        }

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            List<string> queries =
                CommandFilter.Parse(twitchMessage.Message).Skip(1).Select(q => q.ToToolkit()).ToList();

            if (queries.Count <= 0)
            {
                queries.AddRange(DefaultStats);
                ;
            }

            var container = new List<StatDef>();

            foreach (string query in queries)
            {
                StatDef stat = DefDatabase<StatDef>.AllDefs.Where(d => d.showOnHumanlikes && d.showOnPawns)
                   .FirstOrDefault(
                        d => d.label.ToToolkit().EqualsIgnoreCase(query) || d.defName.EqualsIgnoreCase(query)
                    );

                if (stat != null)
                {
                    container.Add(stat);
                }
            }

            if (!container.Any())
            {
                return;
            }

            string[] parts = container
               .Select(s => ResponseHelper.JoinPair(s.LabelCap, s.ValueToString(pawn.GetStatValue(s))))
               .ToArray();

            twitchMessage.Reply(parts.SectionJoin());
        }
    }
}
