using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class CommandParser
    {
        public static string[] Parse(string input, string prefix = "!")
        {
            if (input.StartsWith(prefix))
            {
                input = input.Substring(prefix.Length);
            }

            var cache = new List<string>();
            var segment = "";
            var quoted = false;
            var escaped = false;

            foreach (var c in input)
            {
                if (escaped && !c.Equals('"'))
                {
                    escaped = false;
                    segment += '\\';
                }

                switch (c)
                {
                    case ' ' when !quoted:
                        cache.Add(segment);
                        segment = "";
                        break;
                    case '"' when !escaped:
                        quoted = !quoted;
                        break;
                    case '"':
                        segment += c;
                        escaped = false;
                        break;
                    case '\\':
                        escaped = true;
                        break;
                    default:
                        segment += c;
                        break;
                }
            }

            if (segment.Length > 0)
            {
                cache.Add(segment);
            }

            return cache.ToArray();
        }

        public static List<KeyValuePair<string, string>> ParseKeyed(string input, string prefix = "!")
        {
            return ParseKeyed(Parse(input, prefix));
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
                                key += c;
                            }
                            else
                            {
                                value += c;
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
