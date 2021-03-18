﻿// ToolkitUtils
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
using System.Linq;
using JetBrains.Annotations;
using ToolkitCore.Utilities;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class CommandParser
    {
        public static List<KeyValuePair<string, string>> ParseKeyed(string input)
        {
            return ParseKeyed(CommandFilter.Parse(input));
        }

        [NotNull]
        public static List<KeyValuePair<string, string>> ParseKeyed([NotNull] IEnumerable<string> input)
        {
            var cache = new List<KeyValuePair<string, string>>();

            foreach (string segment in input)
            {
                if (!segment.Contains('='))
                {
                    continue;
                }

                var key = "";
                var value = "";
                var sep = false;
                var escaped = false;

                foreach (char c in segment)
                {
                    switch (c)
                    {
                        case '=' when !escaped:
                            sep = true;
                            break;
                        case '\\':
                            escaped = true;
                            break;
                        default:
                        {
                            if (!sep)
                            {
                                key += c.ToString();
                            }
                            else
                            {
                                value += c.ToString();
                            }

                            break;
                        }
                    }
                }

                cache.Add(new KeyValuePair<string, string>(key, value));
            }

            return cache;
        }

        public static bool TryParseBool(string input, bool defaultValue = true)
        {
            if (input.EqualsIgnoreCase("yes")
                || input.EqualsIgnoreCase("true")
                || input.EqualsIgnoreCase("y")
                || input.Equals("1")
                || input.Equals("+"))
            {
                return true;
            }

            if (input.EqualsIgnoreCase("no")
                || input.EqualsIgnoreCase("false")
                || input.EqualsIgnoreCase("n")
                || input.Equals("0")
                || input.Equals("-"))
            {
                return false;
            }

            if (input.Equals("👍")
                || input.Equals("✔️")
                || input.Equals("☑️")
                || input.Equals("✅")
                || input.Equals("🆗"))
            {
                return true;
            }

            if (input.Equals("👎")
                || input.Equals("🛑")
                || input.Equals("🚫")
                || input.Equals("⛔")
                || input.Equals("⏹️")
                || input.Equals("⏏️")
                || input.Equals("❌")
                || input.Equals("❎"))
            {
                return false;
            }

            if (input.Equals("🎲"))
            {
                return new Random().Next(1) == 1;
            }

            return defaultValue;
        }
    }
}
