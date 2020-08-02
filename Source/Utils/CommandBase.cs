using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using ToolkitCore;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CommandBase : CommandDriver
    {
        private const int MessageLimit = 500;

        private static Pawn FindPawn(string username)
        {
            return Find.ColonistBar.Entries.Where(c => c.pawn != null)
               .Select(c => c.pawn)
               .FirstOrDefault(c => ((NameTriple) c.Name)?.Nick.EqualsIgnoreCase(username) ?? false);
        }

        public static Pawn GetOrFindPawn(string username, bool allowKidnapped = false)
        {
            Pawn safe = GetPawn(username);

            if (safe.IsKidnapped() && !allowKidnapped)
            {
                return null;
            }

            if (safe != null)
            {
                return safe;
            }

            Pawn result = FindPawn(username);

            if (result == null)
            {
                return null;
            }

            TkLogger.Warn($"Viewer \"{username}\" was unlinked from their pawn!  Reassigning...");

            var component = Current.Game.GetComponent<GameComponentPawns>();

            component.pawnHistory[username] = result;
            component.viewerNameQueue.Remove(username);

            return result;
        }

        public static Pawn GetPawn(string username)
        {
            var component = Current.Game.GetComponent<GameComponentPawns>();
            IEnumerable<Pawn> query = component.pawnHistory.Keys.Where(k => k.EqualsIgnoreCase(username))
               .Select(p => component.pawnHistory[p]);

            return query.FirstOrDefault();
        }

        internal static void SendMessage(string message)
        {
            if (message.NullOrEmpty())
            {
                return;
            }

            string[] words = message.Split(new[] {' '}, StringSplitOptions.None);
            var builder = new StringBuilder();
            var messages = new List<string>();
            var chars = 0;

            foreach (string word in words)
            {
                if (chars + word.Length <= MessageLimit - 3)
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

            if (builder.Length > 0)
            {
                messages.Add(builder.ToString());
                builder.Clear();
            }

            if (messages.Count <= 0)
            {
                return;
            }

            foreach (string m in messages)
            {
                TwitchWrapper.SendChatMessage(m.Trim());
            }
        }
    }
}
