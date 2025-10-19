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
using ToolkitUtils.Mod.Data;

namespace ToolkitUtils.Mod.Services;

/// <summary>
///     Provides an interface for managing and calculating karma-related operations for viewers, incorporating
///     attributes such as viewer information, specific karma values, and resources spent.
/// </summary>
public interface IKarmaService
{
    /// <summary>
    ///     Calculates the change in karma for a specific viewer based on the given karma value and the number of coins
    ///     spent.
    /// </summary>
    /// <param name="viewer">The viewer whose karma is being updated. Implements IViewer interface.</param>
    /// <param name="karma">The karma record containing value and name to be applied.</param>
    /// <param name="coinsSpent">The total coins spent, which may influence the calculation of the karma change.</param>
    /// <returns>The updated karma value as an integer.</returns>
    int CalculateChange(Viewer viewer, Karma karma, int coinsSpent);
}
