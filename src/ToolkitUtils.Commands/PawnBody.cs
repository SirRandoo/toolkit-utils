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
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnBody : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple().WithHeader("HealthOverview".TranslateSimple()));

                return;
            }

            twitchMessage.Reply(GetPawnBody(pawn!).WithHeader("HealthOverview".TranslateSimple()));
        }

        private static float GetListPriority([CanBeNull] BodyPartRecord record) =>
            record == null ? 9999999f : (float)record.height * 10000 + record.coverageAbsWithChildren;

        private static string GetPawnBody([NotNull] Pawn target)
        {
            if (target.health?.hediffSet?.hediffs?.Count <= 0)
            {
                return "NoHealthConditions".TranslateSimple().CapitalizeFirst();
            }

            List<IGrouping<BodyPartRecord, Hediff>> hediffsGrouped = GetVisibleHediffGroupsInOrder(target).ToList();
            var builder = new StringBuilder();

            if (!TkSettings.TempInGear)
            {
                builder.Append(ResponseHelper.TemperatureGlyph.AltText("ComfyTemperatureRange".TranslateSimple()));
                builder.Append(" ");
                builder.Append(target.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature());
                builder.Append("~");
                builder.Append(target.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature());
                builder.Append(ResponseHelper.OuterGroupSeparator.AltText(ResponseHelper.OuterGroupSeparatorAlt));
            }

            var hediffIndex = 0;
            int hediffTotal = hediffsGrouped.Count - 1;

            foreach (IGrouping<BodyPartRecord, Hediff> item in hediffsGrouped)
            {
                builder.Append(item.Key?.LabelCap ?? "WholeBody".TranslateSimple());
                builder.Append(": ");

                var index = 0;
                int total = item.Count() - 1;

                foreach (IGrouping<int, Hediff> group in item.GroupBy(h => h.UIGroupKey))
                {
                    Hediff affliction = group.First();

                    if (group.Any(i => i.Bleeding))
                    {
                        if (TkSettings.Emojis)
                        {
                            builder.Append(ResponseHelper.BleedingGlyph);
                        }
                        else
                        {
                            builder.Append("(");
                            builder.Append("BleedingRate".TranslateSimple());
                            builder.Append(" ");
                            builder.Append(")");
                        }
                    }

                    if (group.All(i => i.IsTended()))
                    {
                        builder.Append(ResponseHelper.BandageGlyph.AltText(""));
                    }

                    builder.Append(RichTextHelper.StripTags(affliction.LabelCap));
                    int gTotal = group.Count();

                    if (gTotal != 1)
                    {
                        builder.Append(" x");
                        builder.Append(gTotal.ToString());
                    }

                    if (index < total)
                    {
                        builder.Append(", ");
                    }

                    index++;
                }

                if (hediffIndex < hediffTotal)
                {
                    builder.Append(ResponseHelper.OuterGroupSeparator.AltText($" {ResponseHelper.OuterGroupSeparatorAlt} "));
                }

                hediffIndex++;
            }

            return builder.ToString();
        }

        [NotNull]
        private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> GetVisibleHediffGroupsInOrder([NotNull] Pawn pawn)
        {
            return GetVisibleHediffs(pawn).GroupBy(x => x.Part).OrderByDescending(x => GetListPriority(x.First().Part));
        }

        private static IEnumerable<Hediff> GetVisibleHediffs([NotNull] Pawn pawn)
        {
            List<Hediff_MissingPart> missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            foreach (Hediff_MissingPart part in missing)
            {
                yield return part;
            }

            IEnumerable<Hediff> e = pawn.health.hediffSet.hediffs.Where(d => !(d is Hediff_MissingPart) && d.Visible);

            foreach (Hediff item in e)
            {
                yield return item;
            }
        }
    }
}
