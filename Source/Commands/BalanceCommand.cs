using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Utilities;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class BalanceCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Viewer viewer = Viewers.GetViewer(twitchMessage.Username);

            if (viewer == null)
            {
                return;
            }

            var container = new List<string>();
            string coins = ToolkitSettings.UnlimitedCoins
                ? ResponseHelper.InfinityGlyph
                : viewer.GetViewerCoins().ToString("N0");
            var karma = (viewer.GetViewerKarma() / 100f).ToString("P0");

            if (TkSettings.Emojis)
            {
                container.Add($"{ResponseHelper.CoinGlyph} {coins}");
                container.Add($"{ResponseHelper.KarmaGlyph} {karma}");
            }
            else
            {
                container.Add(
                    ResponseHelper.JoinPair(
                        "TKUtils.Balance.Coins".Localize().CapitalizeFirst(),
                        coins
                    )
                );

                container.Add(
                    ResponseHelper.JoinPair(
                        "TKUtils.Balance.Karma".Localize().CapitalizeFirst(),
                        karma
                    )
                );
            }

            if (ToolkitSettings.EarningCoins && TkSettings.ShowCoinRate)
            {
                int income = CalculateCoinAward(viewer);

                container.Add(
                    (
                        income > 0
                            ? $"{ResponseHelper.IncomeGlyph} +{income:N0}"
                            : $"{ResponseHelper.DebtGlyph} {income:N0}"
                    ).AltText(
                        "TKUtils.Balance.Rate".Localize(
                            CalculateCoinAward(viewer),
                            ToolkitSettings.CoinInterval
                        )
                    )
                );
            }

            twitchMessage.Reply(container.GroupedJoin());
        }

        private static int CalculateCoinAward(Viewer viewer)
        {
            int baseCoins = ToolkitSettings.CoinAmount;
            float multiplier = viewer.GetViewerKarma() / 100f;

            if (viewer.IsSub)
            {
                baseCoins += ToolkitSettings.SubscriberExtraCoins;
                multiplier *= ToolkitSettings.SubscriberCoinMultiplier;
            }
            else if (viewer.IsVIP)
            {
                baseCoins += ToolkitSettings.VIPExtraCoins;
                multiplier *= ToolkitSettings.VIPCoinMultiplier;
            }
            else if (viewer.mod)
            {
                baseCoins += ToolkitSettings.ModExtraCoins;
                multiplier *= ToolkitSettings.ModCoinMultiplier;
            }

            int minutesElapsed = TimeHelper.MinutesElapsed(viewer.last_seen);

            if (ToolkitSettings.ChatReqsForCoins)
            {
                if (minutesElapsed > ToolkitSettings.TimeBeforeHalfCoins)
                {
                    multiplier *= 0.5f;
                }

                if (minutesElapsed > ToolkitSettings.TimeBeforeNoCoins)
                {
                    multiplier *= 0.0f;
                }
            }

            return (int) Math.Ceiling((double) baseCoins * multiplier);
        }
    }
}
