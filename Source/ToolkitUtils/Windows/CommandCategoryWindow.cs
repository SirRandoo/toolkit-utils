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
using TwitchToolkit.Commands;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class CommandCategoryWindow : CategoricalEditorWindow<Command>
    {
        /// <inheritdoc/>
        public CommandCategoryWindow() : base("All Commands")
        {
        }

        /// <inheritdoc/>
        protected override bool VisibleInSearch([NotNull] Command entry)
        {
            if (StringComparer.OrdinalIgnoreCase.Compare(entry.command, SearchWidget.filter.Text) >= 0)
            {
                return true;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(entry.defName, SearchWidget.filter.Text) >= 0;
        }

        /// <inheritdoc/>
        protected override bool IsEntryDisabled([NotNull] Command entry) => !entry.enabled;

        /// <inheritdoc/>
        protected override void OpenEditorFor([NotNull] Command entry)
        {
            Find.WindowStack.Add(new CommandEditorDialog(entry));
        }

        /// <inheritdoc/>
        protected override void DisableEntry([NotNull] Command entry)
        {
            entry.enabled = false;
        }

        /// <inheritdoc/>
        protected override void ResetAllEntries()
        {
            CommandEditor.LoadBackups();
        }
    }
}
