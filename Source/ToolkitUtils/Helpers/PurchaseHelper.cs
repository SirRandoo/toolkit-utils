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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.IncidentSettings;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers;

public static class PurchaseHelper
{
    public static bool CanAfford(this Viewer v, int price) => v.GetViewerCoins() >= price || ToolkitSettings.UnlimitedCoins;

    public static bool IsResearched(this ThingDef thing) => BuyItemSettings.mustResearchFirst && thing.GetUnfinishedPrerequisites().Count <= 0;

    public static List<ResearchProjectDef> GetUnfinishedPrerequisites(this ThingDef thing)
    {
        var projects = new List<ResearchProjectDef>();

        if (thing.recipeMaker?.researchPrerequisite?.IsFinished == false)
        {
            projects.Add(thing.recipeMaker.researchPrerequisite);
        }

        if (thing.recipeMaker?.researchPrerequisites?.Any(p => !p.IsFinished) == true)
        {
            projects.AddRange(thing.recipeMaker.researchPrerequisites.Where(p => !p.IsFinished));
        }

        if (thing.researchPrerequisites?.Any(p => !p.IsFinished) == true)
        {
            projects.AddRange(thing.researchPrerequisites.Where(p => !p.IsFinished));
        }

        projects.RemoveDuplicates();

        return projects;
    }

    public static bool Stackable(this ThingDef thing) => thing.stackLimit > 1;

    public static string ToToolkit(this string t) => t.Replace(" ", "").ToLower();

    public static int CalculateStorePrice(this ThingDef d) => Math.Max(1, Convert.ToInt32(d.BaseMarketValue * 10.0f / 6.0f));

    public static bool ToChance(this int value) => value > 0 && Rand.Chance(value / 100f);

    public static bool TryGetPawn(string viewer, [NotNullWhen(true)] out Pawn? pawn, bool kidnapped = false)
    {
        pawn = CommandBase.GetOrFindPawn(viewer.ToLowerInvariant(), kidnapped);

        return pawn != null;
    }

    public static int GetMaximumPurchaseAmount(this Viewer viewer, int cost) => Mathf.FloorToInt(viewer.GetViewerCoins() / (float)cost);

    public static void Charge(this Viewer viewer, StoreIncident incident)
    {
        if (!ToolkitSettings.UnlimitedCoins)
        {
            viewer.TakeViewerCoins(incident.cost);
        }

        viewer.CalculateNewKarma(incident.karmaType, incident.cost);
    }

    public static void Charge(this Viewer viewer, StoreIncident incident, float weight)
    {
        if (!ToolkitSettings.UnlimitedCoins)
        {
            viewer.TakeViewerCoins(incident.cost);
        }

        viewer.CalculateNewKarma(incident.karmaType, Mathf.CeilToInt(incident.cost * weight));
    }

    public static void Charge(this Viewer viewer, int cost, KarmaType karmaType)
    {
        if (!ToolkitSettings.UnlimitedCoins)
        {
            viewer.TakeViewerCoins(cost);
        }

        viewer.CalculateNewKarma(karmaType, cost);
    }

    public static void Charge(this Viewer viewer, int cost, float weight, KarmaType karmaType)
    {
        if (!ToolkitSettings.UnlimitedCoins)
        {
            viewer.TakeViewerCoins(cost);
        }

        viewer.CalculateNewKarma(karmaType, Mathf.CeilToInt(cost * weight));
    }

    [ContractAnnotation("thing:notnull => true,projects:notnull; thing:notnull => false,projects:null")]
    public static bool TryGetUnfinishedPrerequisites(ThingDef thing, out List<ResearchProjectDef> projects)
    {
        projects = thing.GetUnfinishedPrerequisites();

        return BuyItemSettings.mustResearchFirst && projects.Count > 0;
    }

    public static bool CanBeStuff(this ThingDef thing, ThingDef? stuff)
    {
        if (stuff is null)
        {
            return false;
        }

        foreach (ThingDef possible in GenStuff.AllowedStuffsFor(thing).Where(s => s.defName.Equals(stuff.defName)))
        {
            if (!Data.ItemData.TryGetValue(possible.defName, out ItemData data))
            {
                return true;
            }

            if (data!.IsStuffAllowed)
            {
                return true;
            }
        }

        return false;
    }

