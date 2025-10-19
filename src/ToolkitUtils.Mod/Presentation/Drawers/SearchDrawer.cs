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
using System.Collections.Generic;
using ToolkitUtils.Mod.Presentation.Dialogs;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>
///     Represents a UI component for search functionality, providing mechanisms to render a searchable input field
///     and handle associated actions.
/// </summary>
/// <typeparam name="T">The type of elements to be searched.</typeparam>
public sealed class SearchDrawer<T>
{
    public delegate IReadOnlyList<string> SearchAction(string query);

    private string _currentQuery = string.Empty;
    private DropdownDialog<T>? _dialog = null;

    private SearchDrawer() {}

    /// <summary>Draws the search drawer within the specified region.</summary>
    /// <param name="region">The region where the search drawer will be rendered.</param>
    public void Draw(Rect region) {}

    /// <summary>Creates a new instance of the <see cref="SearchDrawer{T}" /> class.</summary>
    /// <returns>A new instance of <see cref="SearchDrawer{T}" /> initialized with default settings.</returns>
    public static SearchDrawer<string> CreateInstance()
    {
        var controlId = $"SK_SearchBox_{SearchDrawerData.ControlId.ToString()}";
        SearchDrawerData.ControlId++;

        return new SearchDrawer<string>();
    }
}

internal static class SearchDrawerData
{
    internal static int ControlId;
}

/// <summary>An abstraction around a IMGUI text field.</summary>
public sealed class TextField
{
    private int _controlId;
    private string _controlName = null!;
    private TextEditor _editor = null!;
    private Rect _region;

    /// <summary>The raw content of the text field.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the current cursor position within the text content of the field.</summary>
    public int CursorPosition
    {
        get => _editor.cursorIndex;
        set => _editor.cursorIndex = value;
    }

    public IntegerRange SelectionRange
    {
        get
        {
            if (_editor.cursorIndex == _editor.selectIndex) return IntegerRange.Zero;

            return _editor.cursorIndex < _editor.selectIndex
                ? new IntegerRange(_editor.cursorIndex, _editor.selectIndex - _editor.cursorIndex)
                : new IntegerRange(_editor.selectIndex, _editor.cursorIndex - _editor.selectIndex);
        }
    }

    /// <summary>Initialises the text field at the given region.</summary>
    /// <param name="region">The region to initialise the text field in.</param>
    public void Initialise(Rect region)
    {
        _region = region;
        _controlName = $"SK_SearchField_{region.ToString()}";
        _controlId = GUIUtility.GetControlID(FocusType.Keyboard, region);
        _editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), _controlId);
    }

    /// <summary>Draws the text field on screen.</summary>
    public void Draw()
    {
        GUI.SetNextControlName(_controlName);
        Content = GUI.TextField(_region, Content, Text.CurTextFieldStyle);
    }
}
