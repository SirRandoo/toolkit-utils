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
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            int totalKills = pawn.records.GetAsInt(RecordDefOf.Kills);
            int animalKills = pawn.records.GetAsInt(RecordDefOf.KillsAnimals);
            int humanLikeKills = pawn.records.GetAsInt(RecordDefOf.KillsHumanlikes);
            int mechanoidKills = pawn.records.GetAsInt(RecordDefOf.KillsMechanoids);

            string mechanoidLabel = RecordDefOf.KillsMechanoids.label.Substring(
                7,
                RecordDefOf.KillsMechanoids.label.LastIndexOf(')') - 7
            );
            string animalLabel = RecordDefOf.KillsAnimals.label.Substring(
                7,
                RecordDefOf.KillsAnimals.label.LastIndexOf(')') - 7
            );
            string humanlikeLabel = RecordDefOf.KillsHumanlikes.label.Substring(
                7,
                RecordDefOf.KillsHumanlikes.label.LastIndexOf(')') - 7
            );

            string container = "TKUtils.Formats.KeyValue"
                .Translate("TKUtils.Misc.Total".Translate(), totalKills.ToString("N0"))
                .RawText;

            container += "⎮";
            container += string.Join(
                ", ",
                "TKUtils.Formats.KeyValue".Translate(humanlikeLabel.CapitalizeFirst(), humanLikeKills.ToString("N0")),
                "TKUtils.Formats.KeyValue".Translate(animalLabel.CapitalizeFirst(), animalKills.ToString("N0")),
                "TKUtils.Formats.KeyValue".Translate(mechanoidLabel.CapitalizeFirst(), mechanoidKills.ToString("N0"))
            );


            twitchMessage.Reply(container);
        }
    }
}
