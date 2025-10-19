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
using JetBrains.Annotations;

namespace ToolkitUtils.Mod;

/// <summary>
///     Represents a Karma level, indicating a qualitative value with a short integer and a descriptive name. This
///     struct allows for comparison and management of various karma levels.
/// </summary>
/// <remarks>
///     The Karma levels range from SuperBad (-3) to SuperGood (3), with pre-defined instances for each level.
///     Intermediate levels are VeryBad (-2), Bad (-1), Neutral (0), Inherit (0), Unset (0), Good (1), and VeryGood (2).
/// </remarks>
[PublicAPI]
public readonly record struct Karma(sbyte Value, string Name)
{
    private const string SuperBadName = "SUPER_BAD";
    private const string VeryBadName = "VERY_BAD";
    private const string BadName = "BAD";
    private const string NeutralName = "NEUTRAL";
    private const string InheritName = "INHERIT";
    private const string UnsetName = "UNSET";
    private const string GoodName = "GOOD";
    private const string VeryGoodName = "VERY_GOOD";
    private const string SuperGoodName = "SUPER_GOOD";

    private const sbyte SuperBadValue = -3;
    private const sbyte VeryBadValue = -2;
    private const sbyte BadValue = -1;
    private const sbyte NeutralValue = 0;
    private const sbyte InheritValue = sbyte.MinValue;
    private const sbyte GoodValue = 1;
    private const sbyte VeryGoodValue = 2;
    private const sbyte SuperGoodValue = 3;

    /// <summary>Represents a karma level that is extremely undesirable or negative.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of -3 and is described as "Super bad". It is the lowest defined level of
    ///     karma.
    /// </remarks>
    public static readonly Karma SuperBad = new(SuperBadValue, SuperBadName);

    /// <summary>Represents a karma level that is highly undesirable or negative.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of -2 and is described as "Very bad". It is one step above the lowest
    ///     defined karma level.
    /// </remarks>
    public static readonly Karma VeryBad = new(VeryBadValue, VeryBadName);

    /// <summary>Represents a karma level that is undesirable or somewhat negative.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of -1 and is described as "Bad". It is positioned between "Very bad" and
    ///     "Neutral" in the defined karma levels.
    /// </remarks>
    public static readonly Karma Bad = new(BadValue, BadName);

    /// <summary>Represents a neutral karma level that indicates a balance between positive and negative attributes.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of 0 and is described as "Neutral". It serves as a midpoint between
    ///     desirable and undesirable karma levels.
    /// </remarks>
    public static readonly Karma Neutral = new(NeutralValue, NeutralName);

    /// <summary>Represents a neutral karma level where the value or type is to be inherited from another context.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of 0 and is described as "Inherit". It indicates that the karma type or
    ///     value should be determined based on another entity or context instead of being explicitly set.
    /// </remarks>
    public static readonly Karma Inherit = new(InheritValue, InheritName);

    /// <summary>Represents a karma level that is positive or desirable.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of 1 and is described as "Good". It is considered an above-average level
    ///     of karma.
    /// </remarks>
    public static readonly Karma Good = new(GoodValue, GoodName);

    /// <summary>Represents a karma level that is considered very favorable or positive.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of 2 and is described as "Very good". It is one step below the highest
    ///     defined karma level.
    /// </remarks>
    public static readonly Karma VeryGood = new(VeryGoodValue, VeryGoodName);

    /// <summary>Represents a karma level that is exceptionally positive or favorable.</summary>
    /// <remarks>
    ///     This karma level is assigned a value of 3 and is described as "Super good". It is the highest defined level of
    ///     karma.
    /// </remarks>
    public static readonly Karma SuperGood = new(SuperGoodValue, SuperGoodName);

    /// <summary>Converts the Karma value to its equivalent multiplier representation.</summary>
    /// <returns>A float representing the multiplier derived from the Karma's value.</returns>
    public float AsMultiplier()
    {
        // SuperGood -> 1 + (3/2)         -> 2.5
        // VeryGood  -> 1 + (2/2)         -> 2
        // Good      -> 1 + (1/2)         -> 1.5
        // Neutral   ->                   -> 1
        // Bad       -> -(1 + (abs(1)/2)) -> -1.5
        // VeryBad   -> -(1 + (abs(2)/2)) -> -2
        // SuperBad  -> -(1 + (abs(3)/3)) -> -2.5

        return Value switch
        {
            > 0   => Value / 2f,
            < 0   => -(Math.Abs(Value) / 2f),
            var _ => 0,
        };
    }

    /// <summary>Retrieves a predefined Karma object based on the given name.</summary>
    /// <param name="name">The name of the Karma object to retrieve.</param>
    /// <returns>A Karma object that corresponds to the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown when the given name does not match any predefined Karma.</exception>
    public static Karma FromName(string name)
    {
        return name switch
        {
            SuperBadName  => SuperBad,
            VeryBadName   => VeryBad,
            BadName       => Bad,
            NeutralName   => Neutral,
            InheritName   => Inherit,
            GoodName      => Good,
            VeryGoodName  => VeryGood,
            SuperGoodName => SuperGood,
            var _         => throw new ArgumentException($"Unknown karma name: {name}"),
        };
    }

    /// <summary>Retrieves a predefined Karma object based on the given value.</summary>
    /// <param name="value">The numeric value of the Karma object to retrieve.</param>
    /// <returns>A Karma object that corresponds to the specified value.</returns>
    /// <exception cref="ArgumentException">Thrown when the given value does not match any predefined Karma.</exception>
    public static Karma FromValue(sbyte value)
    {
        return value switch
        {
            SuperBadValue  => SuperBad,
            VeryBadValue   => VeryBad,
            BadValue       => Bad,
            NeutralValue   => Neutral,
            InheritValue   => Inherit,
            GoodValue      => Good,
            VeryGoodValue  => VeryGood,
            SuperGoodValue => SuperGood,
            var _          => throw new ArgumentException($"Unknown karma value: {value}"),
        };
    }
}
