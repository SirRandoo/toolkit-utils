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
// with ToolkitUtils.Core. If not, see <https://www.gnu.org/licenses/>.
using System;
using RimWorld;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Domain.Settings;
using UnityEngine;

namespace ToolkitUtils.Core.Extensions;

public static class ProductExtensions
{
    public static int CalculateItemPrice(this ItemProduct product, ItemProduct? stuff = null, QualityCategory? quality = null)
    {
        long price = product.Price;

        if (stuff != null)
        {
            if (TryApplyMaterial(price, stuff, out long costWithMaterial))
                price = costWithMaterial;
            else
                throw new InvalidOperationException($"'{product.Name}' cannot be made of '{stuff.Name}'");
        }

        if (quality == null) return (int)Math.Max(Math.Min(price, int.MaxValue), val2: 0);

        if (TryApplyQualityMultiplier(price, quality.Value, out long costWithQuality))
            price = costWithQuality;
        else
            throw new InvalidOperationException($"'{product.Name}' cannot be made of '{quality.Value}' quality");

        return (int)Math.Max(Math.Min(price, int.MaxValue), val2: 0);
    }

    private static bool TryApplyMaterial(long cost, ItemProduct stuff, out long newCost)
    {
        if (stuff.Metadata is not ItemProductMetadata stuffMetadata)
        {
            newCost = -1;

            return false;
        }

        if (stuffMetadata.Properties.HasFlagFast(ItemProperties.Material))
        {
            newCost = cost;

            return false;
        }

        if (stuffMetadata.Properties.HasFlagFast(ItemProperties.Tiny))
        {
            newCost = cost + Mathf.RoundToInt(stuff.Price * 10f * 1.05f);

            return true;
        }

        newCost = Mathf.RoundToInt((cost + stuff.Price) * 1.05f);

        return true;
    }

    private static bool TryApplyQualityMultiplier(long cost, QualityCategory quality, out long newCost)
    {
        // QualityMultiplierSettings settings = quality switch
        // {
        //     QualityCategory.Awful => SettingsRegistry.Quality.Settings.Awful,
        //     QualityCategory.Poor => SettingsRegistry.Quality.Settings.Poor,
        //     QualityCategory.Normal => SettingsRegistry.Quality.Settings.Normal,
        //     QualityCategory.Good => SettingsRegistry.Quality.Settings.Good,
        //     QualityCategory.Excellent => SettingsRegistry.Quality.Settings.Excellent,
        //     QualityCategory.Masterwork => SettingsRegistry.Quality.Settings.Masterwork,
        //     QualityCategory.Legendary => SettingsRegistry.Quality.Settings.Legendary,
        //     var _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, $"Unsupported quality '{quality}'")
        // };

        // return TryApplyQualityMultiplier(cost, settings, out newCost);

        newCost = cost; // FIXME
        return false;
    }

    private static bool TryApplyQualityMultiplier(long cost, QualityMultiplierSettings settings, out long newCost)
    {
        if (settings.IsEnabled)
        {
            newCost = Mathf.RoundToInt(cost * settings.Multiplier);

            return true;
        }

        newCost = int.MaxValue;

        return false;
    }
}
