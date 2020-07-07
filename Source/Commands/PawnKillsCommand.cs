using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnKillsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".TranslateSimple());
                return;
            }

            int totalKills = pawn.records.GetAsInt(RecordDefOf.Kills);
            int animalKills = pawn.records.GetAsInt(RecordDefOf.KillsAnimals);
            int humanLikeKills = pawn.records.GetAsInt(RecordDefOf.KillsHumanlikes);
            int mechanoidKills = pawn.records.GetAsInt(RecordDefOf.KillsMechanoids);

            string container = ResponseHelper.JoinPair("TKUtils.Total".Localize(), totalKills.ToString("N0"));

            container += ResponseHelper.OuterGroupSeparator;
            container += string.Join(
                ", ",
                ResponseHelper.JoinPair(
                    "TKUtils.PawnKills.Humanlike".Localize().CapitalizeFirst(),
                    humanLikeKills.ToString("N0")
                ),
                ResponseHelper.JoinPair(
                    "TKUtils.PawnKills.Animals".Localize().CapitalizeFirst(),
                    animalKills.ToString("N0")
                ),
                ResponseHelper.JoinPair(
                    "TKUtils.PawnKills.Mechanoids".Localize().CapitalizeFirst(),
                    mechanoidKills.ToString("N0")
                )
            );


            twitchMessage.Reply(container);
        }
    }
}
