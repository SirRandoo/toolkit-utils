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

/// <summary>Represents an</summary>
public interface IEasterEgg
{
    /// <summary>The chance the viewers within <see cref="UserIds" /> have to get this easter egg.</summary>
    float Chance { get; }

    /// <summary>The ids of the user that can obtain this easter egg.</summary>
    string[] UserIds { get; }
}
