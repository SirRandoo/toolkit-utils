// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows;

public class IncidentCategoryWindow : CategoricalEditorWindow<StoreIncident>
{
    /// <inheritdoc/>
    public IncidentCategoryWindow() : base("TKUtils.Headers.AllIncidents".TranslateSimple())
    {
        Store_IncidentEditor.UpdatePriceSheet();
    }

    /// <inheritdoc/>
    protected override bool VisibleInSearch(StoreIncident entry)
    {
        if (entry.abbreviation.IndexOf(SearchWidget.filter.Text, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        return entry.label.IndexOf(SearchWidget.filter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <inheritdoc/>
    protected override bool IsEntryDisabled(StoreIncident entry) => entry.cost < 1 && entry != StoreIncidentDefOf.Item;

    /// <inheritdoc/>
    protected override void OpenEditorFor(StoreIncident entry)
    {
        Find.WindowStack.Add(new StoreIncidentEditor(entry));
    }

    /// <inheritdoc/>
    protected override void ResetEntry(StoreIncident entry)
    {
        Store_IncidentEditor.LoadBackup(entry);

        if (entry.cost < 1)
        {
            entry.cost = 50;
        }

        Store_IncidentEditor.SaveCopy(entry);
    }

    /// <inheritdoc/>
    protected override void DisableEntry(StoreIncident entry)
    {
        entry.cost = -10;

        Store_IncidentEditor.SaveCopy(entry);
    }

    /// <inheritdoc/>
    public override void PostClose()
    {
        Store_IncidentEditor.UpdatePriceSheet();
        base.PostClose();
    }
}