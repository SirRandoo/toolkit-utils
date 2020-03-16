using System.Collections.Generic;
using System.Linq;
using System.Text;

using ToolkitCore;

using TwitchLib.Client.Models;

using TwitchToolkit;
using TwitchToolkit.PawnQueue;

using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CommandBase : CommandDriver
    {
        private const int MESSAGE_LIMIT = 500;

        public static void Error(string message)
        {
            Verse.Log.Error($"ERROR {TKUtils.ID} :: {message}");
        }

        public static Pawn FindPawn(string username)
        {
            return Find.ColonistBar.Entries
                .Where(c => ((NameTriple) c.pawn.Name).Nick.EqualsIgnoreCase(username))
                .Select(c => c.pawn)
                .FirstOrDefault();
        }

        public static Pawn GetPawn(string username)
        {
            var component = Current.Game.GetComponent<GameComponentPawns>();
            var query = component.pawnHistory.Keys
                .Where(k => k.EqualsIgnoreCase(username))
                .Select(p => component.pawnHistory[p]);

            return query.Any() ? query.First() : null;
        }

        public static Pawn GetPawnDestructive(string username)
        {
            var safe = GetPawn(username);

            if(safe != null)
            {
                return safe;
            }

            var destructive = FindPawn(username);

            if(destructive != null)
            {
                Warn($"Viewer \"{username}\" was unlinked from their pawn!  Reassigning...");

                var component = Current.Game.GetComponent<GameComponentPawns>();

                component.pawnHistory[username] = destructive;
                component.viewerNameQueue.Remove(username);
            }

            return destructive;
        }

        public static string GetTranslatedEmoji(string emoji, string text = null)
        {
            if(text == null)
            {
                text = $"{emoji}.Text";
            }

            if(TKSettings.Emojis)
            {
                return emoji;
            }

            return text;
        }

        public static void Log(string message) => Verse.Log.Message($"{TKUtils.ID} :: {message}");

        public static void SendCommandMessage(string viewer, string message)
        {
            SendMessage(
                "TKUtils.Formats.CommandBase".Translate(
                    viewer.Named("VIEWER"),
                    message.Named("MESSAGE")
                )
            );
        }

        public static void SendCommandMessage(string message, ChatMessage ircMessage) => SendCommandMessage(ircMessage.Username, message);

        public static void SendMessage(string message)
        {
            if(message.NullOrEmpty())
            {
                return;
            }

            var words = message.Split(new char[] { ' ' }, System.StringSplitOptions.None);
            var builder = new StringBuilder();
            var messages = new List<string>();
            var chars = 0;

            foreach(var word in words)
            {
                if(chars + word.Length <= MESSAGE_LIMIT - 3)
                {
                    builder.Append($"{word} ");
                    chars += word.Length + 1;
                }
                else
                {
                    builder.Append("...");
                    messages.Add(builder.ToString());
                    builder.Clear();
                    chars = 0;
                }
            }

            if(builder.Length > 0)
            {
                messages.Add(builder.ToString());
                builder.Clear();
            }

            if(messages.Count > 0)
            {
                foreach(var m in messages)
                {
                    MessageQueue.messageQueue.Enqueue(m.Trim());
                }
            }
        }

<<<<<<< HEAD
        public static void SendMessage(string message, ChatMessage ircMessage) => SendMessage(message);
=======
        public static void SendMessage(string message, IRCMessage ircMessage)
        {
            SendMessage(message, CommandsHandler.SendToChatroom(ircMessage));
        }
>>>>>>> cd62ab800fd9bb2a1046201238cc2082fe54899b

        public static void Warn(string message)
        {
            Verse.Log.Message($"WARN {TKUtils.ID} :: {message}");
        }
    }
}
