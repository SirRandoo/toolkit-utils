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
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    internal static class DomainIndexer
    {
        internal static readonly MutatorEntry[] Mutators;
        internal static readonly SelectorEntry[] Selectors;

        private static readonly string[] FilteredNamespaceRoots =
        {
            "System", "Unity", "Steamworks", "Verse", "RimWorld", "Utf8Json", "Mono", "RestSharp", "SimpleJSON", "MoonSharp", "TwitchLib", "Newtonsoft", "HugsLib", "HarmonyLib", "MS", "NAudio",
            "TMPro"
        };

        static DomainIndexer()
        {
            var mutatorEntries = new List<MutatorEntry>();
            var selectorEntries = new List<SelectorEntry>();

            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GlobalAssemblyCache).SelectMany(a => a.GetTypes()).Where(t => !t.IsInterface && !t.IsAbstract))
            {
                if (FilteredNamespaceRoots.Any(r => type.Namespace?.StartsWith(r) == true))
                {
                    continue;
                }

                bool isGeneric = type.IsGenericType || type.GetInterfaces().Any(i => i.IsGenericType);


                if (typeof(ICompatibilityProvider).IsAssignableFrom(type))
                {
                    CompatRegistry.ProcessType(type);
                }
                else
                {
                    switch (isGeneric)
                    {
                        case true when GameHelper.IsGenericTypeDeep(type, typeof(ISelectorBase<>)):
                            selectorEntries.Add(ProcessSelector(type));

                            break;
                        case true when GameHelper.IsGenericTypeDeep(type, typeof(IMutatorBase<>)):
                            mutatorEntries.Add(ProcessMutator(type));

                            break;
                    }
                }
            }

            Mutators = mutatorEntries.ToArray();
            Selectors = selectorEntries.ToArray();
        }

        [NotNull]
        private static SelectorEntry ProcessSelector([NotNull] Type selector)
        {
            Type selectorBase = typeof(ISelectorBase<>);
            var entry = new SelectorEntry { Type = selector };

            if (GameHelper.IsGenericType(selector, selectorBase, false, typeof(ThingItem)))
            {
                entry.Target = EditorTarget.Item;
            }
            else if (GameHelper.IsGenericType(selector, selectorBase, false, typeof(TraitItem)))
            {
                entry.Target = EditorTarget.Trait;
            }
            else if (GameHelper.IsGenericType(selector, selectorBase, false, typeof(PawnKindItem)))
            {
                entry.Target = EditorTarget.Pawn;
            }
            else if (GameHelper.IsGenericType(selector, selectorBase, false, typeof(EventItem)))
            {
                entry.Target = EditorTarget.Event;
            }
            else
            {
                entry.Target = EditorTarget.Any;
            }

            return entry;
        }

        [NotNull]
        private static MutatorEntry ProcessMutator([NotNull] Type mutator)
        {
            Type mutatorBase = typeof(IMutatorBase<>);
            var entry = new MutatorEntry { Type = mutator };

            if (GameHelper.IsGenericType(mutator, mutatorBase, false, typeof(ThingItem)))
            {
                entry.Target = EditorTarget.Item;
            }
            else if (GameHelper.IsGenericType(mutator, mutatorBase, false, typeof(TraitItem)))
            {
                entry.Target = EditorTarget.Trait;
            }
            else if (GameHelper.IsGenericType(mutator, mutatorBase, false, typeof(PawnKindItem)))
            {
                entry.Target = EditorTarget.Pawn;
            }
            else if (GameHelper.IsGenericType(mutator, mutatorBase, false, typeof(EventItem)))
            {
                entry.Target = EditorTarget.Event;
            }
            else
            {
                entry.Target = EditorTarget.Any;
            }

            return entry;
        }

        internal enum EditorTarget { Any, Item, Trait, Pawn, Event }

        internal class SelectorEntry
        {
            public Type Type { get; set; }
            public EditorTarget Target { get; set; }
        }

        internal class MutatorEntry
        {
            public Type Type { get; set; }
            public EditorTarget Target { get; set; }
        }
    }
}
