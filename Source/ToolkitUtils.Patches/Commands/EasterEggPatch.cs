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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit.Utilities;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class EasterEggPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(EasterEgg), nameof(EasterEgg.ExecuteSirRandooEasterEgg));
        }

        private static bool Prefix()
        {
            CommandRouter.MainThreadCommands.Enqueue(
                () =>
                {
                    ArgWorker.ItemProxy item = GenerateItem();

                    if (item == null)
                    {
                        return;
                    }

                    SpawnItem(item);
                }
            );

            return false;
        }

        private static void SpawnItem([NotNull] ArgWorker.ItemProxy item)
        {
            Map map = Current.Game.AnyPlayerHomeMap;

            if (map == null)
            {
                return;
            }

            Thing thing = PurchaseHelper.MakeThing(item.Thing.Thing, item.Stuff?.Thing, item.Quality);
            IntVec3 position = DropCellFinder.TradeDropSpot(map);

            if (item.Thing.Thing.Minifiable)
            {
                ThingDef minifiedDef = item.Thing.Thing.minifiedDef;
                var minifiedThing = (MinifiedThing)ThingMaker.MakeThing(minifiedDef);
                minifiedThing.InnerThing = thing;
                minifiedThing.stackCount = 1;
                PurchaseHelper.SpawnItem(position, map, minifiedThing);
            }
            else
            {
                thing.stackCount = 1;
                PurchaseHelper.SpawnItem(position, map, thing);
            }

            Find.LetterStack.ReceiveLetter("SirRandoo is here", @"SirRandoo has sent you a rare item! Enjoy!", LetterDefOf.PositiveEvent, thing);
        }

        [CanBeNull]
        private static ArgWorker.ItemProxy GenerateItem()
        {
            var proxy = new ArgWorker.ItemProxy
            {
                Thing = Data.Items.Where(i => i.Thing != null)
                   .Where(i => i.Thing.race == null)
                   .Where(i => i.Cost > 200 && i.Cost < 2000)
                   .InRandomOrder()
                   .FirstOrDefault()
            };

            if (proxy.Thing == null)
            {
                return null;
            }

            if (!proxy.Thing.Thing.MadeFromStuff)
            {
                return proxy;
            }

            ThingDef stuff = GenStuff.RandomStuffByCommonalityFor(proxy.Thing.Thing);
            proxy.Stuff = Data.Items.Find(i => string.Equals(i.DefName, stuff.defName));

            return proxy;
        }
    }
}
