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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Concurrent;
using NLog;
using ToolkitUtils.Mod.Logging;
using TwitchToolkit;
using Viewer = ToolkitUtils.Mod.Data.Viewer;

namespace ToolkitUtils.Mod.Services;

/// <summary>
///     Implements the <see cref="IKarmaService" /> interface, providing functionality to calculate changes in karma
///     and associated operations based on viewer attributes, karma values, and resources utilized.
/// </summary>
internal sealed class MomentumKarmaService : IKarmaService
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private readonly ConcurrentDictionary<Viewer, KarmaStreak> _karmaStreaks = new();

    /// <inheritdoc />
    public int CalculateChange(Viewer viewer, Karma karma, int coinsSpent)
    {
        Logger.Trace(message: "Calculating karma change for {ViewerName} (Karma: {KarmaName}, Coins spent: {CoinsSpent})", viewer.Name, karma.Name, coinsSpent);

        double currentKarma = karma.Value;
        double karmaCap = ToolkitSettings.KarmaCap;
        double karmaMin = ToolkitSettings.KarmaMinimum;

        Logger.Trace(message: "Current karma is {CurrentKarma}", currentKarma);
        Logger.Trace(message: "Karma range is {KarmaMinimum}~{KarmaCap}", karmaMin, karmaCap);

        double curveMultiplier = 1.0 + Math.Log(karmaCap + 1) / 7.0;
        double resistanceFactor = 5.0 + Math.Log(currentKarma + 1);

        Logger.Trace(message: "Curve multiplier is {CurveMultiplier}", curveMultiplier);
        Logger.Trace(message: "Resistance factor is {ResistanceFactor}", resistanceFactor);

        double momentum = GetMomentum(viewer, karma);

        Logger.Trace(message: "Momentum is {Momentum}", momentum);

        double karmaChange = coinsSpent * momentum * curveMultiplier / resistanceFactor;

        double direction = karma.Value < 0 ? -1 : 1;

        if (karma == Karma.SuperBad) direction = -2.5;

        double newKarma = Math.Min(currentKarma + direction * karmaChange, karmaCap);

        if (newKarma < 1.0 && ToolkitSettings.BanViewersWhoPurchaseAlwaysBad) newKarma = 1.0;

        newKarma = Math.Max(newKarma, karmaMin);

        var value = (int)Math.Ceiling(newKarma);

        Logger.Trace(message: "Karma change for {ViewerName} is {KarmaChange}", viewer.Name, value);

        return value;
    }

    private double GetMomentum(Viewer viewer, Karma karma)
    {
        if (!_karmaStreaks.TryGetValue(viewer, out KarmaStreak? karmaStreak))
        {
            _karmaStreaks.TryAdd(viewer, new KarmaStreak(karma, Count: 1));

            return 0;
        }

        if ((DateTime.UtcNow - karmaStreak.LastUpdated).TotalSeconds >= 30 && karmaStreak.Count > 0)
        {
            karmaStreak = karmaStreak with
            {
                LastUpdated = DateTime.UtcNow, Count = karmaStreak.Count == 1 ? 0 : (int)Math.Round(karmaStreak.Count / 2f),
            };
        }

        if (karmaStreak.Karma == karma)
        {
            _karmaStreaks.AddOrUpdate(viewer, new KarmaStreak(karma, karmaStreak.Count + 1), updateValueFactory: (_, value) => new KarmaStreak(karma, value.Count + 1));

            return 1.0 + karmaStreak.Count * GetStreakScale(karma);
        }

        _karmaStreaks.AddOrUpdate(viewer, new KarmaStreak(karma, Count: 1), updateValueFactory: (_, _) => new KarmaStreak(karma, Count: 1));

        return 0;
    }

    private static double GetStreakScale(Karma karma)
    {
        if (karma == Karma.SuperBad) return 0.25;
        if (karma == Karma.VeryBad) return 0.18;
        if (karma == Karma.Bad) return 0.12;
        if (karma == Karma.Neutral) return 0.05;
        if (karma == Karma.Good) return 0.1;
        if (karma == Karma.VeryGood) return 0.15;

        return karma == Karma.SuperGood ? 0.2 : 0.0;
    }

    private sealed record KarmaStreak(Karma Karma, int Count, DateTime LastUpdated = default);
}
