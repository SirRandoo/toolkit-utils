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
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Factions : CommandGroup
{
    [Command("factions")]
    public async Task<Result> GetFactionsAsync()
    {
        if (Current.Game == null) return Result.Fail(TranslationService.Instance.GetNoGameError());

        FactionData[] total = await RouterService.Instance.RouteToMainAsync(GetAllFactions);

        if (total.Length <= 0) return Result.Ok(TranslationService.Instance.FormatNoFactions());

        var container = new string[total.Length];

        for (var index = 0; index < total.Length; index++)
        {
            FactionData faction = total[index];

            Translation kindText = faction.RelationKind switch
            {
                FactionRelationKind.Ally    => TranslationService.Instance.GetTranslation("AllyLower"),
                FactionRelationKind.Hostile => TranslationService.Instance.GetTranslation("HostileLower"),
                FactionRelationKind.Neutral => TranslationService.Instance.GetTranslation("NeutralLower"),
                var _                       => Translation.None,
            };

            container[index] = kindText != Translation.None ? $"{faction.Name} ({kindText}) : {faction.Goodwill:+#;-#;0})" : $"{faction.Name} : {faction.Goodwill:+#;-#;0})";
        }

        return Result.Ok(string.Join(separator: ", ", container));
    }

    private static FactionData[] GetAllFactions()
    {
        List<Faction> factions = Find.FactionManager.AllFactionsListForReading;
        var container = new FactionData[factions.Count];

        for (var index = 0; index < factions.Count; index++)
        {
            Faction faction = factions[index];

            if (faction.IsPlayer || faction.Hidden) continue;

            container[index] = new FactionData(faction.Name.CapitalizeFirst(), faction.PlayerGoodwill, faction.PlayerRelationKind);
        }

        return container;
    }

    private record struct FactionData(string Name, int Goodwill, FactionRelationKind RelationKind);
}
