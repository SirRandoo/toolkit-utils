using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnClass : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            CharacterData data = MagicComp.GetCharacterData(pawn);

            if (data == null || !data.IsGifted || data.ClassName.NullOrEmpty())
            {
                twitchMessage.Reply("TKUtils.PawnClass.None".Localize());
                return;
            }

            var container = new List<string>
            {
                ResponseHelper.JoinPair(
                    "TKUtils.PawnClass.Level".Localize().CapitalizeFirst(),
                    data.Level.ToString("N0")
                ),
                ResponseHelper.JoinPair(
                    "TKUtils.PawnClass.Experience".Localize().CapitalizeFirst(),
                    data.ExperienceString
                )
            };

            string key = GetResourceKey(data);
            if (!key.NullOrEmpty())
            {
                string rateKey = key.Localize();
                var t = $"{data.CurrentResource:N0} / {data.MaxResource:N0} (";

                if (data.ResourceRegenRate > 0)
                {
                    t += "+";
                }

                t += $"{data.ResourceRegenRate:N0} {rateKey.Substring(0, 1).CapitalizeFirst()}P/5s";
                t += ")";

                container.Add(ResponseHelper.JoinPair(rateKey.CapitalizeFirst(), t));
            }

            if (data.SkillPoints > 0)
            {
                container.Add("TKUtils.PawnClass.Points".Localize(data.SkillPoints.ToString("N0")));
            }

            twitchMessage.Reply(container.GroupedJoin().WithHeader(Unrichify.StripTags(data.ClassName)));
        }

        private static string GetResourceKey(CharacterData data)
        {
            switch (data.Type)
            {
                case ClassTypes.Might:
                    return "TKUtils.PawnClass.Stamina";
                case ClassTypes.Magic:
                    return "TKUtils.PawnClass.Mana";
                default:
                    return null;
            }
        }
    }
}
