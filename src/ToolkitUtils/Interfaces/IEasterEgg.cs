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

using TwitchToolkit;
using TwitchToolkit.Incidents;
using Verse;

namespace ToolkitUtils.Interfaces
{
    public interface IEasterEgg
    {
        float Chance { get; }
        string UserId { get; }
        bool IsPossible(StoreIncident incident, Viewer viewer);
        void Execute(Viewer viewer);
        void Execute(Viewer viewer, Pawn pawn);
    }
}
