﻿// ToolkitUtils
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp;

public static class SimpleSidearms
{
    public static bool Active;
    private static bool _pendingInit = true;

    private static Type? _compSidearmMemory;
    private static FieldInfo? _sidearmMemoryWeapons;
    private static FieldInfo? _thingFromPair;
    private static FieldInfo? _stuffFromPair;

    private static void DeferredInitialization(ThingWithComps catalyst)
    {
        _pendingInit = false;

        ThingComp comp = catalyst.AllComps.FirstOrDefault(c => c.GetType().Name.Equals("CompSidearmMemory"));

        if (comp == null)
        {
            return;
        }

        try
        {
            Assembly assembly = comp.GetType().Assembly;

            _compSidearmMemory = comp.GetType();
            Type weaponPair = assembly.GetType("SimpleSidearms.rimworld.ThingDefStuffDefPair");

            _sidearmMemoryWeapons = _compSidearmMemory.GetField("rememberedWeapons");
            _thingFromPair = weaponPair.GetField("thing");
            _stuffFromPair = weaponPair.GetField("stuff");

            Active = true;
        }
        catch (Exception e)
        {
            TkUtils.Logger.Error("Compatibility class for Simple Sidearms failed!", e);
        }
    }

    public static IEnumerable<Thing> GetSidearms(Pawn pawn)
    {
        if (_pendingInit)
        {
            DeferredInitialization(pawn);
        }

        if (!Active || !TryGetSidearmMemory(pawn, out IList? weapons))
        {
            return Array.Empty<Thing>();
        }

        try
        {
            var container = new List<Thing>();

            foreach (object obj in weapons)
            {
                if (!TryGetWeapon(obj, out Thing weapon))
                {
                    continue;
                }

                container.Add(weapon);
            }

            return container;
        }
        catch (Exception)
        {
            return Array.Empty<Thing>();
        }
    }

    private static bool TryGetSidearmComp(ThingWithComps pawn, [NotNullWhen(true)] out ThingComp? comp)
    {
        comp = pawn.AllComps.FirstOrFallback(c => c?.GetType() == _compSidearmMemory);

        return comp != null;
    }

    private static bool TryGetSidearmMemory(ThingWithComps pawn, [NotNullWhen(true)] out IList? weapons)
    {
        if (!TryGetSidearmComp(pawn, out ThingComp? comp))
        {
            weapons = null;

            return false;
        }

        if (_sidearmMemoryWeapons?.GetValue(comp) is not IList value)
        {
            weapons = null;

            return false;
        }

        weapons = value;

        return true;
    }

    private static bool TryGetWeapon(object weaponPair, [NotNullWhen(true)] out Thing? weapon)
    {
        object? thingValue = _thingFromPair?.GetValue(weaponPair);
        object? stuffValue = _stuffFromPair?.GetValue(weaponPair);

        if (thingValue is not ThingDef thing)
        {
            weapon = null;

            return false;
        }

        if (stuffValue is not ThingDef stuff)
        {
            weapon = ThingMaker.MakeThing(thing);

            return true;
        }

        weapon = ThingMaker.MakeThing(thing, stuff);

        return weapon is not null;
    }
}
