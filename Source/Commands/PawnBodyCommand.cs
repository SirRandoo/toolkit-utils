using System.Collections.Generic;
using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnBodyCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetPawnDestructive(message.User);

            if(pawn == null)
            {
                SendCommandMessage("TKUtils.Responses.NoPawn".Translate(), message);
                return;
            }

            SendCommandMessage(
                "TKUtils.Formats.PawnBody.Base".Translate(
                    GetPawnBody(pawn).Named("HEDIFFS")
                ),
                message
            );
        }

        private float GetListPriority(BodyPartRecord record) => record == null ? 9999999f : ((float) record.height * 10000) + record.coverageAbsWithChildren;

        private string GetPawnBody(Pawn target)
        {
            var hediffs = target.health.hediffSet.hediffs;

            if(hediffs == null || !hediffs.Any())
            {
                return "TKUtils.Responses.Healthy".Translate();
            }

            var hediffsGrouped = GetVisibleHediffGroupsInOrder(target);
            var parts = new List<string>();

            if(!TKSettings.TempInGear)
            {
                var tempMin = GenText.ToStringTemperature(StatExtension.GetStatValue(target, StatDefOf.ComfyTemperatureMin, applyPostProcess: true));
                var tempMax = GenText.ToStringTemperature(StatExtension.GetStatValue(target, StatDefOf.ComfyTemperatureMax, applyPostProcess: true));

                parts.Add(
                    "TKUtils.Formats.PawnGear.Temperature".Translate(
                        tempMin.Named("MINIMUM"),
                        tempMax.Named("MAXIMUM"),
                        GetTemperatureDisplay().Named("DISPLAY")
                    )
                );
            }
            foreach(var item in hediffsGrouped)
            {
                var bits = new List<string>();

                foreach(var group in item.GroupBy(h => h.UIGroupKey))
                {
                    var bleeding = "";
                    var amount = "";
                    var count = group.Where(i => i.Bleeding).Count();
                    var total = group.Count();

                    if(total != 1)
                    {
                        amount = $" x{total.ToString()}";
                    }

                    if(count > 0)
                    {
                        bleeding += GetTranslatedEmoji("TKUtils.Formats.PawnBody.Bleeding").Translate();
                    }

                    bits.Add($"{bleeding}{group.First().LabelCap}{amount}");
                }

                parts.Add(
                    "TKUtils.Formats.PawnBody.Affliction".Translate(
                        (item.Key != null ? item.Key.LabelCap : "WholeBody".Translate().ToString()).Named("PART"),
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            bits
                        ).Named("HEDIFFS")
                    )
                );
            }

            return string.Join("TKUtils.Misc.Separators.Upper".Translate(), parts);
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

        private IEnumerable<IGrouping<BodyPartRecord, Hediff>> GetVisibleHediffGroupsInOrder(Pawn pawn)
        {
            return GetVisibleHediffs(pawn)
                .GroupBy(x => x.Part)
                .OrderByDescending(x => GetListPriority(x.First().Part));
        }

        private IEnumerable<Hediff> GetVisibleHediffs(Pawn pawn)
        {
            var missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            foreach(var part in missing)
            {
                yield return part;
            }

            var e = pawn.health.hediffSet.hediffs.Where(d => !(d is Hediff_MissingPart) && d.Visible);

            foreach(var item in e)
            {
                yield return item;
            }
        }
    }
}
