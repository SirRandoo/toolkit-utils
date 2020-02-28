using System.Linq;
using System.Text;

using RimWorld;

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
            if(!CommandsHandler.AllowCommand(message)) return;

            var pawn = GetPawn(message.User);

            if(pawn == null)
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named("TKUtils.Responses.NoPawn".Translate(), "MESSAGE")
                    ),
                    message
                );
                return;
            }

            var builder = new StringBuilder("TKUtils.Responses.GearWord".Translate());
            builder.Append(":");

            if(Settings.TempInGear)
            {
                var tempMin = StatExtension.GetStatValue(pawn, StatDefOf.ComfyTemperatureMin, applyPostProcess: true);
                var tempMax = StatExtension.GetStatValue(pawn, StatDefOf.ComfyTemperatureMax, applyPostProcess: true);

                builder.Append("🌡 ");
                builder.Append($"Min: {GenText.ToStringTemperature(tempMin, format: "F1")}, ");
                builder.Append($"Max: {GenText.ToStringTemperature(tempMax, format: "F1")}");
            }

            if(Settings.ShowArmor)
            {
                builder.Append($" | {"TKUtils.Responses.ArmorWord".Translate()}: ");
                var sharp = Mathf.Round(CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp) * 100f);
                var blunt = Mathf.Round(CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp) * 100f);
                var heat = Mathf.Round(CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp) * 100f);

                if(sharp > 0) builder.Append($"🔪 {sharp}%, ");
                if(blunt > 0) builder.Append($"🍳 {blunt}%, ");
                if(heat > 0) builder.Append($"🔥 {heat}%");
            }

            if(Settings.ShowWeapon)
            {
                var e = pawn.equipment;
                if(e != null && e.AllEquipmentListForReading?.Count > 0)
                {
                    builder.Append($" | {"TKUtils.Responses.EquipmentWord".Translate()}: ");
                    builder.Append(
                        string.Join(
                            ", ",
                            e.AllEquipmentListForReading.Select(i => i.LabelCap)
                        )
                    );
                }
            }

            if(Settings.ShowApparel)
            {
                var a = pawn.apparel;
                if(a != null && a.WornApparelCount > 0)
                {
                    builder.Append($" | {"TKUtils.Responses.ApparelWord".Translate()}: ");
                    builder.Append(
                        string.Join(
                            ", ",
                            pawn.apparel.WornApparel.Select(i => i.LabelCap)
                        )
                    );
                }
            }

            SendMessage(
                "TKUtils.Responses.Format".Translate(
                    NamedArgumentUtility.Named(message.User, "VIEWER"),
                    NamedArgumentUtility.Named(builder.ToString(), "MESSAGE")
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
    }
}
