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
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;
using Result = ToolkitUtils.Mod.Result;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class WeatherBuyGroup : CommandGroup
{
    [Group("weather")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class WeatherGroup(ExecutionContext context, ILetterService letterService) : CommandGroup
    {
        [Command("set")]
        public async Task<Result> SetWeatherAsync(WeatherProduct weather)
        {
            if (!weather.Enabled) return Result.Fail(TranslationService.Instance.FormatProductDisabled(weather));
            if (Current.Game == null) return Result.Fail(TranslationService.Instance.FormatNoLoadedGame());
            if (Find.Maps == null || Find.Maps.Count <= 0) return Result.Fail(TranslationService.Instance.FormatNoMapFound());

            Result<Transaction> transaction = context.Invoker.ReserveCoins(weather.Price);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(weather.Price, context.Invoker.Coins));

            IReadOnlyList<Map> candidates =
                await RouterService.Instance.RouteToMainAsync(() => Find.Maps.FindAll(m => m.IsPlayerHome && !m.IsTempIncidentMap && !m.IsTempIncidentMap));

            if (candidates.Count <= 0) return Result.Fail(TranslationService.Instance.GetNoPlayerMapError());

            foreach (Map map in candidates) await RouterService.Instance.RouteToMainAsync(map.weatherManager.TransitionTo, weather.Def);
            transaction.Value.PostTransaction();
            context.Invoker.SetKarma(context.Invoker.Karma * weather.Karma.Value);

            string letterTitle = TranslationService.Instance.GetTranslation("TKUtils.Letters.Weather.Title");
            string letterBody = TranslationService.Instance.GetTranslation("TKUtils.Letters.Weather.Body").Format(
                new
                {
                    WeatherName = weather.Def.label,
                }
            );

            if (weather.Metadata is { IsBad: true, })
                await letterService.SendNegativeLetterAsync(letterTitle, letterBody, LookTargets.Invalid);
            else
                await letterService.SendNeutralLetterAsync(letterTitle, letterBody, LookTargets.Invalid);

            return Result.Ok(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Weather.Set").Format(
                    new
                    {
                        WeatherName = weather.Def.label,
                    }
                )
            );
        }
    }
}
