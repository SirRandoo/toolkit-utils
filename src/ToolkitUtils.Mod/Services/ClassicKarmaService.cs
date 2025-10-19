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
using System.Collections.Generic;
using TwitchToolkit;
using Viewer = ToolkitUtils.Mod.Data.Viewer;

namespace ToolkitUtils.Mod.Services;

internal sealed class ClassicKarmaService : IKarmaService
{
    private static readonly (double threshold, Dictionary<Karma, double> multipliers)[] TierMapping = GetDefaultTierMapping();

    public int CalculateChange(Viewer viewer, Karma karma, int coinsSpent)
    {
        double tier = (double)viewer.Karma / ToolkitSettings.KarmaCap;
        double curveValue = CalculateForCurve();
        double newKarma = viewer.Karma;

        Helper.Log($"Calculating karma change for {viewer.Name} (Karma: {karma.Name}, Coins spent: {coinsSpent})");

        if (karma == Karma.SuperBad)
        {
            newKarma -= coinsSpent * (ToolkitSettings.KarmaCap / 100.0) / ToolkitSettings.DoomBonus;

            if (tier < 0.061) return 0;
        }
        else
        {
            double bonus = GetBonusMultiplier(tier, karma);
            newKarma += coinsSpent * curveValue / bonus * (karma == Karma.Bad || karma == Karma.VeryBad ? -1 : 1);
        }

        newKarma = Math.Max(newKarma, ToolkitSettings.BanViewersWhoPurchaseAlwaysBad ? 1.0 : ToolkitSettings.KarmaMinimum);

        return Math.Min((int)Math.Ceiling(newKarma), ToolkitSettings.KarmaCap);
    }

    private static double GetBonusMultiplier(double tier, Karma karma)
    {
        foreach ((double threshold, Dictionary<Karma, double>? multipliers) in TierMapping)
        {
            if (tier > threshold && multipliers.TryGetValue(karma, out double bonus)) return bonus;
        }

        return 1;
    }

    private static double CalculateForCurve() => 0.00116279069f * ToolkitSettings.KarmaCap + 0.8372093f;

    private static (double threshold, Dictionary<Karma, double> multipliers)[] GetDefaultTierMapping() =>
    [
        (0.55, new Dictionary<Karma, double>
        {
            {
                Karma.Good, ToolkitSettings.TierOneGoodBonus
            },
            {
                Karma.Neutral, ToolkitSettings.TierOneNeutralBonus
            },
            {
                Karma.Bad, ToolkitSettings.TierOneBadBonus
            },
        }),
        (0.36, new Dictionary<Karma, double>
        {
            {
                Karma.Good, ToolkitSettings.TierTwoGoodBonus
            },
            {
                Karma.Neutral, ToolkitSettings.TierTwoNeutralBonus
            },
            {
                Karma.Bad, ToolkitSettings.TierTwoBadBonus
            },
        }),
        (0.06, new Dictionary<Karma, double>
        {
            {
                Karma.Good, ToolkitSettings.TierThreeGoodBonus
            },
            {
                Karma.Neutral, ToolkitSettings.TierThreeNeutralBonus
            },
            {
                Karma.Bad, ToolkitSettings.TierThreeBadBonus
            },
        }),
        (0.00, new Dictionary<Karma, double>
        {
            {
                Karma.Good, ToolkitSettings.TierFourGoodBonus
            },
            {
                Karma.Neutral, ToolkitSettings.TierFourNeutralBonus
            },
            {
                Karma.Bad, ToolkitSettings.TierFourBadBonus
            },
        }),
    ];
}
