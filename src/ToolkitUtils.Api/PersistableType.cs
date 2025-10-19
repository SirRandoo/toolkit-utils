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
namespace ToolkitUtils.Api;

/// <summary>An enumeration representing the different types of persistable information supported by the mod.</summary>
public enum PersistableType
{
    /// <summary>The persistable type is unknown, or unsupported by the mod.</summary>
    Unknown,

    /// <summary>
    ///     The persistable type represents a data type. Data types are classes that will be loaded and saved into the
    ///     <see cref="FilePaths.DataRoot" /> directory.
    /// </summary>
    Data,

    /// <summary>
    ///     The persistable type represents user configurable settings. Settings are classes that will be loaded and saved
    ///     into the <see cref="FilePaths.SettingsRoot" /> directory.
    /// </summary>
    Settings,
}
