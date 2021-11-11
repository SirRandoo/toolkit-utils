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
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnInteraction : CommandBase
    {
        private static readonly Dictionary<string, InteractionProxy> InteractionIndex;

        static PawnInteraction()
        {
            InteractionIndex = new Dictionary<string, InteractionProxy>
            {
                { "Insult", new InteractionProxy { Interaction = InteractionDefOf.Insult, IsBad = true } },
                { "Chat", new InteractionProxy { Interaction = InteractionDefOf.Chitchat } },
                { "Flirt", new InteractionProxy { Interaction = InteractionDefOf.RomanceAttempt } },
                { "DeepChat", new InteractionProxy { Interaction = InteractionDefOf.DeepTalk } }
            };
        }

        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            if (!InteractionIndex.TryGetValue(command.defName, out InteractionProxy interaction))
            {
                LogHelper.Warn($@"{command.label}({command.defName}) is bound to the {nameof(PawnInteraction)} class, but has no interaction registered.");
                return;
            }

            Viewer data = Viewers.GetViewer(msg.Username);

            if (!PurchaseHelper.TryGetPawn(msg.Username, out Pawn pawn))
            {
                msg.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            string query = CommandFilter.Parse(msg.Message).Skip(1).FirstOrFallback("");
            Pawn target = null;

            if (!query.NullOrEmpty())
            {
                if (query.StartsWith("@"))
                {
                    query = query.Substring(1);
                }

                Viewer viewer = Viewers.All.FirstOrDefault(v => v.username.EqualsIgnoreCase(query));

                if (viewer == null)
                {
                    return;
                }

                target = GetOrFindPawn(viewer.username);

                if (target == null)
                {
                    msg.Reply("TKUtils.PawnNotFound".LocalizeKeyed(query));
                    return;
                }
            }

            target ??= Find.ColonistBar.Entries.Where(p => p.pawn != pawn).RandomElement().pawn;

            CommandRouter.MainThreadCommands.Enqueue(
                () =>
                {
                    string result = ForcedInteractionWorker.InteractWith(pawn, target, interaction.Interaction);

                    if (interaction.IsBad)
                    {
                        data.SetViewerKarma(Mathf.Max(data.karma - (int)Mathf.Ceil(data.karma * 0.1f), ToolkitSettings.KarmaMinimum));
                    }

                    msg.Reply(result);
                }
            );
        }

        private class InteractionProxy
        {
            public bool IsBad { get; set; }
            public InteractionDef Interaction { get; set; }
        }
    }
}
