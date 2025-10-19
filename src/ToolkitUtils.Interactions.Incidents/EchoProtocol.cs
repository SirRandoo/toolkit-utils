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
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Extensions;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Sanctuary(ExecutionContext context, ILetterService letterService) : CommandGroup
{
    [Command("echoprotocol")]
    public async Task<Result> InitiateEchoProtocolAsync()
    {
        Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.Sanctuary.cost);

        if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.Sanctuary.cost, context.Invoker.Coins));

        Map[] maps = await Current.Game.GetMapsAsync();
        Map[] playerMaps = Array.FindAll(maps, match: m => m.IsPlayerHome && !m.GameConditionManager.ConditionIsActive(GameConditionDefOfs.EchoProtocol));

        if (playerMaps.Length <= 0) return Result.Fail(TranslationService.Instance.GetNoPlayerMapError());

        for (var i = 0; i < playerMaps.Length; i++)
        {
            Map playerMap = playerMaps[i];

            GameCondition condition = await MainThreadExtensions.OnMainAsync(() => GameConditionMaker.MakeCondition(
                    GameConditionDefOfs.EchoProtocol,
                    Rand.Range(minInclusive: 2, maxExclusive: 6) * 60_000
                )
            );

            await MainThreadExtensions.OnMainAsync(playerMap.gameConditionManager.RegisterCondition, condition);
        }

        context.Invoker.Charge(StoreIncidentDefOfs.Sanctuary);

        await letterService.SendPositiveLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.EchoBody.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.EchoProtocol.Body"),
            new LookTargets()
        );

        return Result.Ok(TranslationService.Instance.GetTranslation("TKUtils.Responses.EchoProtocol.Complete"));
    }
}
