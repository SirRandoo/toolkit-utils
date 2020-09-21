using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class RemovePassion : IncidentHelperVariablesSettings
    {
        public static bool Randomness = true;
        public static int ChanceToFail = 20;
        public static int ChanceToHop = 10;
        public static int ChangeToIncrease = 5;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Randomness, "removePassionRandomness", true);
            Scribe_Values.Look(ref ChanceToFail, "removePassionFailChance", 20);
            Scribe_Values.Look(ref ChanceToHop, "removePassionHopChance", 10);
            Scribe_Values.Look(ref ChangeToIncrease, "removePassionIncreaseChance", 5);
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new RemovePassionDialog());
        }
    }
}
