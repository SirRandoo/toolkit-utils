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
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Dialogs;

/// <summary>
///     Represents a modal dialog window used to configure or manage research overrides within the context of the
///     application.
/// </summary>
/// <remarks>
///     This dialog provides user interaction related to research modifications and is implemented as a discrete
///     instance of the Unity-based UI. It inherits from the <see cref="Window" /> class, enabling rendering of custom
///     contents and behavior within the game's user interface.
/// </remarks>
public sealed class ResearchOverrideDialog : Window
{
    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        throw new NotImplementedException();
    }
}
