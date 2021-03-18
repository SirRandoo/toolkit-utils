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

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class PurchaseHelper
    {
        public static bool CanAfford([NotNull] this Viewer v, int price)
        {
            return v.GetViewerCoins() >= price || ToolkitSettings.UnlimitedCoins;
        }

        public static bool IsResearched(this ThingDef thing)
        {
            return BuyItemSettings.mustResearchFirst && thing.GetUnfinishedPrerequisites().Count <= 0;
        }

        [NotNull]
        public static List<ResearchProjectDef> GetUnfinishedPrerequisites([NotNull] this ThingDef thing)
        {
            var projects = new List<ResearchProjectDef>();

            if (!thing.recipeMaker?.researchPrerequisite?.IsFinished ?? false)
            {
                projects.Add(thing.recipeMaker.researchPrerequisite);
            }

            if (thing.recipeMaker?.researchPrerequisites?.Any(p => !p.IsFinished) ?? false)
            {
                projects.AddRange(thing.recipeMaker.researchPrerequisites.Where(p => !p.IsFinished));
            }

            if (thing.researchPrerequisites?.Any(p => !p.IsFinished) ?? false)
            {
                projects.AddRange(thing.researchPrerequisites.Where(p => !p.IsFinished));
            }

            projects.RemoveDuplicates();
            return projects;
        }

        public static bool Stackable([NotNull] this ThingDef thing)
        {
            return thing.stackLimit > 1;
        }

        [NotNull]
        public static string ToToolkit([NotNull] this string t)
        {
            return t.Replace(" ", "").ToLower();
        }

        public static int CalculateStorePrice([NotNull] this ThingDef d)
        {
            return Math.Max(1, Convert.ToInt32(d.BaseMarketValue * 10.0f / 6.0f));
        }

        public static bool ToChance(this int value)
        {
            return value > 0 && Rand.Chance(value / 100f);
        }

        public static bool TryGetPawn([NotNull] string viewer, [CanBeNull] out Pawn pawn, bool kidnapped = false)
        {
            pawn = CommandBase.GetOrFindPawn(viewer.ToLowerInvariant(), kidnapped);
            return pawn != null;
        }

        public static int GetMaximumPurchaseAmount([NotNull] this Viewer viewer, int cost)
        {
            return Mathf.FloorToInt(viewer.GetViewerCoins() / (float) cost);
        }

        public static void Charge([NotNull] this Viewer viewer, [NotNull] StoreIncident incident)
        {
            if (!ToolkitSettings.UnlimitedCoins)
            {
                viewer.TakeViewerCoins(incident.cost);
            }

            viewer.CalculateNewKarma(incident.karmaType, incident.cost);
        }

        public static void Charge([NotNull] this Viewer viewer, [NotNull] StoreIncident incident, float weight)
        {
            if (!ToolkitSettings.UnlimitedCoins)
            {
                viewer.TakeViewerCoins(incident.cost);
            }

            viewer.CalculateNewKarma(incident.karmaType, Mathf.CeilToInt(incident.cost * weight));
        }

        public static void Charge([NotNull] this Viewer viewer, int cost, KarmaType karmaType)
        {
            if (!ToolkitSettings.UnlimitedCoins)
            {
                viewer.TakeViewerCoins(cost);
            }

            viewer.CalculateNewKarma(karmaType, cost);
        }

        public static void Charge([NotNull] this Viewer viewer, int cost, float weight, KarmaType karmaType)
        {
            if (!ToolkitSettings.UnlimitedCoins)
            {
                viewer.TakeViewerCoins(cost);
            }

            viewer.CalculateNewKarma(karmaType, Mathf.CeilToInt(cost * weight));
        }
    }
}