    public static int GetItemPrice(this ThingItem thing, ThingItem? stuff)
    {
        if (stuff is null)
        {
            return thing.Cost;
        }

        int cost = thing.Cost;

        if (!thing.Thing!.MadeFromStuff)
        {
            return cost;
        }

        if (stuff.Thing!.smallVolume)
        {
            cost += thing.Thing.CostStuffCount * stuff.Cost * 10;
        }
        else
        {
            cost += thing.Thing.CostStuffCount * stuff.Cost;
        }

        cost = Mathf.CeilToInt(cost * 1.05f);

        return cost;
    }

    public static int GetItemPrice(this ThingItem thing, ThingItem? stuff, QualityCategory quality)
    {
        int price = GetItemPrice(thing, stuff);

        switch (quality)
        {
            case QualityCategory.Awful:
                price = Mathf.CeilToInt(price * Item.AwfulMultiplier);

                break;
            case QualityCategory.Poor:
                price = Mathf.CeilToInt(price * Item.PoorMultiplier);

                break;
            case QualityCategory.Normal:
                price = Mathf.CeilToInt(price * Item.NormalMultiplier);

                break;
            case QualityCategory.Good:
                price = Mathf.CeilToInt(price * Item.GoodMultiplier);

                break;
            case QualityCategory.Excellent:
                price = Mathf.CeilToInt(price * Item.ExcellentMultiplier);

                break;
            case QualityCategory.Masterwork:
                price = Mathf.CeilToInt(price * Item.MasterworkMultiplier);

                break;
            case QualityCategory.Legendary:
                price = Mathf.CeilToInt(price * Item.LegendaryMultiplier);

                break;
        }

        return Mathf.CeilToInt(price * 1.1f);
    }

    public static void TrySetQuality(this Thing thing, QualityCategory? quality)
    {
        var comp = thing.TryGetComp<CompQuality>();
        comp?.SetQuality(quality ?? QualityUtility.GenerateQualityTraderItem(), ArtGenerationContext.Outsider);
    }

    public static Thing MakeThing(ThingDef thing, ThingDef stuff, QualityCategory? quality)
    {
        Thing t = ThingMaker.MakeThing(thing, stuff);
        TrySetQuality(t, quality);

        return t;
    }

    public static Thing MakeThing(ThingDef thing, ThingDef stuff) => ThingMaker.MakeThing(thing, stuff);

    public static Thing MakeThing(ThingDef thing) => ThingMaker.MakeThing(thing);

    public static void SpawnItem(ThingDef thing, Map map, IntVec3 position)
    {
        SpawnItem(position, map, MakeThing(thing));
    }

    public static void SpawnItem(ThingDef thing, ThingDef stuff, Map map, IntVec3 position)
    {
        SpawnItem(position, map, MakeThing(thing, stuff));
    }

    public static void SpawnItem(ThingDef thing, ThingDef stuff, QualityCategory? quality, Map map, IntVec3 position)
    {
        SpawnItem(position, map, MakeThing(thing, stuff, quality));
    }

    public static void SpawnItem(IntVec3 position, Map map, Thing item)
    {
        var hatcher = item.TryGetComp<CompHatcher>();

        if (hatcher != null)
        {
            hatcher.hatcheeFaction = Faction.OfPlayer;
        }

        if (Current.Game.GetComponent<Coordinator>()?.TrySpawnItem(map, item) == true)
        {
            return;
        }

        TradeUtility.SpawnDropPod(position, map, item);
    }

    public static void SpawnPawn(Pawn pawn, IntVec3 location, Map map)
    {
        if (Current.Game.GetComponent<Coordinator>()?.TrySpawnPawn(map, pawn) == true)
        {
            return;
        }

        GenSpawn.Spawn(pawn, location, map);
    }

    public static ViewerState GetState(this Viewer viewer) => new ViewerState { Coins = viewer.GetViewerCoins(), Karma = viewer.GetViewerKarma() };

    public static bool TryMultiply(int i1, int i2, out int result)
    {
        try
        {
            result = checked(i1 * i2);
        }
        catch (OverflowException)
        {
            result = 0;

            return false;
        }

        return true;
    }
}
