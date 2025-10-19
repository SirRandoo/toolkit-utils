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
// with ToolkitUtils.Plugins.TwitchToolkit. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using ToolkitUtils.Mod.Data;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Plugins.TwitchToolkit.Presentation;

/// <summary>
///     Represents a dialog window for displaying Twitch viewers, utilizing the design conventions defined by
///     ToolkitUtils.
/// </summary>
public sealed class ViewersDialog : Window
{
    private IReadOnlyList<Viewer> _viewers = [];

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        // TODO: Implement the viewers dialog's UI.
    }
}
