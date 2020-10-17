using JetBrains.Annotations;
using MoonSharp.Interpreter;
using TwitchToolkit.Incidents;

namespace SirRandoo.ToolkitUtils.Proxies
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class StoreIncidentProxy
    {
        private readonly StoreIncident storeIncident;
        private readonly StoreIncidentVariables storeIncidentVariables;

        [MoonSharpHidden]
        public StoreIncidentProxy(StoreIncident storeIncident)
        {
            this.storeIncident = storeIncident;
            storeIncidentVariables = storeIncident as StoreIncidentVariables;
        }

        public int Cost => storeIncident.cost;
        public int EventCap => storeIncident.eventCap;
        public string KarmaType => storeIncident.karmaType.ToString();
        public string Abbreviation => storeIncident.abbreviation;
        public string Abr => storeIncident.abbreviation;

        public bool IsVariables => storeIncidentVariables != null;
        public int MinPointsToFire => storeIncidentVariables?.minPointsToFire ?? -1;
        public int MaxWager => storeIncidentVariables?.maxWager ?? -1;
    }
}
