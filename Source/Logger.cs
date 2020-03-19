using System;

using Verse;

namespace SirRandoo.ToolkitUtils
{
    public static class Logger
    {
        public static void Debug(string message)
        {
            if(Prefs.DevMode)
            {
                Log("DEBUG", message);
            }
        }

        public static void Error(string message, Exception exception)
        {
            Verse.Log.Error($"{message}: {exception.GetType().Name}({exception.Message})\n{exception.StackTrace}");
        }

        public static void Info(string message)
        {
            Log("INFO", message);
        }

        public static void Log(string level, string message, string color = null)
        {
            if(color.NullOrEmpty())
            {
                Verse.Log.Message($"{level.ToUpper()} {TKUtils.ID} :: {message}");
            }
            else
            {
                Verse.Log.Message($"<color=\"{color}\">{level.ToUpper()} {TKUtils.ID} :: {message}</color>");
            }
        }

        public static void Warn(string message)
        {
            Log("WARN", message, color: "#ff8080");
        }
    }
}
