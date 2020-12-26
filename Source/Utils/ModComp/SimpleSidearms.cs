using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    public static class SimpleSidearms
    {
        public static bool Active;
        private static bool _pendingInit = true;

        private static Type _compSidearmMemory;
        private static FieldInfo _sidearmMemoryWeapons;
        private static FieldInfo _thingFromPair;
        private static FieldInfo _stuffFromPair;

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
                LogHelper.Error("Compatibility class for Simple Sidearms failed!", e);
            }
        }

        public static IEnumerable<Thing> GetSidearms(Pawn pawn)
        {
            if (_pendingInit)
            {
                DeferredInitialization(pawn);
            }

            if (!Active || !TryGetSidearmMemory(pawn, out IList weapons))
            {
                return null;
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
                return null;
            }
        }

        private static bool TryGetSidearmComp(ThingWithComps pawn, out ThingComp comp)
        {
            comp = pawn.AllComps.FirstOrFallback(c => c.GetType() == _compSidearmMemory);

            return comp != null;
        }

        private static bool TryGetSidearmMemory(ThingWithComps pawn, out IList weapons)
        {
            if (!TryGetSidearmComp(pawn, out ThingComp comp))
            {
                weapons = null;
                return false;
            }

            if (!(_sidearmMemoryWeapons.GetValue(comp) is IList value))
            {
                weapons = null;
                return false;
            }

            weapons = value;
            return true;
        }

        private static bool TryGetWeapon(object weaponPair, out Thing weapon)
        {
            object thingValue = _thingFromPair.GetValue(weaponPair);
            object stuffValue = _stuffFromPair.GetValue(weaponPair);

            if (!(thingValue is ThingDef thing))
            {
                weapon = null;
                return false;
            }

            if (!(stuffValue is ThingDef stuff))
            {
                weapon = ThingMaker.MakeThing(thing);
                return true;
            }

            weapon = ThingMaker.MakeThing(thing, stuff);
            return weapon != null;
        }
    }
}
