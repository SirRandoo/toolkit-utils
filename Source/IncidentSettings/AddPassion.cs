using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class AddPassion : IncidentHelperVariablesSettings
    {
        public static bool Randomness = true;
        public static int ChanceToFail = 20;
        public static int ChanceToHop = 10;
        public static int ChanceToDecrease = 5;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Randomness, "addPassionRandomness", true);
            Scribe_Values.Look(ref ChanceToFail, "addPassionFailChance", 20);
            Scribe_Values.Look(ref ChanceToHop, "addPassionHopChance", 10);
            Scribe_Values.Look(ref ChanceToDecrease, "addPassionDecreaseChance", 5);
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new AddPassionDialog());
        }
    }
}
