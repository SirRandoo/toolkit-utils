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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public abstract class UsabilityHandlerBase<T> : IUsabilityHandler where T : ThingComp
    {
        public UsabilityHandlerBase([NotNull] params Type[] excluded)
        {
            ExcludedTypes = new HashSet<Type>(excluded);
        }

        protected HashSet<Type> ExcludedTypes { get; }

        public abstract string Id { get; }

        public virtual bool IsUsable([NotNull] ThingDef thing)
        {
            return thing.HasAssignableCompFrom(typeof(T));
        }

        public virtual void Use([NotNull] Pawn pawn, ThingDef thing)
        {
            Thing t = ThingMaker.MakeThing(thing);

            if (!(t is ThingWithComps withComps))
            {
                throw new InvalidOperationException($@"The thing ""{thing.defName}"" doesn't have any comps.");
            }

            T comp = withComps.GetComps<T>().FirstOrDefault(c => !ExcludedTypes.Any(i => i.IsInstanceOfType(c)));

            string failReason = null;

            if (comp == null || !IsUsable(comp, pawn, thing, out failReason))
            {
                throw new OperationCanceledException(
                    $@"The thing ""{thing.defName}"" could not be used by {pawn.LabelShort}. Fail reason: {failReason}"
                );
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
                throw new InvalidOperationException(
                    $@"The thing ""{thing.defName}"" could not be used by {pawn.LabelShort}.",
                    e
                );
            }
        }

        protected abstract bool IsUsable(T comp, Pawn pawn, ThingDef thing, out string failReason);
        protected abstract void Use(T comp, Pawn pawn, Thing thing);
    }
}
