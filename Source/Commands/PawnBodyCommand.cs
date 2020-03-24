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

            var pawn = GetOrFindPawn(message.User);

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
                TaggedString display;

                if(IsTemperatureCustom)
                {
                    display = TaggedString.Empty;
                }
                else
                {
                    display = GetTemperatureDisplay();

                    if(TemperatureCheck && display.RawText.Contains("["))
                    {
                        Logger.Info("Custom temperature display detected; omitting temperature scale from now on.");
                        IsTemperatureCustom = true;
                        TemperatureCheck = false;

                        display = TaggedString.Empty;
                    }
                }

                parts.Add(
                    "TKUtils.Formats.PawnGear.Temperature".Translate(
                        tempMin.Named("MINIMUM"),
                        tempMax.Named("MAXIMUM"),
                        display.Named("DISPLAY")
                    )
                );
            }
            foreach(var item in hediffsGrouped)
            {
                var bodyPart = item.Key?.LabelCap ?? "WholeBody".Translate();
                var bits = new List<string>();

                foreach(var group in item.GroupBy(h => h.UIGroupKey))
                {
                    var display = group.First().LabelCap;
                    var count = group.Where(i => i.Bleeding).Count();
                    var total = group.Count();

                    if(total != 1)
                    {
                        display += $" x{total.ToString()}";
                    }

                    if(count > 0)
                    {
                        display = GetTranslatedEmoji("TKUtils.Formats.PawnBody.Bleeding").Translate() + display;
                    }

                    bits.Add(display);
                }

                parts.Add(
                    "TKUtils.Formats.PawnBody.Affliction".Translate(
                        bodyPart.Named("PART"),
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            bits.ToArray()
                        ).Named("HEDIFFS")
                    )
                );
            }

            return string.Join("TKUtils.Misc.Separators.Upper".Translate(), parts.ToArray());
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
