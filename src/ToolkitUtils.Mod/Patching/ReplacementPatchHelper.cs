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
using System;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Patching;

/// <summary>
///     A helper class that assists in replacing invocations with other invocations from within a Harmony transpiler
///     patch.
/// </summary>
[PublicAPI]
public static class ReplacementPatchHelper
{
    /// <summary>Determines whether a given <see cref="CodeInstruction" /> represents the invocation of a specific constructor.</summary>
    /// <param name="instruction">The instruction to be checked.</param>
    /// <param name="originalConstructor">
    ///     The constructor to compare against. The method evaluates whether this constructor is
    ///     the target of the provided instruction.
    /// </param>
    /// <returns>
    ///     A boolean value indicating whether the <paramref name="instruction" /> represents an invocation of the
    ///     <paramref name="originalConstructor" />.
    /// </returns>
    public static bool IsConstructorInvocation(CodeInstruction instruction, MethodBase originalConstructor) =>
        instruction.opcode == OpCodes.Newobj && ReferenceEquals(instruction.operand, originalConstructor);

    /// <summary>
    ///     Determines whether a given <see cref="CodeInstruction" /> represents the loading of a type token for a
    ///     specific type.
    /// </summary>
    /// <param name="instruction">The instruction to be checked.</param>
    /// <param name="originalType">
    ///     The type to compare against. The method evaluates whether this type is the target of the
    ///     provided instruction.
    /// </param>
    /// <returns>
    ///     A boolean value indicating whether the <paramref name="instruction" /> represents the loading of a type token
    ///     for the <paramref name="originalType" />.
    /// </returns>
    public static bool IsTypeTokenLoading(CodeInstruction instruction, Type originalType) =>
        instruction.opcode == OpCodes.Ldtoken && ReferenceEquals(instruction.operand, originalType);
}
