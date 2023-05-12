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

using System;
using System.Collections.Generic;

namespace ToolkitUtils.Workers
{
    /// <summary>
    ///     A base class for drawing store data in a portable way.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TableWorker<T> : TableWorkerBase
    {
        private protected List<T> InternalData;

        /// <summary>
        ///     The internal store data the worker is drawing.
        /// </summary>
        public IEnumerable<T> Data => InternalData;

        /// <summary>
        ///     Called to ensure the given data exists within the worker's
        ///     internal datastore.
        /// </summary>
        /// <param name="data">The data to verify</param>
        public abstract void EnsureExists(T data);

        /// <summary>
        ///     Informs the worker that the global store data changed externally.
        /// </summary>
        public abstract void NotifyGlobalDataChanged();

        /// <summary>
        ///     Informs the worker to filter its contents according to the
        ///     callable passed.
        /// </summary>
        /// <param name="worker">
        ///     A callable whose sole argument is the item to
        ///     filter, with its return value being whether the item should be
        ///     displayed
        /// </param>
        public abstract void NotifyCustomSearchRequested(Func<T, bool> worker);

        protected enum StateKey { Enable, Disable }

        protected enum SettingsKey { Expand, Collapse }
    }
}
