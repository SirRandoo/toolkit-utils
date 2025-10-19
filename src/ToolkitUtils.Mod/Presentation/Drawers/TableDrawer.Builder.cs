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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitUtils.Mod.Presentation.Drawers;

public abstract partial class TableDrawer<T>
{
    /// <summary>
    ///     Provides a mechanism to construct and configure instances of <see cref="TableDrawer{T}" /> with specific
    ///     columns and dataset.
    /// </summary>
    /// <typeparam name="TBase">
    ///     The specific implementation of the <see cref="TableDrawer{T}" /> class being constructed. Must
    ///     be a subtype of the <see cref="TableDrawer{T}" /> where T represents the data type.
    /// </typeparam>
    /// <remarks>
    ///     A builder pattern implementation enabling gradual, customizable setup of table columns and content before
    ///     final instantiation.
    /// </remarks>
    /// <example>
    ///     Use the Builder to define table columns and set the dataset before invoking <c>Build</c> to generate the table
    ///     drawer instance.
    /// </example>
    public sealed class Builder<TBase>(Func<TBase> instantiator) where TBase : TableDrawer<T>
    {
        private readonly List<TableColumn> _columns = [];

        /// <summary>Adds a new column to the table configuration with specified options.</summary>
        /// <param name="columnOptionsAction">
        ///     A delegate used to configure the column options, such as header text, width, icon, or
        ///     sorting behavior.
        /// </param>
        /// <returns>The current builder instance to allow for method chaining.</returns>
        public Builder<TBase> WithColumn(Action<TableColumnOptions> columnOptionsAction)
        {
            var options = new TableColumnOptions();

            // User configure
            columnOptionsAction(options);

            _columns.Add(new TableColumn(options.Header, options.RelativeWidth, options.Icon, options.SortAction));

            return this;
        }

        /// <summary>Creates and initializes an instance of the table drawer with the configured columns and dataset.</summary>
        /// <param name="dataSet">A collection of data items that will populate the table.</param>
        /// <returns>The configured instance of the table drawer.</returns>
        public TBase Build(IEnumerable<T> dataSet)
        {
            TBase instance = instantiator();
            instance._columns = _columns;
            instance._columnRegions = new Rect[_columns.Count];
            instance._contents = dataSet.Select(i => new TableEntry(i)).ToList();
            instance._viewportHeight = instance._contents.Count * UiConstants.LineHeight;

            return instance;
        }
    }

    /// <summary>Represents the configuration options for defining a column in a table.</summary>
    /// <remarks>
    ///     Allows customization of column header, width, text positioning, associated icon, and sorting behavior for a
    ///     table. Used during table construction to specify the visual and functional characteristics of individual columns.
    /// </remarks>
    public class TableColumnOptions
    {
        /// <summary>Represents an icon associated with a column in the table configuration.</summary>
        /// <remarks>
        ///     This property is used to define a graphical icon (as a <see cref="Texture2D" /> object) that can be displayed
        ///     in the header of the column. The icon can help better represent the meaning or function of the column to the user.
        /// </remarks>
        public Texture2D? Icon { get; set; }

        /// <summary>
        ///     A delegate defining the action to be performed for sorting a column. The action receives the specified
        ///     <see cref="SortOrder" /> and the associated collection of <see cref="TableEntry" /> items.
        /// </summary>
        public Action<SortOrder, IReadOnlyList<TableEntry>>? SortAction { get; set; }

        /// <summary>Represents the text or title displayed at the top of a table column.</summary>
        public string? Header { get; set; }

        /// <summary>
        ///     Represents the proportional width of a table column relative to other columns. This value determines how much
        ///     space the column occupies in comparison to others.
        /// </summary>
        public float RelativeWidth { get; set; }

        /// <summary>Gets or sets the alignment of the text within a column in the table.</summary>
        public TextAnchor TextAnchor { get; set; }
    }

    /// <summary>
    ///     Represents a column in a table, defining its appearance and behavior, such as header, width, sorting, and
    ///     optional icons.
    /// </summary>
    /// <remarks>
    ///     A column in the context of <see cref="TableDrawer{T}" /> is used to visually render and configure individual
    ///     table elements. It includes functionality for sorting and icon representation alongside its display properties.
    /// </remarks>
    private sealed class TableColumn(string? header, float relativeWidth, Texture2D? icon = null, Action<SortOrder, IReadOnlyList<TableEntry>>? sortAction = null)
    {
        /// <summary>Represents the sort direction applied to a table column.</summary>
        /// <remarks>
        ///     This property determines whether the column's data is sorted in ascending, descending, or no particular order.
        ///     It also interacts with provided sort logic to adjust the display and order of data in the table dynamically based
        ///     on user interactions.
        /// </remarks>
        public SortOrder SortOrder { get; set; }

        /// <summary>Represents the textual content displayed in the header of a table column.</summary>
        /// <remarks>
        ///     This property is used to define a meaningful label or title for the column in a tabular interface. It helps
        ///     users understand the type of data or functionality associated with the column.
        /// </remarks>
        public string? Header { get; } = header;

        /// <summary>Defines the proportional width of a column relative to the total table width.</summary>
        /// <remarks>
        ///     This property determines how much horizontal space a column occupies in relation to other columns, expressed
        ///     as a ratio. The sum of all column widths is normalized to the total available width of the table.
        /// </remarks>
        public float RelativeWidth { get; } = relativeWidth;

        /// <summary>Represents an optional graphical icon associated with a table column.</summary>
        /// <remarks>
        ///     This property defines a visual element, represented as a <see cref="Texture2D" />, that can be displayed
        ///     alongside the column header. It enhances the UI by providing a clear representation or visual cue relevant to the
        ///     column's content or functionality. When populated, the icon is rendered within the respective column's header.
        /// </remarks>
        public Texture2D? Icon { get; } = icon;

        /// <summary>Defines the action to be executed when sorting is triggered for a column.</summary>
        /// <remarks>
        ///     This property represents a callback function that is invoked whenever the column is sorted. The action accepts
        ///     the current <see cref="SortOrder" /> and a collection of <see cref="TableEntry" /> objects to perform the sort
        ///     processing. If undefined, the column is treated as unsortable.
        /// </remarks>
        public Action<SortOrder, IReadOnlyList<TableEntry>>? SortAction { get; } = sortAction;
    }

    /// <summary>Represents a single entry in a table used by <see cref="TableDrawer{T}" />.</summary>
    /// <remarks>
    ///     Provides mechanisms for storing and controlling the visibility of a specific data record within a table.
    ///     Primarily utilized internally by <see cref="TableDrawer{T}" /> to manage individual rows or items.
    /// </remarks>
    public sealed class TableEntry(T data)
    {
        /// <summary>Represents the data item associated with a row in the table.</summary>
        /// <remarks>
        ///     This property stores an instance of the data type associated with the table's rows. It is used by various
        ///     table operations such as filtering, searching, and rendering to manipulate or display the content of the rows in
        ///     the table. The type of the data is defined by the generic parameter <typeparamref name="T" /> of the table.
        /// </remarks>
        public T Data { get; } = data;

        /// <summary>Indicates whether the associated table entry is visible in the table rendering process.</summary>
        /// <remarks>
        ///     This property determines the visibility of a table entry in scenarios such as searching, filtering, or
        ///     dynamically managing the display of data. Modifying this value directly impacts whether the entry contributes to
        ///     layout and resource consumption during rendering.
        /// </remarks>
        public bool Visible { get; set; } = true;
    }
}
