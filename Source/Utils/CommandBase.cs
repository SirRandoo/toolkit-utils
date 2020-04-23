using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CommandBase : CommandMethod
    {
        private const int MessageLimit = 500;

        public CommandBase(ToolkitChatCommand command) : base(command)
        {
        }

        private static Pawn FindPawn(string username)
        {
            return Find.ColonistBar.Entries.Where(c => ((NameTriple) c.pawn.Name).Nick.EqualsIgnoreCase(username))
                .Select(c => c.pawn)
                .FirstOrDefault();
        }

        public static Pawn GetOrFindPawn(string username, bool allowKidnapped = false)
        {
            var safe = GetPawn(username);

            if (safe.IsKidnapped() && !allowKidnapped)
            {
                return null;
            }

            if (safe != null)
            {
                return safe;
            }

            var result = FindPawn(username);

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
            var query = component.pawnHistory.Keys
                .Where(k => k.EqualsIgnoreCase(username))
                .Select(p => component.pawnHistory[p]);

            return query.FirstOrDefault();
        }

        internal static void SendMessage(string message)
        {
            if (message.NullOrEmpty())
            {
                return;
            }

            var words = message.Split(new[] {' '}, StringSplitOptions.None);
            var builder = new StringBuilder();
            var messages = new List<string>();
            var chars = 0;

            foreach (var word in words)
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

            foreach (var m in messages)
            {
                TwitchWrapper.SendChatMessage(m.Trim());
            }
        }
    }
}
