using System;
using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnClassCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            MagicComp.CharacterData data = MagicComp.GetCharacterData(pawn);

            if (data == null || !data.Gifted || data.Class.NullOrEmpty())
            {
                twitchMessage.Reply("TKUtils.Responses.PawnClass.None".Translate());
                return;
            }

            var container = new List<string>
            {
                "TKUtils.Formats.KeyValue".Translate(
                    "TKUtils.Misc.Level".Translate(),
                    data.Level.ToString("N0")
                ),
                "TKUtils.Formats.KeyValue".Translate(
                    "TKUtils.Misc.XP".Translate(),
                    data.Experience
                )
            };


            string key = null;

            if (data.Type == MagicComp.ClassTypes.Might)
            {
                key = "TKUtils.Misc.Stamina";
            }
            else if (data.Type == MagicComp.ClassTypes.Magic)
            {
                key = "TKUtils.Misc.Mana";
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
                    "TKUtils.Formats.KeyValue".Translate(key.Translate(), t)
                );
            }

            if (data.Points > 0)
            {
                container.Add("TKUtils.Formats.PawnClass.Points".Translate(data.Points.ToString("N0")));
            }

            twitchMessage.Reply(
                string.Join("⎮", container.ToArray())
                    .WithHeader($"{data.Class}")
            );
        }
    }
}
