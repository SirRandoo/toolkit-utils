using System;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class CommandHelper
    {
        public static void Execute(this Command command, ITwitchMessage message, bool emojiOverride = false)
        {
            if (command.requiresAdmin && !message.ChatMessage.IsBroadcaster)
            {
                return;
            }

            if (command.requiresMod && (!message.ChatMessage.IsModerator && !message.ChatMessage.IsBroadcaster))
            {
                return;
            }

            RuntimeChecker.ExecuteInMainThread(
                $"{command.command}[{message.Message}]",
                delegate
                {
                    bool emojis = TkSettings.Emojis;

                    try
                    {
                        if (emojiOverride)
                        {
                            TkSettings.Emojis = false;
                        }

                        command.RunCommand(message);
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error($@"Command ""{command.command}"" threw an exception!", e);
                    }
                    finally
                    {
                        TkSettings.Emojis = emojis;
                    }
                }
            );
        }

        internal static string ValidatePrefix(string prefix)
        {
            if (prefix.StartsWith("/") || prefix.StartsWith("."))
            {
                prefix = prefix.Substring(1);
            }

            return prefix.Replace(" ", "");
        }
    }
}
