using System;
using System.Linq;

using HarmonyLib;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;
using Command = TwitchToolkit.Command;

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
                if (!TkSettings.Commands)
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
                var segments = CommandParser.Parse(message, TkSettings.Prefix);
                var unemoji = segments.Any(i => i.EqualsIgnoreCase("--text"));
                var emojiCache = TkSettings.Emojis;

                IRCMessage payload;
                if (message.Substring(0, TkSettings.Prefix.Length).EqualsIgnoreCase(TkSettings.Prefix))
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

                    var commands = DefDatabase<Command>.AllDefsListForReading.ToList();
                    Command command = null;

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
                            payload = new IRCMessage
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

                            if(command.commandDriver.Name.Equals("Buy") && !command.defName.EqualsIgnoreCase("buy"))
                            {
                                payload.Message = ($"!buy {command.command}" + " " + string.Join(" ", segments.Skip(1))).Trim();
                            }

                            Logger.Info($"Falsified viewer's command from \"{msg.Message}\" to \"{payload.Message}\"");

                            if(unemoji)
                            {
                                TkSettings.Emojis = false;
                            }

                            try
                            {
                                command.RunCommand(payload);
                            }
                            catch(Exception e)
                            {
                                Logger.Error($"Command \"{command.command}\" failed", e);
                            }

                            if(unemoji)
                            {
                                TkSettings.Emojis = emojiCache;
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

                    Logger.Info($"Falsified viewer's command from \"{msg.Message}\" to \"{payload.Message}\"  -- Hard-coded commands check");

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
                            Logger.Error($"Twitch interface \"{tInterface.GetType().FullName}\" failed", e);
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
                Logger.Error($"Command parser failed", e);
            }

            return false;
        }
    }
}
