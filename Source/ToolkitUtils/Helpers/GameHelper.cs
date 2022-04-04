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
using CommonLib.Helpers;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class GameHelper
    {
        private static readonly PreceptDef[] MarriagePrecepts =
        {
            PreceptDefOf.SpouseCount_Male_MaxTwo, PreceptDefOf.SpouseCount_Female_MaxTwo,
            PreceptDefOf.SpouseCount_Male_MaxThree, PreceptDefOf.SpouseCount_Female_MaxThree,
            PreceptDefOf.SpouseCount_Male_MaxFour, PreceptDefOf.SpouseCount_Female_MaxFour,
            PreceptDefOf.SpouseCount_Male_Unlimited, PreceptDefOf.SpouseCount_Female_Unlimited
        };
    
        [CanBeNull] public static string SanitizedLabel([NotNull] this Def def) => def.label == null ? null : RichTextHelper.StripTags(def.label);

        [CanBeNull] public static string SanitizedLabel([NotNull] this Thing thing) => thing.def?.label == null ? null : RichTextHelper.StripTags(thing.def.label);

        [NotNull]
        public static string TryGetModName([CanBeNull] this ModContentPack content)
        {
            if (content?.IsCoreMod == true)
            {
                return "RimWorld";
            }

            return content?.Name ?? "Unknown";
        }

        [NotNull] public static string TryGetModName([CanBeNull] this Def def) => def?.modContentPack?.TryGetModName() ?? "Unknown";

        // https://stackoverflow.com/a/42246387
        [NotNull]
        public static IEnumerable<Type> GetAllTypes([NotNull] Type genericType, params Type[] genericParameters)
        {
            if (!genericType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Specified type must be a generic type definition", nameof(genericType));
            }

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => IsGenericType(t, genericType, false, genericParameters));
        }
        
        [NotNull]
        public static IEnumerable<Type> GetAllTypes([NotNull] Type @interface)
        {
            if (!@interface.IsInterface)
            {
                throw new ArgumentException("Specified type must be an interface definition", nameof(@interface));
            }

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsClass && @interface.IsAssignableFrom(t));
        }

        internal static bool IsGenericTypeDeep([NotNull] Type type, Type genericType, bool fuzzy = false, params Type[] genericParams)
        {
            if (IsGenericType(type, genericType, fuzzy, genericParams))
            {
                return true;
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                if (IsGenericTypeDeep(@interface, genericType, fuzzy, genericParams))
                {
                    return true;
                }
            }

            return type.BaseType != null && IsGenericTypeDeep(type.BaseType, genericType, fuzzy, genericParams);
        }

        internal static bool IsGenericType([NotNull] Type type, Type genericType, bool fuzzy = false, params Type[] genericParams)
        {
            if (type.IsGenericType && genericType.IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return true;
            }

            if (type.GetInterfaces().Any(i => i.IsGenericType && genericType.IsAssignableFrom(i.GetGenericTypeDefinition())))
            {
                return true;
            }

            if (fuzzy)
            {
                return false;
            }

            Type[] args = type.GetGenericArguments();

            return args.Length == genericParams.Length && args.Zip(genericParams, (f, s) => s.IsAssignableFrom(f)).All(c => c);
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

        public static bool CanPawnsMarry(Pawn asker, Pawn askee)
        {
            if (asker.GetSpouses(false).Any(p => p.Equals(askee)))
            {
                return false;
            }

            if (askee.GetSpouses(false).Any(p => p.Equals(asker)))
            {
                return false;
            }
            
            return HasOpenSpouseSlot(asker) && HasOpenSpouseSlot(askee);
        }

        private static bool HasOpenSpouseSlot(Pawn pawn)
        {
            foreach (PreceptDef precept in MarriagePrecepts)
            {
                Gender preceptAffects = GetGenderForPrecept(precept);

                if (preceptAffects != pawn.gender && pawn.ideo.Ideo.HasPrecept(precept))
                {
                    continue;
                }

                return pawn.GetSpouseCount(false) < GetLimitForPrecept(precept);
            }

            return pawn.GetSpouseCount(false) <= 0;
        }
        
        private static Gender GetGenderForPrecept([NotNull] PreceptDef precept)
        {
            var comp = precept.comps.Find(c => c is PreceptComp_UnwillingToDo_Gendered) as PreceptComp_UnwillingToDo_Gendered;

            return comp?.gender ?? Gender.None;
        }

        private static int GetLimitForPrecept([NotNull] Def precept)
        {
            switch (precept.defName)
            {
                case nameof(PreceptDefOf.SpouseCount_Male_MaxTwo):
                    return 2;
                case nameof(PreceptDefOf.SpouseCount_Male_MaxThree):
                    return 3;
                case nameof(PreceptDefOf.SpouseCount_Male_MaxFour):
                    return 4;
                case nameof(PreceptDefOf.SpouseCount_Male_Unlimited):
                    return int.MaxValue;
                case nameof(PreceptDefOf.SpouseCount_Female_MaxTwo):
                    return 2;
                case nameof(PreceptDefOf.SpouseCount_Female_MaxThree):
                    return 3;
                case nameof(PreceptDefOf.SpouseCount_Female_MaxFour):
                    return 4;
                case nameof(PreceptDefOf.SpouseCount_Female_Unlimited):
                    return int.MaxValue;
                default:
                    return 1;
            }
        }
    }
}
