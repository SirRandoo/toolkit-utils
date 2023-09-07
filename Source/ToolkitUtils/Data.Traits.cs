// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils;

public static partial class Data
{
    /// <summary>
    ///     The traits available for purchase within the mod's store.
    /// </summary>
    public static List<TraitItem> Traits { get; private set; }

    /// <summary>
    ///     Loads the traits saved to the given file.
    /// </summary>
    /// <param name="path">The file to load traits from</param>
    /// <param name="ignoreErrors">Whether loading errors should be ignored</param>
    public static void LoadTraits(string path, bool ignoreErrors)
    {
        Traits = LoadJson<List<TraitItem>>(path, ignoreErrors) ?? new List<TraitItem>();
    }

    /// <summary>
    ///     Loads the traits saved to the given file.
    /// </summary>
    /// <param name="path">The file to load traits from</param>
    /// <param name="ignoreErrors">Whether loading errors should be ignored</param>
    public static async Task LoadTraitsAsync(string path, bool ignoreErrors)
    {
        Traits = await LoadJsonAsync<List<TraitItem>>(path, ignoreErrors) ?? new List<TraitItem>();
    }

    /// <summary>
    ///     Saves the current list of traits saved to the given file.
    /// </summary>
    /// <param name="path">The file to save the traits to</param>
    public static void SaveTraits(string path)
    {
        SaveJson(Traits, path);
    }

    /// <summary>
    ///     Saves the current list of traits saved to the given file.
    /// </summary>
    /// <param name="path">The file to save the traits to</param>
    public static async Task SaveTraitsAsync(string path)
    {
        await SaveJsonAsync(Traits, path);
    }

    private static void ValidateTraits()
    {
        List<TraitDef> traitDefs = DefDatabase<TraitDef>.AllDefsListForReading;
        Traits.RemoveAll(t => traitDefs.Find(d => d.defName.Equals(t.DefName)) == null);

        foreach (TraitDef def in traitDefs)
        {
            List<TraitItem> existing = Traits.FindAll(t => t.DefName.Equals(def.defName));

            if (existing.NullOrEmpty())
            {
                Traits.AddRange(def.ToTraitItems());

                continue;
            }

            TraitItem[] traitItems = def.ToTraitItems().Where(t => !existing.Any(e => e.Degree == t.Degree)).ToArray();

            if (traitItems.Length > 0)
            {
                Traits.AddRange(traitItems);
            }
        }
    }

    private static void ValidateTraitData()
    {
        var builder = new StringBuilder();

        foreach (TraitItem trait in Traits)
        {
            trait.TraitData ??= new TraitData();

            try
            {
                trait.TraitData.Mod = trait.TraitDef.TryGetModName();
            }
            catch (Exception)
            {
                builder.AppendLine($" - {trait.Name ?? trait.DefName}");
            }
        }

        if (builder.Length <= 0)
        {
            return;
        }

        builder.Insert(0, "The following traits could not be processed:\n");
        TkUtils.Logger.Warn(builder.ToString());
    }

    /// <summary>
    ///     Gets a trait from the given input.
    /// </summary>
    /// <param name="input">The raw input to check against</param>
    /// <param name="trait">
    ///     The <see cref="TraitItem"/> found with the given
    ///     input, or <c>null</c> if no trait could be found
    /// </param>
    /// <returns>Whether a trait was found from the given input</returns>
    /// <remarks>
    ///     Prefixing <see cref="input"/> with a dollar sign ($) will cause
    ///     the method to search against the <see cref="TraitItem.DefName"/>s
    ///     of the traits, instead of the trait
    ///     <see cref="TraitItem.Name"/>s.
    /// </remarks>
    [ContractAnnotation("input:notnull => true,trait:notnull; input:notnull => false,trait:null")]
    public static bool TryGetTrait(string input, out TraitItem trait)
    {
        if (input.StartsWith("$"))
        {
            input = input.Substring(1);

            trait = Traits.Find(t => string.Equals(t.DefName, input));
        }
        else
        {
            trait = Traits.Find(t => string.Equals(t.Name.ToToolkit().StripTags(), input.ToToolkit().StripTags(), StringComparison.InvariantCultureIgnoreCase));
        }

        return trait != null;
    }

    /// <summary>
    ///     Processes the given trait partial data, and loads it into the
    ///     mod's store.
    /// </summary>
    /// <param name="partialData">
    ///     A collection of partial data to load into
    ///     the mod's store
    /// </param>
    public static void ProcessTraitPartial(IEnumerable<TraitItem> partialData)
    {
        var builder = new StringBuilder();

        foreach (TraitItem partial in partialData)
        {
            TraitItem existing = Traits.Find(i => i.DefName.Equals(partial.DefName) && i.Degree == partial.Degree);

            if (existing == null)
            {
                if (partial.TraitDef == null)
                {
                    builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}:{partial.Degree}\n");

                    continue;
                }

                Traits.Add(partial);

                continue;
            }

            existing.Name = partial.Name;
            existing.CanAdd = partial.CanAdd;
            existing.CostToAdd = partial.CostToAdd;
            existing.CanRemove = partial.CanRemove;
            existing.CostToRemove = partial.CostToRemove;
            existing.Data = partial.Data;
        }

        if (builder.Length <= 0)
        {
            return;
        }

        builder.Insert(0, "The following traits could not be loaded from the partial data provided:\n");
        TkUtils.Logger.Warn(builder.ToString());
    }
}