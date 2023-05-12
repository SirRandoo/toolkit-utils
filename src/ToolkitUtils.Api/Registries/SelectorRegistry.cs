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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ToolkitUtils.Data.Models;
using Verse;

namespace ToolkitUtils.Api
{
    [StaticConstructorOnStartup]
    public static class SelectorRegistry
    {
        private static readonly List<ISelector> Registry = new List<ISelector>();
        private static readonly Dictionary<string, ISelector> RegistryKeyed = new Dictionary<string, ISelector>();

        static SelectorRegistry()
        {
            foreach (Type type in typeof(ISelector).AllSubclassesNonAbstract())
            {
                if (!(Activator.CreateInstance(type) is ISelector selector) || RegistryKeyed.ContainsKey(selector.Id))
                {
                    continue;
                }

                RegistryKeyed.TryAdd(selector.Id, selector);
                Registry.Add(selector);
            }
        }

        /// <summary>
        ///     Returns an enumerable of all the selectors registered within the
        ///     registry.
        /// </summary>
        public static IEnumerable<ISelector> AllSelectors => Registry;

        /// <summary>
        ///     Attempts to get a selector with the given id.
        /// </summary>
        /// <param name="id">The id of the selector being queried for.</param>
        /// <param name="selector">
        ///     A selector instance if a selector with the given id was found, or
        ///     <see langword="null"/> if it wasn't found.
        /// </param>
        /// <returns>Whether a selector with the given was was found.</returns>
        [ContractAnnotation("=> true, selector: notnull; => false, selector: null")]
        public static bool TryGetSelector([NotNull] string id, out ISelector selector) => RegistryKeyed.TryGetValue(id, out selector);
    }
}
