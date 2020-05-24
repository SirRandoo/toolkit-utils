using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class SimpleSidearms
    {
        public static readonly bool Active;

        private static readonly Type CompSidearmMemory;
        private static readonly FieldInfo SidearmMemoryWeapons;
        private static readonly FieldInfo ThingFromPair;
        private static readonly FieldInfo StuffFromPair;

        static SimpleSidearms()
        {
            foreach (var handle in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("PeteTimesSix.SimpleSidearms")
            ))
            {
                try
                {
                    var assembly = handle.GetType().Assembly;

                    CompSidearmMemory = assembly.GetType("SimpleSidearms.rimworld.CompSidearmMemory");
                    var weaponPair = assembly.GetType("SimpleSidearms.rimworld.ThingDefStuffDefPair");

                    SidearmMemoryWeapons = CompSidearmMemory.GetField("rememberedWeapons");
                    ThingFromPair = weaponPair.GetField("thing");
                    StuffFromPair = weaponPair.GetField("stuff");

                    Active = true;
                }
                catch (Exception e)
                {
                    TkLogger.Error("Compatibility class for Simple Sidearms failed!", e);
                }
            }
        }

        public static IEnumerable<Thing> GetSidearms(Pawn pawn)
        {
            if (!Active)
            {
                return null;
            }

            var comp = pawn.AllComps.FirstOrDefault(c => c.GetType() == CompSidearmMemory);


            if (!(SidearmMemoryWeapons.GetValue(comp) is IList value))
            {
                return null;
            }

            try
            {
                var container = new List<Thing>();

                foreach (var obj in value)
                {
                    var thingValue = ThingFromPair.GetValue(obj);
                    var stuffValue = StuffFromPair.GetValue(obj);

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
