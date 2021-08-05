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
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class GameHelper
    {
        [CanBeNull]
        public static string SanitizedLabel([NotNull] this Def def)
        {
            return def.label == null ? null : Unrichify.StripTags(def.label);
        }

        [CanBeNull]
        public static string SanitizedLabel([NotNull] this Thing thing)
        {
            return thing.def?.label == null ? null : Unrichify.StripTags(thing.def.label);
        }

        [NotNull]
        public static string TryGetModName([CanBeNull] this ModContentPack content)
        {
            if (content?.IsCoreMod == true)
            {
                return "RimWorld";
            }

            return content?.Name ?? "Unknown";
        }

        [NotNull]
        public static string TryGetModName([CanBeNull] this Def def)
        {
            return def?.modContentPack?.TryGetModName() ?? "Unknown";
        }

        // https://stackoverflow.com/a/42246387
        [NotNull]
        public static IEnumerable<Type> GetAllTypes([NotNull] Type genericType, params Type[] genericParameters)
        {
            if (!genericType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Specified type must be a generic type definition", nameof(genericType));
            }

            return AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a => a.GetTypes())
               .Where(t => IsGenericTypeValid(t, genericType, genericParameters));
        }

        private static bool IsGenericTypeValid([NotNull] Type type, Type genericType, params Type[] genericParameters)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            if (type.GetGenericTypeDefinition() != genericType)
            {
                return false;
            }

            Type[] args = type.GetGenericArguments();
            return args.Length == genericParameters.Length
                   && args.Zip(genericParameters, (f, s) => s.IsAssignableFrom(f)).All(c => c);
        }

        public static bool GetDefaultUsability([NotNull] ThingDef thing)
        {
            if (thing.tradeTags.NullOrEmpty())
            {
                return true;
            }

            foreach (string tag in thing.tradeTags)
            {
                if (tag.Equals("Artifact", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                if (tag.Equals("ExoticMisc", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool GetDefaultMaterialState([NotNull] ThingDef thing)
        {
            if (!thing.IsStuff)
            {
                return false;
            }

            var rarity = 1f;
            float commonality = thing.stuffProps.commonality;

            if (commonality < 1.0f)
            {
                return false;
            }

            if (thing.researchPrerequisites.NullOrEmpty())
            {
                return rarity >= 0.85f;
            }

            foreach (ResearchProjectDef project in thing.researchPrerequisites)
            {
                var tier = (int)project.techLevel;

                if (tier <= 1)
                {
                    continue;
                }

                rarity *= (float)(int)TechLevel.Neolithic / (int)project.techLevel;

                if (project.TechprintCount > 0)
                {
                    rarity *= project.TechprintCount / 25f * project.techprintCommonality;
                }
            }

            return rarity >= 0.85f;
        }
    }
}
