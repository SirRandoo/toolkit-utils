using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnKillsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var totalKills = pawn.records.GetAsInt(RecordDefOf.Kills);
            var animalKills = pawn.records.GetAsInt(RecordDefOf.KillsAnimals);
            var humanLikeKills = pawn.records.GetAsInt(RecordDefOf.KillsHumanlikes);
            var mechanoidKills = pawn.records.GetAsInt(RecordDefOf.KillsMechanoids);

            var mechanoidLabel = RecordDefOf.KillsMechanoids.label.Substring(
                7,
                RecordDefOf.KillsMechanoids.label.LastIndexOf(')') - 7
            );
            var animalLabel = RecordDefOf.KillsAnimals.label.Substring(
                7,
                RecordDefOf.KillsAnimals.label.LastIndexOf(')') - 7
            );
            var humanlikeLabel = RecordDefOf.KillsHumanlikes.label.Substring(
                7,
                RecordDefOf.KillsHumanlikes.label.LastIndexOf(')') - 7
            );

            var container = "TKUtils.Formats.KeyValue"
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
