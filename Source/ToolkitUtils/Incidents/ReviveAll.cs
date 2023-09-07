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
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents;

[UsedImplicitly]
public class ReviveAll : IncidentHelper
{
    private List<Pawn> _pawns;

    public override bool IsPossible()
    {
        List<Pawn> list = Find.ColonistBar.GetColonistsInOrder().Where(p => p.Dead && p.SpawnedOrAnyParentSpawned && !PawnTracker.pawnsToRevive.Contains(p)).ToList();

        if (!list.Any())
        {
            return false;
        }

        _pawns = list;

        foreach (Pawn p in list)
        {
            PawnTracker.pawnsToRevive.Add(p);
        }

        return true;
    }

    public override void TryExecute()
    {
        var any = false;

        foreach (Pawn pawn in _pawns)
        {
            if (!pawn.TryResurrect())
            {
                continue;
            }

            any = true;
        }

        if (!any)
        {
            return;
        }

        Find.LetterStack.ReceiveLetter("TKUtils.MassRevivalLetter.Title".Localize(), "TKUtils.MassRevivalLetter.Description".Localize(), LetterDefOf.PositiveEvent);
    }
}