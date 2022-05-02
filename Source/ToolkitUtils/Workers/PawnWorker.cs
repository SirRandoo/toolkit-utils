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

using SirRandoo.ToolkitUtils.Models;

namespace SirRandoo.ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for drawing an editor page for pawn kinds in a portable
    ///     way.
    /// </summary>
    public class PawnWorker : ItemWorkerBase<TableWorker<TableSettingsItem<PawnKindItem>>, PawnKindItem>
    {
        /// <inheritdoc cref="ItemWorkerBase{T,TU}.Prepare"/>
        public override void Prepare()
        {
            base.Prepare();
            Worker = new PawnTableWorker();
            Worker.Prepare();

            DiscoverMutators(DomainIndexer.EditorTarget.Any);
            DiscoverSelectors(DomainIndexer.EditorTarget.Any);
            DiscoverMutators(DomainIndexer.EditorTarget.Pawn);
            DiscoverSelectors(DomainIndexer.EditorTarget.Pawn);
        }
    }
}
