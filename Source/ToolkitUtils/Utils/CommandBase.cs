// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CommandBase : CommandDriver
    {
        [CanBeNull]
        private static Pawn FindPawn(string username)
        {
            return Find.ColonistBar.GetColonistsInOrder()
               .Where(p => p.Faction == Faction.OfPlayer)
               .FirstOrDefault(c => ((NameTriple)c.Name)?.Nick.EqualsIgnoreCase(username) ?? false);
        }

        [CanBeNull]
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

            var component = Current.Game.GetComponent<GameComponentPawns>();

            component.pawnHistory[username] = result;
            component.viewerNameQueue.Remove(username);

            return result;
        }

        [CanBeNull]
        public static Pawn GetPawn(string username)
        {
            var component = Current.Game.GetComponent<GameComponentPawns>();
            IEnumerable<Pawn> query = component.pawnHistory.Keys.Where(k => k.EqualsIgnoreCase(username))
               .Select(p => component.pawnHistory[p]);

            return query.FirstOrDefault();
        }
    }
}
