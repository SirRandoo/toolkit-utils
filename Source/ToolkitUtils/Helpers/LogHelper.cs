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
using System.Diagnostics;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class LogHelper
    {
        private const string WarnColor = "#ff6b00";

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            Log("DEBUG", message);
        }

        public static void Error(string message, [NotNull] Exception exception)
        {
            Verse.Log.Error($"{message}: {exception.GetType().Name}({exception.Message})\n{exception.StackTrace}");
        }

        public static void Info(string message)
        {
            Log("INFO", message);
        }

        private static void Log([NotNull] string level, string message, [CanBeNull] string color = null)
        {
            Verse.Log.Message(
                color.NullOrEmpty()
                    ? $"{level.ToUpper()} {TkUtils.Id} :: {message}"
                    : $"<color=\"{color}\">{level.ToUpper()} {TkUtils.Id} :: {message}</color>"
            );
        }

        public static void Warn(string message)
        {
            Log("WARN", message, WarnColor);
        }
    }
}
