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
    public static class MutatorRegistry
    {
        private static readonly List<IMutator> Registry = new List<IMutator>();
        private static readonly Dictionary<string, IMutator> RegistryKeyed = new Dictionary<string, IMutator>();

        static MutatorRegistry()
        {
            foreach (Type type in typeof(IMutator).AllSubclassesNonAbstract())
            {
                if (!(Activator.CreateInstance(type) is IMutator selector) || RegistryKeyed.ContainsKey(selector.Id))
                {
                    continue;
                }

                RegistryKeyed.TryAdd(selector.Id, selector);
                Registry.Add(selector);
            }
        }

        /// <summary>
        ///     Returns an enumerable of all the mutators registered within the
        ///     registry.
        /// </summary>
        public static IEnumerable<IMutator> AllMutators => Registry;

        /// <summary>
        ///     Attempts to get a mutator with the given id.
        /// </summary>
        /// <param name="id">The id of the mutator being queried for.</param>
        /// <param name="mutator">
        ///     A mutator instance if a mutator with the given id was found, or
        ///     <see langword="null"/> if it wasn't found.
        /// </param>
        /// <returns>Whether a selector with the given was was found.</returns>
        [ContractAnnotation("=> true, mutator: notnull; => false, mutator: null")]
        public static bool TryGetMutators([NotNull] string id, out IMutator mutator) => RegistryKeyed.TryGetValue(id, out mutator);
    }
}
