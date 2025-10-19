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
namespace ToolkitUtils.Mod;

/// <summary>Represents a type of karma service, defined by a name and a numerical value.</summary>
public readonly record struct KarmaServiceType(string Name, sbyte Value)
{
    /// <summary>
    ///     Represents a predefined type of karma service with name "Classic" and an associated value of 0. Commonly used
    ///     to denote the "Classic" behavior or configuration of the karma service.
    /// </summary>
    public static readonly KarmaServiceType Classic = new(Name: "Classic", Value: 0);

    /// <summary>
    ///     Represents the "Momentum" karma service type, which corresponds to a specific implementation of the
    ///     IKarmaService. This service type is identified by its name and an associated value.
    /// </summary>
    public static readonly KarmaServiceType Momentum = new(Name: "Momentum", Value: 1);
}
