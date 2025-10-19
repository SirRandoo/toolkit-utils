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
using RimWorld;

namespace ToolkitUtils.Mod;

/// <summary>Represents the quality of an item derived from the game's quality categories.</summary>
/// <remarks>
///     The ItemQuality struct defines a model to encapsulate the name and value associated with a predefined quality
///     level for items. It also provides pre-defined qualities through static properties for ease of use.
/// </remarks>
[PublicAPI]
public readonly record struct ItemQuality(string Name, sbyte Value)
{
    private const string AwfulName = nameof(QualityCategory.Awful);
    private const string PoorName = nameof(QualityCategory.Poor);
    private const string NormalName = nameof(QualityCategory.Normal);
    private const string GoodName = nameof(QualityCategory.Good);
    private const string ExcellentName = nameof(QualityCategory.Excellent);
    private const string MasterworkName = nameof(QualityCategory.Masterwork);
    private const string LegendaryName = nameof(QualityCategory.Legendary);
    private const string UnsetName = "Unset";

    private const int AwfulValue = -2;
    private const int PoorValue = -1;
    private const int NormalValue = 0;
    private const int GoodValue = 1;
    private const int ExcellentValue = 2;
    private const int MasterworkValue = 3;
    private const int LegendaryValue = 4;
    private const int UnsetValue = sbyte.MinValue;

    /// <summary>
    ///     Represents the "Awful" quality level of an item, characterized by its low value and reliability. Commonly used
    ///     to identify items of the lowest quality in a customizable quality hierarchy.
    /// </summary>
    public static readonly ItemQuality Awful = new(AwfulName, AwfulValue);

    /// <summary>
    ///     Represents an item quality categorized as "Poor." This quality indicates items that have a lower-tier value,
    ///     typically used to denote substandard or below-average condition. Commonly used as a predefined static instance in
    ///     scenarios where item quality influences behavior or categorization.
    /// </summary>
    public static readonly ItemQuality Poor = new(PoorName, PoorValue);

    /// <summary>
    ///     Represents an item quality with a neutral value and the name "Normal". Used as the standard reference point
    ///     for quality comparisons or classifications.
    /// </summary>
    public static readonly ItemQuality Normal = new(NormalName, NormalValue);

    /// <summary>
    ///     Represents an item quality classified as "Good," typically indicating a moderate level of quality. This is a
    ///     predefined constant instance of the ItemQuality record. Commonly used to describe a middling level of quality in a
    ///     range of item categorizations.
    /// </summary>
    public static readonly ItemQuality Good = new(GoodName, GoodValue);

    /// <summary>
    ///     Represents an item quality with a high level of excellence and a predefined value. Typically used to define or
    ///     filter items of this specific quality in the domain. It is part of the set of predefined qualities mapped to
    ///     different categories and values for item classification. The name associated with this quality is "Excellent", and
    ///     the standard value is 2.
    /// </summary>
    public static readonly ItemQuality Excellent = new(ExcellentName, ExcellentValue);

    /// <summary>
    ///     Represents an item quality categorized as "Masterwork." Typically associated with higher-tier or exceptional
    ///     quality items. Commonly utilized in item quality comparison or filtering logic.
    /// </summary>
    public static readonly ItemQuality Masterwork = new(MasterworkName, MasterworkValue);

    /// <summary>
    ///     Represents the highest quality level for an item, indicating exceptional value or rarity. Primarily used for
    ///     categorization or filtering within the system. This is a predefined static instance of the ItemQuality record
    ///     associated with the Legendary quality category.
    /// </summary>
    public static readonly ItemQuality Legendary = new(LegendaryName, LegendaryValue);

    /// <summary>Represents an undefined or uninitialized state for item quality.</summary>
    /// <remarks>
    ///     This value is used to signify that the quality of an item has not been set or determined. It is represented by
    ///     the name "Unset" and the minimum sbyte value.
    /// </remarks>
    public static readonly ItemQuality Unset = new(UnsetName, UnsetValue);

    /// <summary>Retrieves an <see cref="ItemQuality" /> instance corresponding to the specified name.</summary>
    /// <param name="name">The name of the item quality to retrieve.</param>
    /// <returns>An <see cref="ItemQuality" /> instance matching the provided name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified name does not match any predefined quality.</exception>
    public static ItemQuality FromName(string name)
    {
        return name switch
        {
            AwfulName      => Awful,
            PoorName       => Poor,
            NormalName     => Normal,
            GoodName       => Good,
            ExcellentName  => Excellent,
            MasterworkName => Masterwork,
            LegendaryName  => Legendary,
            UnsetName      => Unset,
            var _          => throw new ArgumentOutOfRangeException(nameof(name), message: "No suitable quality conversion found."),
        };
    }

    /// <summary>Retrieves an <see cref="ItemQuality" /> instance corresponding to the specified value.</summary>
    /// <param name="value">The value of the item quality to retrieve.</param>
    /// <returns>An <see cref="ItemQuality" /> instance matching the provided value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified value does not match any predefined quality.</exception>
    public static ItemQuality FromValue(sbyte value)
    {
        return value switch
        {
            AwfulValue      => Awful,
            PoorValue       => Poor,
            NormalValue     => Normal,
            GoodValue       => Good,
            ExcellentValue  => Excellent,
            MasterworkValue => Masterwork,
            LegendaryValue  => Legendary,
            UnsetValue      => Unset,
            var _           => throw new ArgumentOutOfRangeException(nameof(value), message: "No suitable quality conversion found."),
        };
    }

    /// <summary>Converts a <see cref="QualityCategory" /> value to its corresponding <see cref="ItemQuality" /> instance.</summary>
    /// <param name="category">The <see cref="QualityCategory" /> value to convert.</param>
    /// <returns>An <see cref="ItemQuality" /> instance corresponding to the specified <see cref="QualityCategory" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified category does not match any predefined quality.</exception>
    public static ItemQuality FromQualityCategory(QualityCategory category)
    {
        return category switch
        {
            QualityCategory.Poor       => Poor,
            QualityCategory.Normal     => Normal,
            QualityCategory.Good       => Good,
            QualityCategory.Excellent  => Excellent,
            QualityCategory.Masterwork => Masterwork,
            QualityCategory.Legendary  => Legendary,
            QualityCategory.Awful      => Awful,
            var _                      => throw new ArgumentOutOfRangeException(nameof(category), message: "No suitable quality conversion found."),
        };
    }

    public static implicit operator QualityCategory(ItemQuality quality)
    {
        if (quality == Awful) return QualityCategory.Awful;
        if (quality == Poor) return QualityCategory.Poor;
        if (quality == Normal) return QualityCategory.Normal;
        if (quality == Good) return QualityCategory.Good;
        if (quality == Excellent) return QualityCategory.Excellent;
        if (quality == Masterwork) return QualityCategory.Masterwork;

        return quality == Legendary ? QualityCategory.Legendary : throw new ArgumentOutOfRangeException(nameof(quality), message: "No suitable quality conversion found.");
    }
}
