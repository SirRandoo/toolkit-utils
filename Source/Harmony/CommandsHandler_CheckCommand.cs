using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using TwitchToolkit;
using Verse;
using Viewers = TwitchToolkit.Viewers;

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
                if (!TkSettings.Commands)
                {
                    return true;
                }

                if (msg?.Message == null)
                {
                    return false;
                }

                var viewer = Viewers.GetViewer(msg.Username);
                viewer.last_seen = DateTime.Now;

                if (viewer.IsBanned)
                {
                    return false;
                }

                var message = msg.Message;
                var segments = CommandParser.Parse(message, TkSettings.Prefix);
                var unemoji = segments.Any(i => i.EqualsIgnoreCase("--text"));
                var emojiCache = TkSettings.Emojis;

                if (message.Substring(0, TkSettings.Prefix.Length).EqualsIgnoreCase(TkSettings.Prefix))
                {
                    if (!segments.Any())
                    {
                        return false;
                    }

                    if (segments.First().StartsWith("/w"))
                    {
                        segments = segments.Skip(1).ToArray();
                    }

                    if (unemoji)
                    {
                        segments = segments
                            .Where(i => !i.EqualsIgnoreCase("--text"))
                            .ToArray();
                    }

                    var commands = DefDatabase<ToolkitChatCommand>.AllDefsListForReading.ToList();
                    ToolkitChatCommand command = null;

                    foreach (var def in commands)
                    {
                        if (def.commandText.Contains(" "))
                        {
                            var spaces = def.commandText.Count(c => c.Equals(' '));
                            var joined = string.Join(" ", segments.Take(spaces));

                            if (!def.commandText.EqualsIgnoreCase(joined))
                            {
                                continue;
                            }

                            command = def;
                            break;
                        }

                        if (!def.commandText.EqualsIgnoreCase(segments.Take(1).First()))
                        {
                            continue;
                        }

                        command = def;
                        break;
                    }

                    if (command != null)
                    {
                        var instance = (CommandMethod) Activator.CreateInstance(command.commandClass, (object) command);
                        var chatMessage = msg;

                        if (command.commandClass.FullName.EqualsIgnoreCase("TwitchToolkit.Commands.ViewerCommands.Buy"))
                        {
                            chatMessage = new ChatMessage(
                                msg.BotUsername,
                                msg.UserId,
                                msg.Username,
                                msg.DisplayName,
                                msg.ColorHex,
                                msg.Color,
                                msg.EmoteSet,
                                $"!{command.commandText} {CombineSegments(segments.Skip(1))}".Trim(),
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

                            Logger.Info(
                                $"Falsified viewer's command from \"{msg.Message}\" to \"{chatMessage.Message}\""
                            );
                        }

                        var payload = new ChatCommand(
                            chatMessage,
                            command.commandText,
                            CombineSegments(segments.Skip(1)),
                            segments.ToList(),
                            '!'
                        );

                        if (instance.CanExecute(payload))
                        {
                            if (unemoji)
                            {
                                TkSettings.Emojis = false;
                            }

                            try
                            {
                                instance.Execute(payload);
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"Command \"{command.commandText}\" failed", e);
                            }

                            if (unemoji)
                            {
                                TkSettings.Emojis = emojiCache;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Command parser failed", e);
            }

            return false;
        }

        private static string CombineSegments(IEnumerable<string> segments)
        {
            return string.Join(
                " ",
                segments.Select(segment => segment.Contains(' ') ? $"\"{segment.Replace("\"", "\\\"")}\"" : segment)
                    .ToArray()
            );
        }
    }
}
