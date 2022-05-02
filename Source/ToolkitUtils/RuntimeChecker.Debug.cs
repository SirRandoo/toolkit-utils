// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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

#if DEBUG
using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils
{
    internal partial class RuntimeChecker
    {
        internal static void Execute(string command, [NotNull] Action func)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            func();

            stopwatch.Stop();
            TkUtils.Logger.Debug($"Command {command} finished in {stopwatch.ElapsedMilliseconds:0.000}ms");
        }
    }
}

#endif
