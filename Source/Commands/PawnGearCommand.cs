using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnGearCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabGear".Translate()));
                return;
            }

            message.Reply(GetPawnGear(pawn).WithHeader("TabGear".Translate()));
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

                    if(TemperatureCheck && display.RawText.Contains("["))
                    {
                        Logger.Info("Custom temperature display detected; omitting temperature scale from now on.");
                        IsTemperatureCustom = true;
                        TemperatureCheck = false;

                        display = TaggedString.Empty;
                    }
                }

            if (TkSettings.ShowArmor)
            {
                var sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var stats = new List<string>();

                if (sharp > 0)
                {
                    stats.Add(
                        GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Sharp").Translate(
                            GenText.ToStringPercent(sharp).Named("SHARP")
                        )
                    );
                }

                if (blunt > 0)
                {
                    stats.Add(
                        GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Blunt").Translate(
                            GenText.ToStringPercent(blunt).Named("BLUNT")
                        )
                    );
                }

                if (heat > 0)
                {
                    stats.Add(
                        GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Heat").Translate(
                            GenText.ToStringPercent(heat).Named("HEAT")
                        )
                    );
                }

                parts.Add(
                    "TKUtils.Formats.PawnGear.Armor".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            stats.ToArray()
                        ).Named("STATS")
                    )
                );
            }

            if (TkSettings.ShowWeapon)
            {
                var e = pawn.equipment;

                if (e != null && e.AllEquipmentListForReading?.Count > 0)
                {
                    var equip = e.AllEquipmentListForReading.Select(eq => eq.LabelCap);

                    foreach(var eq in equip)
                    {
                        container.Add(eq.LabelCap);
                    }

                    parts.Add(
                        "TKUtils.Formats.PawnGear.Equipment".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                container.ToArray()
                            ).Named("EQUIPMENT")
                        )
                    );
                }
            }

            if (!TkSettings.ShowApparel)
            {
                var a = pawn.apparel;

                if(a != null && a.WornApparelCount > 0)
                {
                    var container = new List<string>();
                    var apparel = a.WornApparel;

                    foreach(var item in apparel)
                    {
                        container.Add(item.LabelCap);
                    }

                    parts.Add(
                        "TKUtils.Formats.PawnGear.Apparel".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                container.ToArray()
                            ).Named("APPAREL")
                        )
                    );
                }
            }

            return string.Join(
                "TKUtils.Misc.Separators.Upper".Translate(),
                parts.ToArray()
            );
        }

        private TaggedString GetTemperatureDisplay()
        {
            switch(Prefs.TemperatureMode)
            {
                case TemperatureDisplayMode.Fahrenheit:
                    return "TKUtils.Misc.Temperature.Fahrenheit".Translate();

                case TemperatureDisplayMode.Kelvin:
                    return "TKUtils.Misc.Temperature.Kelvin".Translate();

                case TemperatureDisplayMode.Celsius:
                    return "TKUtils.Misc.Temperature.Celsius".Translate();

                default:
                    return "?";
            }
        }
    }
}
