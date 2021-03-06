﻿// ToolkitUtils
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
    [UsedImplicitly]
    [StaticConstructorOnStartup]
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

        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            if (!PurchaseHelper.TryGetPawn(msg.Username, out Pawn pawn))
            {
                msg.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            List<string> queries = CommandFilter.Parse(msg.Message).Skip(1).Select(q => q.ToToolkit()).ToList();

            if (queries.Count <= 0)
            {
                queries.AddRange(DefaultStats);
            }

            List<StatDef> container = queries
               .Select(
                    query => DefDatabase<StatDef>.AllDefs.Where(d => d.showOnHumanlikes && d.showOnPawns)
                       .FirstOrDefault(
                            d => d.label.ToToolkit().EqualsIgnoreCase(query) || d.defName.EqualsIgnoreCase(query)
                        )
                )
               .Where(stat => stat != null)
               .ToList();

            if (container.Count <= 0)
            {
                return;
            }

            string[] parts = container
               .Select(s => ResponseHelper.JoinPair(s.LabelCap, s.ValueToString(pawn.GetStatValue(s))))
               .ToArray();

            msg.Reply(parts.SectionJoin());
        }
    }
}
