using System;
using System.Collections.Generic;
using System.Linq;
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

            if (thing.recipeMaker?.researchPrerequisites?.All(p => !p.IsFinished) ?? false)
            {
                projects.AddRange(thing.recipeMaker.researchPrerequisites.Where(p => !p.IsFinished));
            }

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
    }
}
