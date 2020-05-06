using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class SimpleSidearms
    {
        public static readonly bool Active;

        static SimpleSidearms()
        {
            Active = ModsConfig.ActiveModsInLoadOrder
                .Any(m => m.PackageId.EqualsIgnoreCase("PeteTimesSix.SimpleSidearms"));
        }

        public static IEnumerable<Thing> GetSidearms(Pawn pawn)
        {
            if (!Active)
            {
                return null;
            }

            var comp = pawn.AllComps.FirstOrDefault(
                c => c.GetType().FullName?.Equals("SimpleSidearms.rimworld.CompSidearmMemory") ?? false
            );

            if (!(comp?.GetType().GetField("rememberedWeapons").GetValue(comp) is IList value))
            {
                return null;
            }

            try
            {
                var container = new List<Thing>();

                foreach (var obj in value)
                {
                    var thingField = obj.GetType().GetField("thing");
                    var stuffField = obj.GetType().GetField("stuff");

                    var thingValue = thingField.GetValue(obj);
                    var stuffValue = stuffField.GetValue(obj);

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
