using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<KeyValuePair<string, string>> ParseKeyed(IEnumerable<string> input)
        {
            var cache = new List<KeyValuePair<string, string>>();

            foreach (var segment in input)
            {
                if (!segment.Contains('='))
                {
                    continue;
                }

                var key = "";
                var value = "";
                var sep = false;
                var escaped = false;

                foreach (var c in segment)
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
