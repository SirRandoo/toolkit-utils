using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnClassCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".TranslateSimple());
                return;
            }

            MagicComp.CharacterData data = MagicComp.GetCharacterData(pawn);

            if (data == null || !data.Gifted || data.Class.NullOrEmpty())
            {
                twitchMessage.Reply("TKUtils.Responses.PawnClass.None".TranslateSimple());
                return;
            }

            var container = new List<string>
            {
                ResponseHelper.JoinPair("TKUtils.Misc.Level".TranslateSimple(), data.Level.ToString("N0")),
                ResponseHelper.JoinPair("TKUtils.Misc.XP".TranslateSimple(), data.Experience)
            };

            string key;
            switch (data.Type)
            {
                case MagicComp.ClassTypes.Might:
                    key = "TKUtils.Misc.Stamina";
                    break;
                case MagicComp.ClassTypes.Magic:
                    key = "TKUtils.Misc.Mana";
                    break;
                default:
                    key = null;
                    break;
            }

            if (!key.NullOrEmpty())
            {
                string t = $"{data.ResourceCurrent:N0} / {data.ResourceMax:N0} (";

                if (data.ResourceRegenRate > 0)
                {
                    t += "+";
                }

                string rateKey = key?.Substring(key.LastIndexOf(".", StringComparison.Ordinal) + 1, 1) ?? "?";

                t += $"{data.ResourceRegenRate:N0} {rateKey}P/5s";
                t += ")";

                container.Add(
                    ResponseHelper.JoinPair(key.TranslateSimple(), t)
                );
            }

            if (data.Points > 0)
            {
                container.Add("TKUtils.Formats.PawnClass.Points".Translate(data.Points.ToString("N0")));
            }

            twitchMessage.Reply(container.GroupedJoin().WithHeader(Unrichify.StripTags(data.Class)));
        }
    }
}
