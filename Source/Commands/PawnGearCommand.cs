using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnGearCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabGear".Translate()));
                return;
            }

            twitchMessage.Reply(GetPawnGear(pawn).WithHeader("TabGear".Translate()));
        }

        private static float CalculateArmorRating(Pawn pawn, StatDef stat)
        {
            var rating = 0f;
            var value = Mathf.Clamp01(pawn.GetStatValue(stat) / 2f);
            var parts = pawn.RaceProps.body.AllParts;
            var apparel = pawn.apparel?.WornApparel;

            foreach (var part in parts)
            {
                var cache = 1f - value;

                if (apparel != null && apparel.Any())
                {
                    cache = apparel.Where(a => a.def.apparel.CoversBodyPart(part))
                        .Select(a => Mathf.Clamp01(a.GetStatValue(stat) / 2f))
                        .Aggregate(cache, (current, v) => current * (1f - v));
                }

                rating += part.coverageAbs * (1f - cache);
            }

            return Mathf.Clamp(rating * 2f, 0f, 2f);
        }

        private static TaggedString GetPawnGear(Pawn pawn)
        {
            var parts = new List<string>();

            if (TkSettings.TempInGear)
            {
                var tempMin = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
                var tempMax = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

                parts.Add($"{"🌡".AltText("ComfyTemperatureRange".Translate().RawText)}{tempMin}~{tempMax}");
            }

            if (TkSettings.ShowArmor)
            {
                var sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Blunt);
                var heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Heat);
                var stats = new List<string>();

                if (sharp > 0)
                {
                    stats.Add($"{"🗡".AltText("ArmorSharp".Translate().RawText)}{sharp.ToStringPercent()}");
                }

                if (blunt > 0)
                {
                    stats.Add($"{"🍳".AltText("ArmorBlunt".Translate().RawText)}{blunt.ToStringPercent()}");
                }

                if (heat > 0)
                {
                    stats.Add($"{"🔥".AltText("ArmorHeat".Translate().RawText)}{heat.ToStringPercent()}");
                }

                if (stats.Any())
                {
                    parts.Add($"{"OverallArmor".Translate().RawText}: {string.Join(", ", stats.ToArray())}");
                }
            }

            if (TkSettings.ShowWeapon)
            {
                var sidearms = SimpleSidearms.GetSidearms(pawn)?.ToList();
                var weapons = new List<string>();
                var equipment = pawn.equipment?.AllEquipmentListForReading ?? new List<ThingWithComps>();
                var equipmentCount = equipment.Count;
                var inventory = pawn.inventory.innerContainer.InnerListForReading ?? new List<Thing>();
                var usedInventory = new List<Thing>();

                if (sidearms?.Any() ?? false)
                {
                    var loops = 0;
                    var equipmentUsed = false;

                    while (sidearms.Any())
                    {
                        var sidearm = sidearms.Take(1).FirstOrDefault();

                        if (sidearm == null)
                        {
                            continue;
                        }

                        if (equipmentCount > 0 && !equipmentUsed)
                        {
                            foreach (var equip in equipment.Where(
                                equip => sidearm.def.defName.Equals(equip.def.defName)
                            ))
                            {
                                weapons.Insert(0, equip.LabelCap);
                                equipmentUsed = true;
                            }

                            if (equipmentUsed)
                            {
                                sidearms.Remove(sidearm);
                                continue;
                            }
                        }

                        foreach (var thing in inventory.Where(
                            thing => sidearm.def.defName.Equals(thing.def.defName)
                        ))
                        {
                            if (usedInventory.Contains(thing))
                            {
                                continue;
                            }

                            weapons.Add(thing.LabelCap);
                            usedInventory.Add(thing);
                            sidearms.Remove(sidearm);
                            break;
                        }

                        loops++;

                        if (loops >= 50)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var e = pawn.equipment;

                    if (e != null && e.AllEquipmentListForReading?.Count > 0)
                    {
                        var equip = e.AllEquipmentListForReading.Select(eq => eq.LabelCap);

                        weapons.AddRange(equip);
                    }
                }

                if (weapons.Any())
                {
                    var section = "Stat_Weapon_Name".Translate().RawText;

                    if (weapons.Count > 1)
                    {
                        section = Find.ActiveLanguageWorker.Pluralize(section);
                    }

                    parts.Add(
                        $"{section}: {string.Join(", ", weapons.ToArray())}"
                    );
                }
            }

            if (!TkSettings.ShowApparel)
            {
                return string.Join("⎮", parts.ToArray());
            }

            var a = pawn.apparel;

            if (a == null || a.WornApparelCount <= 0)
            {
                return string.Join("⎮", parts.ToArray());
            }

            var apparel = a.WornApparel;

            parts.Add(
                $"{"Apparel".Translate().RawText}: {string.Join(", ", apparel.Select(item => item.LabelCap).ToArray())}"
            );

            return !parts.Any()
                ? "None".Translate().RawText
                : string.Join("⎮", parts.ToArray());
        }
    }
}
