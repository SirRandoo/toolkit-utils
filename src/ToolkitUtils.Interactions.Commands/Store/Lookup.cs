// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Products;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

/// <summary>
///     Represents a set of commands for performing lookup operations within the system. Provides functionality to
///     search for various entities such as skills, animals, kinds, events, traits, items, mods, and diseases.
/// </summary>
[Group("lookup")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Lookup(CommandSettings settings) : CommandGroup
{
    /// <summary>Performs a lookup for a skill based on the provided query and returns matching results.</summary>
    /// <param name="query">The search term to look up the skill by. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no results are found).
    /// </returns>
    [Command("skill")]
    public ValueTask<Result> LookupSkillAsync(string query)
    {
        var index = 0;
        var results = new string[settings.LookupLimit];

        foreach (SkillDef def in DefDatabase<SkillDef>.AllDefs)
        {
            if (string.Compare(def.defName, query, StringComparison.OrdinalIgnoreCase) == -1) continue;
            if (string.Compare(def.skillLabel, query, StringComparison.InvariantCultureIgnoreCase) == -1) continue;
            if (string.Compare(def.label, query, StringComparison.InvariantCultureIgnoreCase) == -1) continue;

            results[index++] = def.label;

            if (index >= results.Length) break;
        }

        return new ValueTask<Result>(
            string.IsNullOrEmpty(results[0]) ? Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)) : Result.Ok($"{query}: {string.Join(separator: ", ", results)}")
        );
    }

    /// <summary>Performs a lookup for an animal based on the provided query and returns matching results.</summary>
    /// <param name="query">The search term to look up the animal by. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no results are found).
    /// </returns>
    [Command("animal")]
    public ValueTask<Result> LookupAnimalAsync(string query)
    {
        var index = 0;
        var results = new string[settings.LookupLimit];

        foreach (ItemProduct product in Registries.Items.AllRegistrants)
        {
            if (product.Metadata is not {} metadata || metadata.Properties.HasFlagFast(ItemProperties.Wildlife)) continue;

            string code = product.Name;

            if (string.Compare(code, query, StringComparison.InvariantCultureIgnoreCase) == -1) continue;

            results[index++] = $"{code} (from {product.Metadata.Mod.Name})";

            if (index >= results.Length) break;
        }

        return new ValueTask<Result>(
            string.IsNullOrEmpty(results[0]) ? Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)) : Result.Ok($"{query}: {string.Join(separator: ", ", results)}")
        );
    }

    /// <summary>Performs a lookup for a kind based on the provided query and returns matching results.</summary>
    /// <param name="query">The search term to look up the kind by. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no results are found).
    /// </returns>
    [Command("kind")]
    public async Task<Result> LookupKindAsync(string query) => await LookupProductAsync(Registries.Pawns.AllRegistrants, query);

    /// <summary>Performs a lookup for an event based on the provided query and returns matching results.</summary>
    /// <param name="query">The search term used to look up the event. The query is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if no results are found.
    /// </returns>
    [Command("event")]
    public async Task<Result> LookupEventAsync(string query) => await LookupProductAsync(Registries.Events.AllRegistrants, query);

    /// <summary>Performs a lookup for a trait based on the provided query and returns matching results.</summary>
    /// <param name="query">The search term to look up the trait by. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no results are found).
    /// </returns>
    [Command("trait")]
    public async Task<Result> LookupTraitAsync(string query) => await LookupProductAsync(Registries.Traits.AllRegistrants, query);

    /// <summary>
    ///     Performs a lookup for an item based on the provided query and returns matching results from the registered
    ///     items.
    /// </summary>
    /// <param name="query">The search term to look up the item by. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no matching results are found).
    /// </returns>
    [Command("item")]
    public async Task<Result> LookupItemAsync(string query) => await LookupProductAsync(Registries.Items.AllRegistrants, query);

    private ValueTask<Result> LookupProductAsync(IReadOnlyList<IProduct> products, string query)
    {
        var index = 0;
        var results = new string[settings.LookupLimit];

        for (var i = 0; i < products.Count; i++)
        {
            IProduct product = products[i];
            string code = product.Name;

            if (string.Compare(code, query, StringComparison.InvariantCultureIgnoreCase) == -1) continue;

            results[index++] = product.ToString();

            if (index >= results.Length) break;
        }


        return new ValueTask<Result>(
            string.IsNullOrEmpty(results[0]) ? Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)) : Result.Ok($"{query}: {string.Join(separator: ", ", results)}")
        );
    }

    /// <summary>Performs a lookup for a mod based on the provided query and returns matching results.</summary>
    /// <param name="query">The identifier or name of the mod to search for. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no matching mod is found).
    /// </returns>
    [Command("mod")]
    public ValueTask<IResult> LookupModAsync(string query)
    {
        IReadOnlyList<Mod.Mod> mods = Registries.Mods.AllRegistrants;

        for (var index = 0; index < mods.Count; index++)
        {
            Mod.Mod mod = mods[index];

            if (!string.Equals(mod.Id, query, StringComparison.OrdinalIgnoreCase)) continue;
            if (!string.Equals(mod.Name, query, StringComparison.InvariantCultureIgnoreCase)) continue;

            return new ValueTask<IResult>(Result.Ok($"{mod.Name} ({mod.Id}, {mod.Version})"));
        }

        return new ValueTask<IResult>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)));
    }

    /// <summary>Performs a lookup for a disease based on the provided query and returns matching results.</summary>
    /// <param name="query">The search term to look up the disease by. The search is case-insensitive.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an IResult, which indicates
    ///     whether the lookup was successful or if it encountered an error (e.g., when no results are found).
    /// </returns>
    [Command("disease")]
    public ValueTask<Result> LookupDiseaseAsync(string query)
    {
        var index = 0;
        var results = new string[settings.LookupLimit];

        List<IncidentDef> defs = DefDatabase<IncidentDef>.AllDefsListForReading;

        for (var i = 0; i < defs.Count; i++)
        {
            IncidentDef def = defs[i];

            if (def.category != IncidentCategoryDefOf.DiseaseHuman) continue;
            if (string.Compare(def.defName, query, StringComparison.OrdinalIgnoreCase) == -1) continue;
            if (string.Compare(def.label, query, StringComparison.InvariantCultureIgnoreCase) == -1) continue;

            results[index++] = def.label;

            if (index >= results.Length) break;
        }

        return string.IsNullOrEmpty(results[0])
            ? new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)))
            : new ValueTask<Result>(Result.Ok($"{query}: {string.Join(separator: ", ", results)}"));
    }
}
