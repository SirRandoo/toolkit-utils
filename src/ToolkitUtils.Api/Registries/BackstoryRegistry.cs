// MIT License
// 
// Copyright (c) 2023 SirRandoo
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

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Data.Models;
using Verse;

namespace ToolkitUtils.Api
{
    [StaticConstructorOnStartup]
    public static class BackstoryRegistry
    {
        private static readonly List<DefWrapper<BackstoryDef>> Registry = new List<DefWrapper<BackstoryDef>>();
        private static readonly Dictionary<string, DefWrapper<BackstoryDef>> RegistryKeyed = new Dictionary<string, DefWrapper<BackstoryDef>>();

        static BackstoryRegistry()
        {
            foreach (BackstoryDef def in DefDatabase<BackstoryDef>.AllDefs)
            {
                if (RegistryKeyed.ContainsKey(def.defName))
                {
                    continue;
                }

                var wrapper = new DefWrapper<BackstoryDef>(def);

                Registry.Add(wrapper);
                RegistryKeyed.Add(def.defName, wrapper);
            }
        }

        /// <summary>
        ///     Returns an enumerable of the backstories registered within the
        ///     registry.
        /// </summary>
        [NotNull]
        public static IReadOnlyList<DefWrapper<BackstoryDef>> AllRegistrants => Registry;

        /// <summary>
        ///     Returns a backstory with the given def name.
        /// </summary>
        /// <param name="defName">The def name of the backstory being requested.</param>
        [CanBeNull]
        public static DefWrapper<BackstoryDef> Get([NotNull] string defName) =>
            RegistryKeyed.TryGetValue(defName, out DefWrapper<BackstoryDef> wrapper) ? wrapper : default;
    }
}
