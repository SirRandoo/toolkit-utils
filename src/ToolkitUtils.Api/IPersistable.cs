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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System.Threading.Tasks;

namespace ToolkitUtils.Api;

/// <summary>An interface defining a class capable of IO operations.</summary>
public interface IPersistable
{
    /// <summary>Saves the class's data to disk.</summary>
    /// <param name="path">The path to the file to save the data in.</param>
    void Save(string path);

    /// <summary>Saves the class's data to disk.</summary>
    /// <param name="path">THe path to the file to save the data in.</param>
    Task SaveAsync(string path);
}
