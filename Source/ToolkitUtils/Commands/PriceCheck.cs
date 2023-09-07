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
using SirRandoo.ToolkitUtils.Defs;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands;

[UsedImplicitly]
public class PriceCheck : CommandBase
{
    private string _invoker;

    private static bool CanRemoveTrait => IncidentDefOf.RemoveTrait.cost > 0 || IncidentDefOf.ReplaceTrait.cost > 0 || IncidentDefOf.ClearTraits.cost > 0
        || IncidentDefOf.SetTraits.cost > 0;

    private static bool CanAddTrait => IncidentDefOf.AddTrait.cost > 0 || IncidentDefOf.ReplaceTrait.cost > 0 || IncidentDefOf.SetTraits.cost > 0;

    private static bool AreTraitsDisabled => IncidentDefOf.AddTrait.cost <= 0 && IncidentDefOf.RemoveTrait.cost <= 0 && IncidentDefOf.ReplaceTrait.cost <= 0
        && IncidentDefOf.ClearTraits.cost <= 0 && IncidentDefOf.SetTraits.cost <= 0;

    public override void RunCommand(ITwitchMessage twitchMessage)
    {
        _invoker = twitchMessage.Username;
        string[] segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
        string category = segments.FirstOrFallback("");
        string query = segments.Skip(1).FirstOrFallback("");
        string quantity = segments.Skip(2).FirstOrFallback("1");

        if (!Lookup.Index.TryGetValue(category.ToLowerInvariant(), out Lookup.Category result))
        {
            quantity = query;
            query = category;
        }

        if (result == Lookup.Category.Disease || result == Lookup.Category.Skill)
        {
            quantity = query;
            query = category;
        }

        PerformLookup(result, query, quantity);
    }

    private void NotifyLookupComplete(string result)
    {
        if (result.NullOrEmpty())
        {
            return;
        }

        MessageHelper.ReplyToUser(_invoker, result);
    }

    private void PerformAnimalLookup(string query, int quantity)
    {
        PawnKindDef kindDef = DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(
            i => i.RaceProps.Animal && (i.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
        );

        if (kindDef == null)
        {
            return;
        }

        ThingItem item = Data.Items.FirstOrDefault(i => i.DefName.EqualsIgnoreCase(kindDef.defName));

        if (item == null || item.Cost <= 0)
        {
            return;
        }

        if (!PurchaseHelper.TryMultiply(item.Cost, quantity, out int result))
        {
            MessageHelper.ReplyToUser(_invoker, "TKUtils.Overflowed".Localize());

            return;
        }

        NotifyLookupComplete("TKUtils.Price.Limited".LocalizeKeyed(kindDef.defName.CapitalizeFirst(), result.ToString("N0")));
    }

    private void PerformEventLookup(string query)
    {
        StoreIncident result = DefDatabase<StoreIncident>.AllDefs.FirstOrDefault(
            i => i.cost > 0 && (i.abbreviation.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
        );

        if (result == null)
        {
            return;
        }

        EventTypes eventType = result.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;

        switch (eventType)
        {
            case EventTypes.Default:
                NotifyLookupComplete("TKUtils.Price.Limited".LocalizeKeyed(result.abbreviation.CapitalizeFirst(), result.cost.ToString("N0")));

                return;
            case EventTypes.Item:
            case EventTypes.PawnKind:
            case EventTypes.Trait:
                NotifyLookupComplete("TKUtils.Price.Overridden".LocalizeKeyed(eventType.ToString()));

                return;
            case EventTypes.Misc:
                NotifyLookupComplete("TKUtils.Price.External".Localize());

                return;
            case EventTypes.Variable:
                NotifyLookupComplete(
                    new[]
                    {
                        "TKUtils.Price.Variables".LocalizeKeyed(result.abbreviation.CapitalizeFirst(), result.cost.ToString("N0")),
                        "TKUtils.Price.External".Localize()
                    }.GroupedJoin()
                );

                return;
        }
    }

    private void PerformItemLookup(string query, int quantity)
    {
        var worker = ArgWorker.CreateInstance(query);

        if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy item) || !item.IsValid() || item.Thing?.Cost <= 0)
        {
            return;
        }

        if (item.TryGetError(out string error))
        {
            MessageHelper.ReplyToUser(_invoker, error);

            return;
        }

        int price = item.Quality.HasValue ? item.Thing!.GetItemPrice(item.Stuff, item.Quality.Value) : item.Thing!.GetItemPrice(item.Stuff);

        if (!PurchaseHelper.TryMultiply(price, quantity, out int total))
        {
            MessageHelper.ReplyToUser(_invoker, "TKUtils.Overflowed".Localize());

            return;
        }

        NotifyLookupComplete("TKUtils.Price.Limited".LocalizeKeyed(item.AsString().CapitalizeFirst(), total.ToString("N0")));
    }

    private void PerformLookup(Lookup.Category category, string query, string amount)
    {
        if (!int.TryParse(amount, out int quantity))
        {
            quantity = 1;
        }

        switch (category)
        {
            case Lookup.Category.Event:
                PerformEventLookup(query);

                return;
            case Lookup.Category.Item:
                PerformItemLookup(query, quantity);

                return;
            case Lookup.Category.Animal:
                PerformAnimalLookup(query, quantity);

                return;
            case Lookup.Category.Trait:
                PerformTraitLookup(query);

                return;
            case Lookup.Category.Kind:
                PerformKindLookup(query);

                return;
        }
    }

    private void PerformKindLookup(string query)
    {
        if (IncidentDefOf.BuyPawn.cost <= 0 || !Data.TryGetPawnKind(query, out PawnKindItem result))
        {
            return;
        }

        NotifyLookupComplete("TKUtils.Price.Limited".LocalizeKeyed(result!.Name.ToToolkit().CapitalizeFirst(), result.Cost.ToString("N0")));
    }

    private void PerformTraitLookup(string query)
    {
        if (AreTraitsDisabled || !Data.TryGetTrait(query, out TraitItem result) || (!result.CanAdd && !result.CanRemove))
        {
            return;
        }

        var parts = new List<string>();

        if (result.CanAdd && CanAddTrait)
        {
            parts.Add("TKUtils.Price.AddTrait".LocalizeKeyed(result.CostToAdd.ToString("N0")));
        }

        if (result.CanRemove && CanRemoveTrait)
        {
            parts.Add("TKUtils.Price.RemoveTrait".LocalizeKeyed(result.CostToRemove.ToString("N0")));
        }

        NotifyLookupComplete($"{result.Name.ToToolkit().CapitalizeFirst()} - {parts.Join(" / ")}");
    }
}
