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

/// <summary>Defines behaviors for modifying an identifiable target object.</summary>
public interface IMutator : IIdentifiable
{
    /// <summary>Applies a mutation to the specified target and returns the result of the operation.</summary>
    /// <param name="target">The target object to which the mutation will be applied.</param>
    /// <returns>A <see cref="Result" /> indicating the success or failure of the operation.</returns>
    Result Mutate(IIdentifiable target);
}
