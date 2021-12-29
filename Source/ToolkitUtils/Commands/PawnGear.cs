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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnGear : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize().WithHeader("TabGear".Localize()));

                return;
            }

            twitchMessage.Reply(GetPawnGear(pawn).WithHeader("TabGear".Localize()));
        }

        private static float CalculateArmorRating([NotNull] Pawn pawn, StatDef stat)
        {
            var rating = 0f;
            float value = Mathf.Clamp01(pawn.GetStatValue(stat) / 2f);
            List<BodyPartRecord> parts = pawn.RaceProps.body.AllParts;
            List<Apparel> apparel = pawn.apparel?.WornApparel;

            foreach (BodyPartRecord part in parts)
            {
                float cache = 1f - value;

                if (apparel != null && apparel.Any())
                {
                    cache = apparel.Where(a => a.def.apparel.CoversBodyPart(part)).Select(a => Mathf.Clamp01(a.GetStatValue(stat) / 2f)).Aggregate(cache, (current, v) => current * (1f - v));
                }

                rating += part.coverageAbs * (1f - cache);
            }

            return Mathf.Clamp(rating * 2f, 0f, 2f);
        }

        private static string GetPawnGear(Pawn pawn)
        {
            var parts = new List<string>();

            if (TkSettings.TempInGear)
            {
                GetTemperatureValues(pawn, parts);
            }

            if (TkSettings.ShowArmor)
            {
                GetArmorValues(pawn, parts);
            }

            if (TkSettings.ShowWeapon)
            {
                GetWeaponData(pawn, parts);
            }

            if (!TkSettings.ShowApparel)
            {
                return parts.GroupedJoin();
            }

            Pawn_ApparelTracker a = pawn.apparel;

            if (a == null || a.WornApparelCount <= 0)
            {
                return parts.GroupedJoin();
            }

            List<Apparel> apparel = a.WornApparel;
            parts.Add($"{"Apparel".Localize()}: {apparel.Select(item => Unrichify.StripTags(item.LabelCap)).SectionJoin()}");

            return !parts.Any() ? "None".Localize() : parts.GroupedJoin();
        }

        private static void GetWeaponData([NotNull] Pawn pawn, ICollection<string> parts)
        {
            List<Thing> sidearms = SimpleSidearms.GetSidearms(pawn)?.ToList();
            var weapons = new List<string>();
            List<ThingWithComps> equipment = pawn.equipment?.AllEquipmentListForReading ?? new List<ThingWithComps>();
            int equipmentCount = equipment.Count;
            List<Thing> inventory = pawn.inventory.innerContainer.InnerListForReading ?? new List<Thing>();
            var usedInventory = new List<Thing>();

            if (sidearms?.Count > 0)
            {
                GetSidearmData(sidearms, equipmentCount, equipment, weapons, inventory, usedInventory);
            }
            else
            {
                Pawn_EquipmentTracker e = pawn.equipment;

                if (e?.AllEquipmentListForReading?.Count > 0)
                {
                    IEnumerable<string> equip = e.AllEquipmentListForReading.Select(eq => Unrichify.StripTags(eq.LabelCap));

                    weapons.AddRange(equip);
                }
            }

            if (weapons.Count <= 0)
            {
                return;
            }

            string section = "Stat_Weapon_Name".Localize();

            parts.Add($"{(weapons.Count > 1 ? section.Pluralize() : section)}: {weapons.SectionJoin()}");
        }

        private static void GetTemperatureValues(Thing pawn, [NotNull] ICollection<string> parts)
        {
            string tempMin = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
            string tempMax = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

            parts.Add($"{ResponseHelper.TemperatureGlyph.AltText($"{"ComfyTemperatureRange".Localize()} ")}{tempMin}~{tempMax}");
        }

        private static void GetSidearmData(
            [NotNull] ICollection<Thing> sidearms,
            int equipmentCount,
            IReadOnlyCollection<ThingWithComps> equipment,
            IList<string> weapons,
            IReadOnlyCollection<Thing> inventory,
            ICollection<Thing> usedInventory
        )
        {
            var loops = 0;
            var equipmentUsed = false;

            while (sidearms.Count > 0 && loops <= 50)
            {
                Thing sidearm = sidearms.Take(1).FirstOrDefault();

                if (sidearm == null)
                {
                    continue;
                }

                if (equipmentCount > 0 && !equipmentUsed && GetEquipmentFromSidearmData(equipment, weapons, sidearm))
                {
                    sidearms.Remove(sidearm);
                    equipmentUsed = true;

                    continue;
                }

                GetSidearms(sidearms, weapons, inventory, usedInventory, sidearm);
                loops++;
            }
        }

        private static void GetSidearms(ICollection<Thing> sidearms, ICollection<string> weapons, [NotNull] IEnumerable<Thing> inventory, ICollection<Thing> usedInventory, Thing sidearm)
        {
            foreach (Thing thing in inventory.Where(thing => sidearm.def.defName.Equals(thing.def.defName)))
            {
                if (usedInventory.Contains(thing))
                {
                    continue;
                }

                weapons.Add(Unrichify.StripTags(thing.LabelCap));
                usedInventory.Add(thing);
                sidearms.Remove(sidearm);

                break;
            }
        }

        private static bool GetEquipmentFromSidearmData([NotNull] IEnumerable<ThingWithComps> equipment, IList<string> weapons, Thing sidearm)
        {
            foreach (ThingWithComps equip in equipment.Where(equip => sidearm.def.defName.Equals(equip.def.defName)))
            {
                weapons.Insert(0, Unrichify.StripTags(equip.LabelCap));

                return true;
            }

            return false;
        }

        private static void GetArmorValues([NotNull] Pawn pawn, ICollection<string> parts)
        {
            float sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
            float blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Blunt);
            float heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Heat);
            var stats = new List<string>();

            if (sharp > 0)
            {
                stats.Add($"{ResponseHelper.DaggerGlyph.AltText($"{"ArmorSharp".Localize()} ")}{sharp.ToStringPercent()}");
            }

            if (blunt > 0)
            {
                stats.Add($"{ResponseHelper.PanGlyph.AltText($"{"ArmorBlunt".Localize()} ")}{blunt.ToStringPercent()}");
            }

            if (heat > 0)
            {
                stats.Add($"{ResponseHelper.FireGlyph.AltText($"{"ArmorHeat".Localize()} ")}{heat.ToStringPercent()}");
            }

            if (stats.Any())
            {
                parts.Add($"{"OverallArmor".Localize()}: {stats.SectionJoin()}");
            }
        }
    }
}
