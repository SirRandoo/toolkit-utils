using System;
using System.Diagnostics;
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

        public static void Error(string message, Exception exception)
        {
            Verse.Log.Error($"{message}: {exception.GetType().Name}({exception.Message})\n{exception.StackTrace}");
        }

        public static void Info(string message)
        {
            Log("INFO", message);
        }

        private static void Log(string level, string message, string color = null)
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
