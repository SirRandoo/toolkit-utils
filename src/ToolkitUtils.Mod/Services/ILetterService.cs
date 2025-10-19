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
using System.Threading.Tasks;
using Verse;

namespace ToolkitUtils.Mod.Services;

/// <summary>Provides functionality for sending various types of letters in the context of the application.</summary>
public interface ILetterService
{
    /// <summary>Sends a letter asynchronously with the specified title, body, letter type, and recipient.</summary>
    /// <param name="title">The title of the letter.</param>
    /// <param name="body">The content of the letter.</param>
    /// <param name="letterDef">The type of letter being sent.</param>
    /// <param name="recipient">The recipient of the letter.</param>
    /// <returns>A task that represents the asynchronous operation of sending the letter.</returns>
    Task SendLetterAsync(string title, string body, LetterDef letterDef, LookTargets recipient);

    /// Sends a neutral letter with a specified title and body to a recipient.
    /// <param name="title">The title of the letter.</param>
    /// <param name="body">The body content of the letter.</param>
    /// <param name="recipient">The recipient of the letter.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendNeutralLetterAsync(string title, string body, LookTargets recipient);

    /// <summary>Sends a positive letter with the specified title and body text to the provided recipient.</summary>
    /// <param name="title">The title of the positive letter.</param>
    /// <param name="body">The body content of the positive letter.</param>
    /// <param name="recipient">The target entity that will receive the letter.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendPositiveLetterAsync(string title, string body, LookTargets recipient);

    /// <summary>Sends a negative letter with the specified title and body to the recipient.</summary>
    /// <param name="title">The title of the letter.</param>
    /// <param name="body">The body content of the letter.</param>
    /// <param name="recipient">The recipient of the letter.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendNegativeLetterAsync(string title, string body, LookTargets recipient);

    /// <summary>
    ///     Sends a threatening letter with the specified title and body to the given recipient. Optionally marks the
    ///     letter as a big threat.
    /// </summary>
    /// <param name="title">The title of the threatening letter.</param>
    /// <param name="body">The body content of the threatening letter.</param>
    /// <param name="recipient">The recipient of the threatening letter.</param>
    /// <param name="bigThreat">Indicates whether the letter represents a significant threat.</param>
    /// <returns>A task that represents the asynchronous operation of sending the threatening letter.</returns>
    Task SendThreateningLetterAsync(string title, string body, LookTargets recipient, bool bigThreat);
}
