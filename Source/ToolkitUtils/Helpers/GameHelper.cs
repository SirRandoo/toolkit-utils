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
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers;

public static class GameHelper
{
    public static string? SanitizedLabel(this Def def) => def.label is null ? null : RichTextHelper.StripTags(def.label);

    public static string? SanitizedLabel(this Thing thing) => thing.def?.label is null ? null : RichTextHelper.StripTags(thing.def.label);

    public static string TryGetModName(this ModContentPack? content)
    {
        if (content is null)
        {
            return "Unknown";
        }

        if (content.IsCoreMod)
        {
            return "RimWorld";
        }

        return content.Name ?? content.PackageId ?? "Unknown";
    }

    public static string TryGetModName(this Def? def) => TryGetModName(def?.modContentPack);

    // https://stackoverflow.com/a/42246387
    public static IEnumerable<Type> GetAllTypes(Type genericType, params Type[] genericParameters)
    {
        if (!genericType.IsGenericTypeDefinition)
        {
            throw new ArgumentException("Specified type must be a generic type definition", nameof(genericType));
        }

        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => IsGenericType(t, genericType, false, genericParameters));
    }

    public static IEnumerable<Type> GetAllTypes(Type @interface)
    {
        if (!@interface.IsInterface)
        {
            throw new ArgumentException("Specified type must be an interface definition", nameof(@interface));
        }

        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsClass && @interface.IsAssignableFrom(t));
    }

    internal static bool IsGenericTypeDeep(Type type, Type genericType, bool fuzzy = false, params Type[] genericParams)
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

        return type.BaseType is not null && IsGenericTypeDeep(type.BaseType, genericType, fuzzy, genericParams);
    }

    internal static bool IsGenericType(Type type, Type genericType, bool fuzzy = false, params Type[] genericParams)
    {
        if (!type.IsGenericType || !genericType.IsAssignableFrom(type.GetGenericTypeDefinition()))
        {
            return false;
        }

        Type[] args = type.GetGenericArguments();

        return fuzzy || (args.Length == genericParams.Length && args.Zip(genericParams, (f, s) => s.IsAssignableFrom(f)).All(c => c));
    }

    public static bool GetDefaultUsability(ThingDef thing)
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

    public static bool GetDefaultMaterialState(ThingDef thing)
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

        for (var index = 0; index < thing.researchPrerequisites.Count; index++)
        {
            ResearchProjectDef project = thing.researchPrerequisites[index];
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

    public static bool HasOpenSpouseSlot(Pawn pawn) => pawn.GetSpouseCount(false) <= 0;
}
