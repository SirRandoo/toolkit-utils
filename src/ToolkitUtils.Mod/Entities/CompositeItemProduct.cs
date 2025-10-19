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
using ToolkitUtils.Mod.Domain.Products;
using QualitySettings = ToolkitUtils.Mod.Domain.Settings.QualitySettings;

namespace ToolkitUtils.Mod.Entities;

/// <summary>Represents an in-game item made of</summary>
/// <param name="Item"></param>
/// <param name="Quality"></param>
/// <param name="Material"></param>
public record CompositeItemProduct(ItemProduct Item, ItemQuality Quality, ItemProduct? Material = null)
{
    public int Cost { get; init; }

    public bool IsPurchasable(QualitySettings qualitySettings)
    {
        if (!Item.Purchasable) return false;
        if (Material is { Purchasable: false, }) return false;
        if (Quality == ItemQuality.Awful) return qualitySettings.Awful.IsEnabled;
        if (Quality == ItemQuality.Poor) return qualitySettings.Poor.IsEnabled;
        if (Quality == ItemQuality.Normal) return qualitySettings.Normal.IsEnabled;
        if (Quality == ItemQuality.Good) return qualitySettings.Good.IsEnabled;
        if (Quality == ItemQuality.Excellent) return qualitySettings.Excellent.IsEnabled;
        if (Quality == ItemQuality.Masterwork) return qualitySettings.Masterwork.IsEnabled;

        return Quality != ItemQuality.Legendary || qualitySettings.Legendary.IsEnabled;
    }
}
