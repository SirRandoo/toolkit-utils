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
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit.Utilities;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class EasterEggPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(EasterEgg), "ExecuteSirRandooEasterEgg");
        }

        public static bool Prefix()
        {
            ThingItem item = Data.Items.Where(i => i.Thing != null)
               .Where(i => i.Thing.race == null)
               .Where(i => i.Cost > 200 && i.Cost < 2000)
               .InRandomOrder()
               .FirstOrDefault();

            if (item?.Item == null)
            {
                return false;
            }

            Find.LetterStack.ReceiveLetter(
                "SirRandoo is here",
                "SirRandoo has sent you a rare item! Enjoy!",
                LetterDefOf.PositiveEvent
            );
            item.Item.PutItemInCargoPod(@"<color=""grey"">SirRandoo</color>: <i>Here have this!</i>", 1, "SirRandoo");
            return false;
        }
    }
}
