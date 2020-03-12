using System;
using System.Linq;

using HarmonyLib;

using rim_twitch;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(CommandsHandler), "CheckCommand")]
    public static class CommandsHandler_CheckCommand
    {
        [HarmonyPrefix]
        public static bool CheckCommand(ChatMessage msg)
        {
            try
            {
                if(!TKSettings.Commands || msg == null || msg.Message == null)
                {
                    return true;
                }

                var viewer = Viewers.GetViewer(msg.Username);
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

                        if(command.requiresAdmin && !msg.Username.EqualsIgnoreCase(ToolkitSettings.Channel))
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
                            var t = ($"!{command.command}" + " " + string.Join(" ", segments.Skip(1))).Trim();

                            if(command.commandDriver.Name.Equals("Buy") && !command.command.EqualsIgnoreCase("buy"))
                            {
                                t = ($"!buy {command.command}" + " " + string.Join(" ", segments.Skip(1))).Trim();
                            }

                            var payload = new ChatMessage(
                                msg.BotUsername,
                                msg.UserId,
                                msg.Username,
                                msg.DisplayName,
                                msg.ColorHex,
                                msg.Color,
                                msg.EmoteSet,
                                ($"!{command.command}" + " " + string.Join(" ", segments.Skip(1))).Trim(),
                                msg.UserType,
                                msg.Channel,
                                msg.Id,
                                msg.IsSubscriber,
                                msg.SubscribedMonthCount,
                                msg.RoomId,
                                msg.IsTurbo,
                                msg.IsModerator,
                                msg.IsMe,
                                msg.IsBroadcaster,
                                msg.Noisy,
                                msg.RawIrcMessage,
                                msg.EmoteReplacedMessage,
                                msg.Badges,
                                msg.CheerBadge,
                                msg.Bits,
                                msg.BitsInDollars
                            );

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
                    var payload = new ChatMessage(
                        msg.BotUsername,
                        msg.UserId,
                        msg.Username,
                        msg.DisplayName,
                        msg.ColorHex,
                        msg.Color,
                        msg.EmoteSet,
                        ($"!" + segments.Take(1).FirstOrDefault().ToLowerInvariant() + " " + string.Join(" ", segments.Skip(1))).Trim(),
                        msg.UserType,
                        msg.Channel,
                        msg.Id,
                        msg.IsSubscriber,
                        msg.SubscribedMonthCount,
                        msg.RoomId,
                        msg.IsTurbo,
                        msg.IsModerator,
                        msg.IsMe,
                        msg.IsBroadcaster,
                        msg.Noisy,
                        msg.RawIrcMessage,
                        msg.EmoteReplacedMessage,
                        msg.Badges,
                        msg.CheerBadge,
                        msg.Bits,
                        msg.BitsInDollars
                    );

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
