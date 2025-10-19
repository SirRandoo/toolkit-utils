// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnBody(GearSettings settings, ExecutionContext context) : CommandGroup
{
    private static readonly BodyPartRecordComparer BodyPartRecordComparerInstance = new();
    private static readonly BodyPartRecord[] PresortedBodyParts = PresortBodyParts();

    private static BodyPartRecord[] PresortBodyParts()
    {
        BodyDef def = DefDatabase<BodyDef>.GetNamed("Human");
        var container = new BodyPartRecord[def.AllParts.Count];

        def.AllParts.CopyTo(container);

        Array.Sort(container, index: 0, container.Length, BodyPartRecordComparerInstance);

        return container;
    }

    [Command("body")]
    public async Task<Result> GetPawnBodyInformationAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
        if (pawn.health.hediffSet.hediffs.Count <= 0) return Result.Ok(TranslationService.Instance.GetTranslation("NoHealthConditions").CapitalizeFirst());

        Dictionary<BodyPartRecord, List<Hediff>> visibleHediffs = await RouterService.Instance.RouteToMainAsync(GetAfflictions, pawn);

        var builder = new StringBuilder();

        if (settings.ShouldShowTemperatureRange)
        {
            builder.Append(UnicodeCharacter.Thermometer.Codepoint);
            builder.Append(" ");

            float temperatureMin = await RouterService.Instance.RouteToMainAsync(pawn.GetStatValue, StatDefOf.ComfyTemperatureMin, arg2: true, arg3: -1);
            float temperatureMax = await RouterService.Instance.RouteToMainAsync(pawn.GetStatValue, StatDefOf.ComfyTemperatureMax, arg2: true, arg3: -1);

            builder.Append(await RouterService.Instance.RouteToMainAsync(GenText.ToStringTemperature, temperatureMin, arg2: "F1"));
            builder.Append("~");
            builder.Append(await RouterService.Instance.RouteToMainAsync(GenText.ToStringTemperature, temperatureMax, arg2: "F1"));
            builder.Append(" | ");
        }

        for (var i = 0; i < PresortedBodyParts.Length; i++)
        {
            BodyPartRecord record = PresortedBodyParts[i];

            if (!visibleHediffs.TryGetValue(record, out List<Hediff>? hediffs)) continue;

            int bodyPartPrefixIndex = builder.Length;

            for (var j = 0; j < hediffs.Count; j++)
            {
                Hediff hediff = hediffs[j];

                if (hediff.Bleeding) builder.Insert(bodyPartPrefixIndex++, UnicodeCharacter.Blood.Codepoint);

                if (await RouterService.Instance.RouteToMainAsync(hediff.IsTended)) builder.Insert(bodyPartPrefixIndex++, UnicodeCharacter.Bandage.Codepoint);

                builder.Append(RichTextHelper.StripTags(hediff.LabelCap));
            }

            var allTended = true;

            for (var i1 = 0; i1 < hediffs.Count; i1++)
            {
                Hediff hediff = hediffs[i1];
                if (await RouterService.Instance.RouteToMainAsync(hediff.IsTended)) continue;

                allTended = false;

                break;
            }

            if (allTended) builder.Append(UnicodeCharacter.Bandage.Codepoint);

            builder.Append(RichTextHelper.StripTags(hediffs[0].def.label.CapitalizeFirst()));

            int totalHediffs = hediffs.Count;

            if (totalHediffs != 1) builder.Append($" x{totalHediffs:N0}");

            builder.Append(", ");
        }

        return Result.Ok(builder.ToString(startIndex: 0, builder.Length - 2));
    }

    private static Dictionary<BodyPartRecord, List<Hediff>> GetAfflictions(Pawn pawn)
    {
        var container = new Dictionary<BodyPartRecord, List<Hediff>>();

        List<Hediff_MissingPart> missingParts = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

        for (var i = 0; i < missingParts.Count; i++)
        {
            Hediff_MissingPart part = missingParts[i];

            if (container.TryGetValue(part.Part, out List<Hediff>? hediffs))
                hediffs.Add(part);
            else
                container[part.Part] = [part,];
        }

        for (var i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
        {
            Hediff hediff = pawn.health.hediffSet.hediffs[i];

            if (container.TryGetValue(hediff.Part, out List<Hediff>? hediffs))
                hediffs.Add(hediff);
            else
                container[hediff.Part] = [hediff,];
        }

        return container;
    }

    private sealed class BodyPartRecordComparer : Comparer<BodyPartRecord>
    {
        /// <inheritdoc />
        public override int Compare(BodyPartRecord x, BodyPartRecord y)
        {
            float xComparant = (float)x.height * 10000 + x.coverageAbsWithChildren;
            float yComparant = (float)y.height * 10000 + y.coverageAbsWithChildren;

            return xComparant.CompareTo(yComparant);
        }
    }
}
