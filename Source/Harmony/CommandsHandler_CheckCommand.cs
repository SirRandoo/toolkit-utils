using System;
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
            try
            {
                if(!TKSettings.Commands || msg == null || msg.Message == null)
                {
                    return true;
                }

                var viewer = Viewers.GetViewer(msg.User);
                viewer.last_seen = DateTime.Now;

                if(viewer.IsBanned)
                {
                    return false;
                }

                var message = msg.Message;
                var segments = CommandParser.Parse(message, TKSettings.Prefix);
                var unemoji = segments.Any(i => i.EqualsIgnoreCase("--text"));
                var emojiCache = TKSettings.Emojis;

                if(message.Substring(0, TKSettings.Prefix.Length).EqualsIgnoreCase(TKSettings.Prefix))
                {
                    if(!segments.Any())
                    {
                        return false;
                    }

                    if(segments.First().StartsWith("/w"))
                    {
                        segments = segments.Skip(1).ToArray();
                    }

                    if(unemoji)
                    {
                        segments = segments
                            .Where(i => !i.EqualsIgnoreCase("--text"))
                            .ToArray();
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
                                Message = ($"!{command.command}" + " " + string.Join(" ", segments.Skip(1))).Trim(),
                                Parameters = msg.Parameters,
                                User = msg.User,
                                Whisper = msg.Whisper
                            };

                            if(command.commandDriver.Name.Equals("Buy") && !command.command.EqualsIgnoreCase("buy"))
                            {
                                payload.Message = ($"!buy {command.command}" + " " + string.Join(" ", segments.Skip(1))).Trim();
                            }

                            CommandBase.Log($"Falsified viewer's command from \"{msg.Message}\" to \"{payload.Message}\"");

                            if(unemoji)
                            {
                                TKSettings.Emojis = false;
                            }

                            try
                            {
                                command.RunCommand(payload);
                            }
                            catch(Exception e)
                            {
                                CommandBase.Error($"Command \"{command.command}\" failed with exception: {e.Message}\n{e.StackTrace}");
                            }

                            if(unemoji)
                            {
                                TKSettings.Emojis = emojiCache;
                            }
                        }
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
                        Message = ($"!" + segments.Take(1).FirstOrDefault().ToLowerInvariant() + " " + string.Join(" ", segments.Skip(1))).Trim(),
                        Parameters = msg.Parameters,
                        User = msg.User,
                        Whisper = msg.Whisper
                    };

                    CommandBase.Log($"Falsified viewer's command from \"{msg.Message}\" to \"{payload.Message}\"  -- Hard-coded commands check");

                    if(unemoji)
                    {
                        TKSettings.Emojis = false;
                    }

                    foreach(var tInterface in twitchInterfaces)
                    {
                        try
                        {
                            tInterface.ParseCommand(payload);
                        }
                        catch(Exception e)
                        {
                            CommandBase.Error($"Twitch interface \"{tInterface.GetType().FullName}\" failed with exception: {e.Message}\n{e.StackTrace}");
                        }
                    }

                    if(unemoji)
                    {
                        TKSettings.Emojis = emojiCache;
                    }
                }
            }
            catch(Exception e)
            {
                CommandBase.Error($"Command parser failed with exception: {e.Message}\n{e.StackTrace}");
            }

            return false;
        }
    }
}
