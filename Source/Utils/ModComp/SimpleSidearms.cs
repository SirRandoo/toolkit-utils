using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            ThingComp comp = catalyst.AllComps
                .FirstOrDefault(c => c.GetType().Name.Equals("CompSidearmMemory"));

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
                TkLogger.Error("Compatibility class for Simple Sidearms failed!", e);
            }
        }

        public static IEnumerable<Thing> GetSidearms(Pawn pawn)
        {
            if (_pendingInit)
            {
                DeferredInitialization(pawn);
            }

            if (!Active)
            {
                return null;
            }

            ThingComp comp = pawn.AllComps.FirstOrDefault(c => c.GetType() == _compSidearmMemory);

            if (!(_sidearmMemoryWeapons.GetValue(comp) is IList value))
            {
                return null;
            }

            try
            {
                var container = new List<Thing>();

                foreach (object obj in value)
                {
                    object thingValue = _thingFromPair.GetValue(obj);
                    object stuffValue = _stuffFromPair.GetValue(obj);

                    if (!(thingValue is ThingDef thing))
                    {
                        continue;
                    }

                    if (!(stuffValue is ThingDef stuff))
                    {
                        container.Add(ThingMaker.MakeThing(thing));
                        continue;
                    }

                    var instance = ThingMaker.MakeThing(thing, stuff) as ThingWithComps;

                    if (instance == null)
                    {
                        continue;
                    }

                    container.Add(instance);
                }

                return container;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
