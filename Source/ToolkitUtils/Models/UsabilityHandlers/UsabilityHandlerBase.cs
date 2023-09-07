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

using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Models.UsabilityHandlers;

public abstract record UsabilityHandlerBase<T>(string ModId = "sirrandoo.tku") : IUsabilityHandler where T : ThingComp
{
    protected UsabilityHandlerBase(params Type[] excluded) : this()
    {
        ExcludedTypes = new HashSet<Type>(excluded);
    }

    protected HashSet<Type> ExcludedTypes { get; } = null!;

    public virtual bool IsUsable(ThingDef thing) => thing.HasAssignableCompFrom(typeof(T));

    public virtual void Use(Pawn pawn, ThingDef thingDef)
    {
        Thing t = ThingMaker.MakeThing(thingDef);

        if (t is not ThingWithComps withComps)
        {
            throw new InvalidOperationException($"""The thing "{thingDef.defName}" doesn't have any comps.""");
        }

        T? comp = withComps.GetComps<T>().FirstOrDefault(c => !ExcludedTypes.Any(i => i.IsInstanceOfType(c)));

        string? failReason = null;

        if (comp is null || !IsUsable(comp, pawn, thingDef, out failReason))
        {
            throw new OperationCanceledException($"""The thing "{thingDef.defName}" could not be used by {pawn.LabelShort}. Fail reason: {failReason}""");
        }

        try
        {
            GenSpawn.Spawn(t, pawn.Position, pawn.Map);
            Use(comp, pawn, t);

            if (t.Spawned)
            {
                t.DeSpawn();
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"""The thing "{thingDef.defName}" could not be used by {pawn.LabelShort}.""", e);
        }
    }

    public virtual string ModId { get; init; } = ModId;

    protected abstract bool IsUsable(T comp, Pawn pawn, ThingDef thing, out string failReason);
    protected abstract void Use(T comp, Pawn pawn, Thing thing);
}
