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
    public class PawnGearCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize().WithHeader("TabGear".Localize()));
                return;
            }

            twitchMessage.Reply(GetPawnGear(pawn).WithHeader("TabGear".Localize()));
        }

        private static float CalculateArmorRating(Pawn pawn, StatDef stat)
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
                string tempMin = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
                string tempMax = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

                parts.Add(
                    $"{ResponseHelper.TemperatureGlyph.AltText("ComfyTemperatureRange".Localize())}{tempMin}~{tempMax}"
                );
            }

            if (TkSettings.ShowArmor)
            {
                float sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                float blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Blunt);
                float heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Heat);
                var stats = new List<string>();

                if (sharp > 0)
                {
                    stats.Add(
                        $"{ResponseHelper.DaggerGlyph.AltText("ArmorSharp".Localize())}{sharp.ToStringPercent()}"
                    );
                }

                if (blunt > 0)
                {
                    stats.Add($"{ResponseHelper.PanGlyph.AltText("ArmorBlunt".Localize())}{blunt.ToStringPercent()}");
                }

                if (heat > 0)
                {
                    stats.Add($"{ResponseHelper.FireGlyph.AltText("ArmorHeat".Localize())}{heat.ToStringPercent()}");
                }

                if (stats.Any())
                {
                    parts.Add($"{"OverallArmor".Localize()}: {stats.SectionJoin()}");
                }
            }

            if (TkSettings.ShowWeapon)
            {
                List<Thing> sidearms = SimpleSidearms.GetSidearms(pawn)?.ToList();
                var weapons = new List<string>();
                List<ThingWithComps> equipment =
                    pawn.equipment?.AllEquipmentListForReading ?? new List<ThingWithComps>();
                int equipmentCount = equipment.Count;
                List<Thing> inventory = pawn.inventory.innerContainer.InnerListForReading ?? new List<Thing>();
                var usedInventory = new List<Thing>();

                if (sidearms?.Any() ?? false)
                {
                    var loops = 0;
                    var equipmentUsed = false;

                    while (sidearms.Any())
                    {
                        Thing sidearm = sidearms.Take(1).FirstOrDefault();

                        if (sidearm == null)
                        {
                            continue;
                        }

                        if (equipmentCount > 0 && !equipmentUsed)
                        {
                            foreach (ThingWithComps equip in equipment.Where(
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

                        foreach (Thing thing in inventory.Where(thing => sidearm.def.defName.Equals(thing.def.defName)))
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
                    Pawn_EquipmentTracker e = pawn.equipment;

                    if (e != null && e.AllEquipmentListForReading?.Count > 0)
                    {
                        IEnumerable<string> equip = e.AllEquipmentListForReading.Select(eq => eq.LabelCap);

                        weapons.AddRange(equip);
                    }
                }

                if (weapons.Any())
                {
                    string section = "Stat_Weapon_Name".Localize();

                    if (weapons.Count > 1)
                    {
                        section = section.Pluralize();
                    }

                    parts.Add($"{section}: {weapons.SectionJoin()}");
                }
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

            parts.Add($"{"Apparel".Localize()}: {apparel.Select(item => item.LabelCap).SectionJoin()}");

            return !parts.Any() ? "None".Localize() : parts.GroupedJoin();
        }
    }
}
