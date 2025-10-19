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
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>A collection of extension for asynchronously modifying the letter stack within the game in a safe way.</summary>
[PublicAPI]
public static class LetterStackExtensions
{
    /// <summary>Removes a letter from the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="letter">A <see cref="Letter" /> instance to remove from the stack.</param>
    public static async Task RemoveLetterAsync(this LetterStack stack, Letter letter)
    {
        await MainThreadExtensions.OnMainAsync(stack.RemoveLetter, letter);
    }

    /// <summary>Adds a letter to the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="letter">A <see cref="Letter" /> instance to add to the stack.</param>
    public static async Task ReceiveLetterAsync(this LetterStack stack, Letter letter)
    {
        await MainThreadExtensions.OnMainAsync(stack.ReceiveLetter, letter, (string?)null, arg3: 0, arg4: true);
    }

    /// <summary>Adds a letter to the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="label">The label of the letter.</param>
    /// <param name="text">The body of the letter.</param>
    /// <param name="def">A <see cref="LetterDef" /> indicating the type of letter being received.</param>
    public static async Task ReceiveLetterAsync(this LetterStack stack, TaggedString label, TaggedString text, LetterDef def)
    {
        await MainThreadExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, (string?)null, arg5: 0, arg6: true);
    }

    /// <summary>Adds a letter to the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="label">The label of the letter.</param>
    /// <param name="text">The body of the letter.</param>
    /// <param name="def">A <see cref="LetterDef" /> indicating the type of letter being received.</param>
    /// <param name="targets">
    ///     One or more objects the letter is referencing. When provided, the game will display arrows
    ///     pointing to the objects on the map the objects are on.
    /// </param>
    public static async Task ReceiveLetterAsync(this LetterStack stack, TaggedString label, TaggedString text, LetterDef def, LookTargets targets)
    {
        await MainThreadExtensions.OnMainAsync(
            stack.ReceiveLetter,
            label,
            text,
            def,
            targets,
            (Faction?)null,
            (Quest?)null,
            (List<ThingDef>?)null,
            (string?)null,
            arg9: 0,
            arg10: true
        );
    }

    /// <summary>Adds a letter to the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="label">The label of the letter.</param>
    /// <param name="text">The body of the letter.</param>
    /// <param name="def">A <see cref="LetterDef" /> indicating the type of letter being received.</param>
    /// <param name="targets">
    ///     One or more objects the letter is referencing. When provided, the game will display arrows
    ///     pointing to the objects on the map the objects are on.
    /// </param>
    /// <param name="faction">The faction to display in the letter.</param>
    public static async Task ReceiveLetterAsync(this LetterStack stack, TaggedString label, TaggedString text, LetterDef def, LookTargets targets, Faction faction)
    {
        await MainThreadExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, targets, faction, (Quest?)null, (List<ThingDef>?)null, (string?)null, arg9: 0, arg10: true);
    }

    /// <summary>Adds a letter to the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="label">The label of the letter.</param>
    /// <param name="text">The body of the letter.</param>
    /// <param name="def">A <see cref="LetterDef" /> indicating the type of letter being received.</param>
    /// <param name="targets">
    ///     One or more objects the letter is referencing. When provided, the game will display arrows
    ///     pointing to the objects on the map the objects are on.
    /// </param>
    /// <param name="faction">The faction to display in the letter.</param>
    /// <param name="quest">The quest the letter is referencing.</param>
    public static async Task ReceiveLetterAsync(this LetterStack stack, TaggedString label, TaggedString text, LetterDef def, LookTargets targets, Faction faction, Quest quest)
    {
        await MainThreadExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, targets, faction, quest, (List<ThingDef>?)null, (string?)null, arg9: 0, arg10: true);
    }

    /// <summary>Adds a letter to the letter stack.</summary>
    /// <param name="stack">A <see cref="LetterStack" /> housing the letters for the current game.</param>
    /// <param name="label">The label of the letter.</param>
    /// <param name="text">The body of the letter.</param>
    /// <param name="def">A <see cref="LetterDef" /> indicating the type of letter being received.</param>
    /// <param name="targets">
    ///     One or more objects the letter is referencing. When provided, the game will display arrows
    ///     pointing to the objects on the map the objects are on.
    /// </param>
    /// <param name="faction">The faction to display in the letter.</param>
    /// <param name="quest">The quest the letter is referencing.</param>
    /// <param name="thingDefs">
    ///     A collection of <see cref="ThingDef" />s that'll be displayed in the body of the letter as
    ///     "hyperlinks". Hyperlinks are a way for users to view information about the things referenced through an
    ///     informational dialog.
    /// </param>
    public static async Task ReceiveLetterAsync(
        this LetterStack stack,
        TaggedString label,
        TaggedString text,
        LetterDef def,
        LookTargets targets,
        Faction faction,
        Quest quest,
        List<ThingDef> thingDefs
    )
    {
        await MainThreadExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, targets, faction, quest, thingDefs, (string?)null, arg9: 0, arg10: true);
    }
}
