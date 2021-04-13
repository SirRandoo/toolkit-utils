﻿// ToolkitUtils
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
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Utilities;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ArgWorker
    {
        private readonly Queue<string> rawArguments;
        private string lastArgument;

        private ArgWorker([NotNull] IEnumerable<string> rawArguments)
        {
            this.rawArguments = new Queue<string>(rawArguments.Select(a => a.ToToolkit()));
        }

        [NotNull]
        public static ArgWorker CreateInstance([NotNull] params string[] rawArguments)
        {
            return new ArgWorker(rawArguments);
        }

        [NotNull]
        public static ArgWorker CreateInstance([NotNull] IEnumerable<string> rawArguments)
        {
            return new ArgWorker(rawArguments);
        }

        [NotNull]
        public static ArgWorker CreateInstance([NotNull] string input)
        {
            return new ArgWorker(CommandFilter.Parse(input));
        }

        public string GetNext()
        {
            if (!rawArguments.TryDequeue(out string next))
            {
                return null;
            }

            lastArgument = next;
            return next;
        }

        public string GetLast()
        {
            return lastArgument;
        }

        public bool HasNext()
        {
            return rawArguments.Count > 0;
        }

        public int GetNextAsInt(int minimum = 0, int maximum = int.MaxValue)
        {
            string next = GetNext();

            if (next == null || !int.TryParse(next, out int value))
            {
                return 0;
            }

            return Math.Max(minimum, Math.Min(value, maximum));
        }

        public bool TryGetNextAsInt(out int value, int minimum = 0, int maximum = int.MaxValue)
        {
            string next = GetNext();

            if (next != null && int.TryParse(next, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }

        [CanBeNull]
        public TraitItem GetNextAsTrait()
        {
            string next = GetNext();

            if (next == null || !Data.TryGetTrait(next, out TraitItem trait))
            {
                return null;
            }

            return trait;
        }

        [CanBeNull]
        public TraitItem GetNextAsTrait(Action<string> errorCallback)
        {
            TraitItem trait = GetNextAsTrait();

            if (trait == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return trait;
        }

        public bool TryGetNextAsTrait([CanBeNull] out TraitItem trait)
        {
            trait = GetNextAsTrait();
            return !(trait is null);
        }

        [CanBeNull]
        public PawnKindItem GetNextAsPawn()
        {
            string next = GetNext();

            if (next == null || !Data.TryGetPawnKind(next, out PawnKindItem pawn))
            {
                return null;
            }

            return pawn;
        }

        [CanBeNull]
        public PawnKindItem GetNextAsPawn(Action<string> errorCallback)
        {
            PawnKindItem pawn = GetNextAsPawn();

            if (pawn == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return pawn;
        }

        public bool TryGetNextAsPawn([CanBeNull] out PawnKindItem pawn)
        {
            pawn = GetNextAsPawn();
            return !(pawn is null);
        }

        [CanBeNull]
        public Command GetNextAsCommand()
        {
            string next = GetNext();

            if (next == null)
            {
                return null;
            }

            return DefDatabase<Command>.AllDefs.FirstOrDefault(
                c => TkSettings.ToolkitStyleCommands
                    ? c.command.StartsWith(next, true, CultureInfo.InvariantCulture)
                    : c.command.Equals(next, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        [CanBeNull]
        public Command GetNextAsCommand(Action<string> errorCallback)
        {
            Command command = GetNextAsCommand();

            if (command == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return command;
        }

        public bool TryGetNextAsCommand([CanBeNull] out Command command)
        {
            command = GetNextAsCommand();
            return !(command is null);
        }

        [CanBeNull]
        public SkillDef GetNextAsSkill()
        {
            string next = GetNext();

            if (next == null)
            {
                return null;
            }

            return DefDatabase<SkillDef>.AllDefs.FirstOrDefault(
                s => (s.label?.ToToolkit().Equals(next) ?? false)
                     || (s.skillLabel?.ToToolkit().Equals(next, StringComparison.InvariantCultureIgnoreCase) ?? false)
                     || s.defName.Equals(next, StringComparison.InvariantCulture)
            );
        }

        [CanBeNull]
        public SkillDef GetNextAsSkill(Action<string> errorCallback)
        {
            SkillDef skill = GetNextAsSkill();

            if (skill == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return skill;
        }

        public bool TryGetNextAsSkill([CanBeNull] out SkillDef def)
        {
            def = GetNextAsSkill();
            return !(def is null);
        }

        private ThingItem GetItemRaw(string input)
        {
            return Data.Items.Find(
                i => i.DefName.Equals(input) || i.Name.Equals(input, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        [CanBeNull]
        public ItemProxy GetNextAsItem()
        {
            string next = GetNext();

            if (next == null)
            {
                return null;
            }

            var proxy = new ItemProxy();

            if (next.Contains("[") && next.Contains("]"))
            {
                string details = next.Substring(next.IndexOf('[') + 1).TrimEnd(']');
                string item = next.Replace($"[{details}]", "");
                proxy.Thing = GetItemRaw(item);

                foreach (string segment in details.Split(','))
                {
                    if (Data.Qualities.TryGetValue(segment, out QualityCategory quality))
                    {
                        proxy.Quality = quality;
                        continue;
                    }

                    proxy.Stuff = GetItemRaw(segment);
                }

                return proxy;
            }

            proxy.Thing = GetItemRaw(next);
            return proxy;
        }

        [CanBeNull]
        public ItemProxy GetNextAsItem(Action<string> errorCallback)
        {
            ItemProxy item = GetNextAsItem();

            if (item == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return item;
        }

        public bool TryGetNextAsItem([CanBeNull] out ItemProxy item)
        {
            item = GetNextAsItem();
            return !(item is null);
        }

        [CanBeNull]
        public PawnCapacityDef GetNextAsCapacity()
        {
            string next = GetNext();

            if (next == null)
            {
                return null;
            }

            return DefDatabase<PawnCapacityDef>.AllDefs.FirstOrDefault(
                c => c.defName.Equals(next)
                     || c.label.ToToolkit().Equals(next, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        [CanBeNull]
        public PawnCapacityDef GetNextAsCapacity(Action<string> errorCallback)
        {
            PawnCapacityDef capacityDef = GetNextAsCapacity();

            if (capacityDef == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return capacityDef;
        }

        public bool TryGetNextAsCapacity([CanBeNull] out PawnCapacityDef capacity)
        {
            capacity = GetNextAsCapacity();
            return !(capacity is null);
        }

        [CanBeNull]
        public StatDef GetNextAsStat()
        {
            string next = GetNext();

            if (next == null)
            {
                return null;
            }

            return DefDatabase<StatDef>.AllDefs.Where(s => s.showOnHumanlikes && s.showOnPawns)
               .FirstOrDefault(
                    s => s.label.ToToolkit().Equals(next, StringComparison.InvariantCultureIgnoreCase)
                         || s.defName.Equals(next, StringComparison.InvariantCulture)
                );
        }

        [CanBeNull]
        public StatDef GetNextAsStat(Action<string> errorCallback)
        {
            StatDef stat = GetNextAsStat();

            if (stat == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return stat;
        }

        public bool TryGetNextAsStat([CanBeNull] out StatDef stat)
        {
            stat = GetNextAsStat();
            return !(stat is null);
        }

        [CanBeNull]
        public ResearchProjectDef GetNextAsResearch()
        {
            string next = GetNext();

            if (next == null)
            {
                return null;
            }

            return DefDatabase<ResearchProjectDef>.AllDefs.FirstOrDefault(
                p => p.label.ToToolkit().Equals(next, StringComparison.InvariantCultureIgnoreCase)
                     || p.defName.Equals(next, StringComparison.InvariantCulture)
            );
        }

        [CanBeNull]
        public ResearchProjectDef GetNextAsResearch(Action<string> errorCallback)
        {
            ResearchProjectDef proj = GetNextAsResearch();

            if (proj == null)
            {
                errorCallback.Invoke(lastArgument);
            }

            return proj;
        }

        public bool TryGetNextAsResearch([CanBeNull] out ResearchProjectDef project)
        {
            project = GetNextAsResearch();
            return !(project is null);
        }

        public IEnumerable<TraitItem> GetAllAsTrait()
        {
            while (HasNext())
            {
                TraitItem trait = GetNextAsTrait();

                if (trait == null)
                {
                    break;
                }

                yield return trait;
            }
        }

        public IEnumerable<TraitItem> GetAllAsTrait(Action<string> errorCallback)
        {
            while (HasNext())
            {
                TraitItem trait = GetNextAsTrait();

                if (trait == null)
                {
                    errorCallback.Invoke(lastArgument);
                    break;
                }

                yield return trait;
            }
        }

        public IEnumerable<ItemProxy> GetAllAsItem()
        {
            while (HasNext())
            {
                ItemProxy item = GetNextAsItem();

                if (item == null)
                {
                    break;
                }

                yield return item;
            }
        }

        public IEnumerable<ItemProxy> GetAllAsItem(Action<string> errorCallback)
        {
            while (HasNext())
            {
                ItemProxy item = GetNextAsItem();

                if (item == null)
                {
                    errorCallback.Invoke(lastArgument);
                    break;
                }

                yield return item;
            }
        }

        public class ItemProxy
        {
            public ThingItem Thing { get; set; }
            public ThingItem Stuff { get; set; }
            public QualityCategory? Quality { get; set; }

            public bool IsValid()
            {
                return Thing != null && Stuff != null && Quality != null;
            }
        }
    }
}