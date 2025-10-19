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
using JetBrains.Annotations;
using Verse;
using Mod = Verse.Mod;

namespace ToolkitUtils.Api;

/// <summary>Represents a utility modification class that extends the functionality of the base mod class.</summary>
/// <remarks>
///     The <see cref="UtilsMod" /> class is designed to be a singleton, meaning only one instance of this class
///     should exist throughout the application's lifecycle. It inherits from the base <see cref="Mod" /> class and
///     provides additional functionalities specific to utility modifications. The class uses the
///     <see cref="ModContentPack" /> to access and manage mod-related content.
/// </remarks>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class UtilsMod : Verse.Mod
{
    /// <inheritdoc />
    public UtilsMod(ModContentPack content) : base(content)
    {
        Instance = this;
    }

    /// <summary>Gets the singleton instance of the <see cref="UtilsMod" /> class.</summary>
    /// <remarks>
    ///     This property holds the single instance of the <see cref="UtilsMod" /> created during the instantiation of the
    ///     mod. It allows access to the mod's functionalities and resources throughout the application's lifecycle. This
    ///     instance is initialized to the current <see cref="UtilsMod" /> object in its constructor and should be null before
    ///     any instance is created.
    /// </remarks>
    [PublicAPI]
    public static UtilsMod Instance { get; private set; } = null!;
}
