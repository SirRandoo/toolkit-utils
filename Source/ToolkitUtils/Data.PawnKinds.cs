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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public static partial class Data
    {
        /// <summary>
        ///     The pawns available for purchase within the mod's store.
        /// </summary>
        public static List<PawnKindItem> PawnKinds { get; private set; }

        /// <summary>
        ///     Loads the traits saved to the given file.
        /// </summary>
        /// <param name="path">The file to load pawn kinds from</param>
        /// <param name="ignoreErrors">Whether loading errors should be ignored</param>
        public static void LoadPawnKinds(string path, bool ignoreErrors)
        {
            PawnKinds = LoadJson<List<PawnKindItem>>(path, ignoreErrors) ?? new List<PawnKindItem>();
        }

        /// <summary>
        ///     Loads the traits saved to the given file.
        /// </summary>
        /// <param name="path">The file to load pawn kinds from</param>
        /// <param name="ignoreErrors">Whether loading errors should be ignored</param>
        public static async Task LoadPawnKindsAsync(string path, bool ignoreErrors)
        {
            PawnKinds = await LoadJsonAsync<List<PawnKindItem>>(path, ignoreErrors) ?? new List<PawnKindItem>();
        }

        /// <summary>
        ///     Saves a list of pawns at the given file path.
        /// </summary>
        /// <param name="path">The file to save pawns to</param>
        public static void SavePawnKinds(string path)
        {
            SaveJson(PawnKinds, path);
        }

        private static void ValidatePawnKinds()
        {
            List<PawnKindDef> kindDefs = DefDatabase<PawnKindDef>.AllDefs.Where(k => k.RaceProps.Humanlike).ToList();
            PawnKinds.RemoveAll(k => kindDefs.Find(d => d.race.defName.Equals(k.DefName)) == null);

            foreach (PawnKindDef def in kindDefs)
            {
                List<PawnKindItem> item = PawnKinds.FindAll(k => k.DefName.Equals(def.race.defName));

                if (item.Count > 0)
                {
                    continue;
                }

                PawnKinds.Add(
                    new PawnKindItem { DefName = def.race.defName, Enabled = true, Name = def.race.label ?? def.race.defName, Cost = def.race.CalculateStorePrice() }
                );
            }
        }

        private static void ValidatePawnKindData()
        {
            var builder = new StringBuilder();

            foreach (PawnKindItem pawn in PawnKinds)
            {
                pawn.PawnData ??= new PawnKindData();

                try
                {
                    pawn.PawnData.Mod = pawn.ColonistKindDef.TryGetModName();
                    pawn.LoadGameData();
                    pawn.UpdateStats();
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {pawn.Name ?? pawn.DefName}");
                }
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following pawn kinds could not be processed:\n");
            TkUtils.Logger.Warn(builder.ToString());
        }

        /// <summary>
        ///     Loads pawns from the given partial data.
        /// </summary>
        /// <param name="partialData">A collection of partial data to load</param>
        public static void LoadPawnPartial([NotNull] IEnumerable<PawnKindItem> partialData)
        {
            var builder = new StringBuilder();

            foreach (PawnKindItem partial in partialData)
            {
                PawnKindItem existing = PawnKinds.Find(i => i.DefName.Equals(partial.DefName));

                if (existing == null)
                {
                    if (partial.ColonistKindDef == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");

                        continue;
                    }

                    PawnKinds.Add(partial);

                    continue;
                }

                existing.Name = partial.Name;
                existing.Cost = partial.Cost;
                existing.Enabled = partial.Enabled;
                existing.Data = partial.Data;
            }
        }

        /// <summary>
        ///     Saves a list of pawns at the given file path.
        /// </summary>
        /// <param name="path">The file to save pawns to</param>
        public static async Task SavePawnKindsAsync(string path)
        {
            await SaveJsonAsync(PawnKinds, path);
        }

        /// <summary>
        ///     Gets a pawn kind from the given input.
        /// </summary>
        /// <param name="input">The raw input to check against</param>
        /// <param name="kind">
        ///     The <see cref="PawnKindItem"/> found with the given
        ///     input, or <c>null</c> if no pawn kind could be found
        /// </param>
        /// <returns>Whether a pawn kind was found from the given input</returns>
        /// <remarks>
        ///     Prefixing <see cref="input"/> with a dollar sign ($) will cause
        ///     the method to search against the
        ///     <see cref="PawnKindItem.DefName"/>s
        ///     of the traits, instead of the trait
        ///     <see cref="PawnKindItem.Name"/>s.
        /// </remarks>
        [ContractAnnotation("input:notnull => true,kind:notnull; input:notnull => false,kind:null")]
        public static bool TryGetPawnKind(string input, out PawnKindItem kind)
        {
            if (input.StartsWith("$"))
            {
                input = input.Substring(1);

                kind = PawnKinds.Find(t => string.Equals(t.DefName, input));
            }
            else
            {
                kind = PawnKinds.Find(t => string.Equals(t.Name.ToToolkit(), input.ToToolkit(), StringComparison.InvariantCultureIgnoreCase));
            }

            return kind != null;
        }
    }
}
