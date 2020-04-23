﻿using System;
using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Utilities;
using Verse;
using Viewer = TwitchToolkit.Viewer;
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class BalanceCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var viewer = Viewers.GetViewer(twitchMessage.Username);
    
            if (viewer == null)
            {
                return;
            }
    
            var container = new List<string> { };
            var coins = ToolkitSettings.UnlimitedCoins ? "∞" : viewer.GetViewerCoins().ToString("N0");
            var karma = (viewer.GetViewerKarma() / 100f).ToString("P0");
    
            if (TkSettings.Emojis)
            {
                container.Add($"💰 {coins}");
                container.Add($"⚖ {karma}");
            }
            else
            {
                container.Add(
                    "TKUtils.Formats.KeyValue".Translate(
                        "TKUtils.Responses.Balance.Coins".Translate().CapitalizeFirst(),
                        coins
                    )
                );
    
                container.Add(
                    "TKUtils.Formats.KeyValue".Translate(
                        "TKUtils.Responses.Balance.Karma".Translate().CapitalizeFirst(),
                        karma
                    )
                );
            }
    
            if (ToolkitSettings.EarningCoins)
            {
                var income = CalculateCoinAward(viewer);
    
                container.Add(
                    (
                        income > 0
                            ? $"📈 {income:N0}"
                            : $"📉 {income:N0}"
                    ).AltText(
                        "TKUtils.Responses.Balance.Rate".Translate(
                            CalculateCoinAward(viewer),
                            ToolkitSettings.CoinInterval
                        )
                    )
                );
            }
    
            twitchMessage.Reply(string.Join("⎮", container.ToArray()));
        }
    
        private static int CalculateCoinAward(Viewer viewer)
        {
            var baseCoins = ToolkitSettings.CoinAmount;
            var multiplier = viewer.GetViewerKarma() / 100f;
    
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
    
            var minutesElapsed = TimeHelper.MinutesElapsed(viewer.last_seen);
    
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
