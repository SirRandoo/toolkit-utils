// ToolkitUtils
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics.CodeAnalysis;
using TwitchToolkit;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Utils;

public abstract class IncidentVariablesBase : IncidentHelperVariables
{
    public override Viewer Viewer { get; set; }

    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false) => CanHappen(message, viewer);

    public override void TryExecute()
    {
        Execute();
    }

    public abstract bool CanHappen(string msg, Viewer viewer);
    public abstract void Execute();
}
