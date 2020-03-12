using System.Collections.Generic;
using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnGearCommand : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetPawnDestructive(message.Username);

            if(pawn == null)
            {
                SendCommandMessage(
                    "TKUtils.Responses.NoPawn".Translate(),
                    message
                );
                return;
            }

            SendCommandMessage(
                "TKUtils.Formats.PawnGear.Base".Translate(
                    GetPawnGear(pawn).Named("COMPOSITE")
                ),
                message
            );
        }

        private float CalculateArmorRating(Pawn pawn, StatDef stat)
        {
            var rating = 0f;
            var value = Mathf.Clamp01(StatExtension.GetStatValue(pawn, stat, applyPostProcess: true) / 2f);
            var parts = pawn.RaceProps.body.AllParts;
            var apparel = pawn.apparel?.WornApparel;

            foreach(var part in parts)
            {
                var cache = 1f - value;

                if(apparel != null && apparel.Any())
                {
                    foreach(var a in apparel)
                    {
                        if(a.def.apparel.CoversBodyPart(part))
                        {
                            float v = Mathf.Clamp01(StatExtension.GetStatValue(a, stat, applyPostProcess: true) / 2f);
                            cache *= 1f - v;
                        }
                    }
                }

                rating += part.coverageAbs * (1f - cache);
            }

            return Mathf.Clamp(rating * 2f, 0f, 2f);
        }

        private TaggedString GetPawnGear(Pawn pawn)
        {
            var parts = new List<string>();

            if(TKSettings.TempInGear)
            {
                var tempMin = GenText.ToStringTemperature(StatExtension.GetStatValue(pawn, StatDefOf.ComfyTemperatureMin, applyPostProcess: true));
                var tempMax = GenText.ToStringTemperature(StatExtension.GetStatValue(pawn, StatDefOf.ComfyTemperatureMax, applyPostProcess: true));

                parts.Add(
                    "TKUtils.Formats.PawnGear.Temperature".Translate(
                        tempMin.Named("MINIMUM"),
                        tempMax.Named("MAXIMUM"),
                        GetTemperatureDisplay().Named("DISPLAY")
                    )
                );
            }

            if(TKSettings.ShowArmor)
            {
                var sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);

                parts.Add(
                    "TKUtils.Formats.PawnGear.Armor".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            new string[]
                            {
                                GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Sharp").Translate(
                                    string.Format("{0:P2}", sharp).Named("SHARP")
                                ),
                                GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Blunt").Translate(
                                    string.Format("{0:P2}", blunt).Named("BLUNT")
                                ),
                                GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Heat").Translate(
                                    string.Format("{0:P2}", heat).Named("HEAT")
                                )
                            }
                        ).Named("STATS")
                    )
                );
            }

            if(TKSettings.ShowWeapon)
            {
                var e = pawn.equipment;

                if(e != null && e.AllEquipmentListForReading?.Count > 0)
                {
                    parts.Add(
                        "TKUtils.Formats.PawnGear.Equipment".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                e.AllEquipmentListForReading.Select(i => i.LabelCap)
                            ).Named("EQUIPMENT")
                        )
                    );
                }
            }

            if(TKSettings.ShowApparel)
            {
                var a = pawn.apparel;

                if(a != null && a.WornApparelCount > 0)
                {
                    parts.Add(
                        "TKUtils.Formats.PawnGear.Apparel".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                a.WornApparel.Select(i => i.LabelCap)
                            ).Named("APPAREL")
                        )
                    );
                }
            }

            return string.Join(
                "TKUtils.Misc.Separators.Upper".Translate(),
                parts
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
                    return "[U]";
            }
        }
    }
}
