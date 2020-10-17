using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class PurchaseHelper
    {
        public static bool CanAfford(this Viewer v, int price)
        {
            return v.GetViewerCoins() >= price || ToolkitSettings.UnlimitedCoins;
        }

        public static bool IsResearched(this ThingDef thing)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.True
            var researched = true;

            if (!thing.recipeMaker?.researchPrerequisite?.IsFinished ?? false)
            {
                researched = false;
            }

            if (thing.recipeMaker?.researchPrerequisites?.All(p => !p.IsFinished) ?? false)
            {
                researched = false;
            }

            if (!thing.IsResearchFinished)
            {
                researched = false;
            }

            return BuyItemSettings.mustResearchFirst && researched;
        }

        public static List<ResearchProjectDef> GetUnfinishedPrerequisites(this ThingDef thing)
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

        public static bool Stackable(this ThingDef thing)
        {
            return thing.stackLimit > 1;
        }

        public static string ToToolkit(this string t)
        {
            return t.Replace(" ", "").ToLower();
        }

        public static int CalculateStorePrice(this ThingDef d)
        {
            return Math.Max(1, Convert.ToInt32(d.BaseMarketValue * 10.0f / 6.0f));
        }

        public static bool ToChance(this int value)
        {
            return value > 0 && Rand.Chance(value / 100f);
        }

        public static bool TryGetPawn(string viewer, out Pawn pawn, bool kidnapped = false)
        {
            pawn = CommandBase.GetOrFindPawn(viewer.ToLowerInvariant(), kidnapped);
            return pawn != null;
        }
    }
}
