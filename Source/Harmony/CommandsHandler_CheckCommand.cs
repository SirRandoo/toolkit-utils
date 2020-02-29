using System.Linq;

using HarmonyLib;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(CommandsHandler), "CheckCommand")]
    public static class CommandsHandler_CheckCommand
    {
        [HarmonyPrefix]
        public static bool CheckCommand(IRCMessage msg)
        {
            if(!TKSettings.Commands || msg == null || msg.Message == null)
            {
                return true;
            }

            var message = msg.Message;

            if(message.Length < TKSettings.Prefix.Length)
            {
                return false;
            }

            var prefixTemp = message.Substring(0, TKSettings.Prefix.Length);

            if(!prefixTemp.EqualsIgnoreCase(TKSettings.Prefix))
            {
                return false;
            }

            var viewer = Viewers.GetViewer(msg.User);

            message = message.Substring(prefixTemp.Length);
            var segments = CommandParser.Parse(message);

            if(!segments.Any())
            {
                return false;
            }

            if(segments.First().StartsWith("/w"))
            {
                segments = segments.Skip(1).ToArray();
            }

            var commands = DefDatabase<TwitchToolkit.Command>.AllDefsListForReading.ToList();
            TwitchToolkit.Command command = null;

            foreach(var def in commands)
            {
                if(def.command.Contains(" "))
                {
                    var spaces = def.command.Count(c => c.Equals(" "));
                    var joined = string.Join(" ", segments.Take(spaces));

                    if(def.command.EqualsIgnoreCase(joined))
                    {
                        command = def;
                        break;
                    }
                }
                else
                {
                    if(def.command.EqualsIgnoreCase(segments.Take(1).First()))
                    {
                        command = def;
                        break;
                    }
                }
            }

            if(command != null)
            {
                var runnable = true;

                if(!command.enabled)
                {
                    runnable = false;
                }

                if(command.requiresMod && !viewer.mod && !viewer.username.EqualsIgnoreCase(ToolkitSettings.Channel))
                {
                    runnable = false;
                }

                if(command.requiresAdmin && !msg.User.EqualsIgnoreCase(ToolkitSettings.Channel))
                {
                    runnable = false;
                }

                if(command.shouldBeInSeparateRoom && !CommandsHandler.AllowCommand(msg))
                {
                    runnable = false;
                }

                if(runnable)
                {
                    // Fix up the message for Toolkit
                    var payload = new IRCMessage
                    {
                        Args = msg.Args,
                        Channel = msg.Channel,
                        Cmd = msg.Cmd,
                        Host = msg.Host,
                        Message = $"!{command.command}" + " " + string.Join(" ", segments.Skip(1)),
                        Parameters = msg.Parameters,
                        User = msg.User,
                        Whisper = msg.Whisper
                    };

                    Log($"Falsified viewer's command from \"{msg.Message}\" to \"{payload.Message}\"");
                    command.RunCommand(payload);
                }
            }

            var twitchInterfaces = Current.Game.components.OfType<TwitchInterfaceBase>().ToList();

            if(twitchInterfaces != null)
            {
                // hard-coded pawn commands....
                var payload = new IRCMessage
                {
                    Args = msg.Args,
                    Channel = msg.Channel,
                    Cmd = msg.Cmd,
                    Host = msg.Host,
                    Message = $"!" + segments.Take(1).FirstOrDefault().ToLowerInvariant() + " " + string.Join(" ", segments.Skip(1)),
                    Parameters = msg.Parameters,
                    User = msg.User,
                    Whisper = msg.Whisper
                };

                Log($"Falsified viewer's command from \"{msg.Message}\" to \"{payload.Message}\"  -- Hard-coded commands");

                foreach(var tInterface in twitchInterfaces)
                {
                    tInterface.ParseCommand(payload);
                }
            }

            return false;
        }

        public static void Log(string message) => Verse.Log.Message($"{TKUtils.ID} :: {message}");
    }
}
