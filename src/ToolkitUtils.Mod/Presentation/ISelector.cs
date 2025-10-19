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
namespace ToolkitUtils.Mod.Presentation;

/// <summary>Defines a mechanism for filtering entries within a table using a specified logic.</summary>
public interface ISelector : IIdentifiable
{
    /// <summary>Filters the specified table entry based on custom criteria.</summary>
    /// <param name="entry">The table entry to be evaluated for filtering.</param>
    /// <returns>True if the entry matches the filter criteria; otherwise, false.</returns>
    bool Filter(IIdentifiable entry);
}
