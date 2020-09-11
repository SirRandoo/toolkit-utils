using System;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class RuntimeChecker
    {
        static RuntimeChecker()
        {
            TkSettings.ValidateDynamicSettings();
            TkUtils.Context ??= SynchronizationContext.Current;

            var wereChanges = false;

            // We're not going to update this to use EventExtension
            // since it appears to wipe previous settings.
            foreach (StoreIncident incident in DefDatabase<StoreIncident>.AllDefs.Where(
                i => i.defName == "BuyPawn" || i.defName == "AddTrait" || i.defName == "RemoveTrait"
            ))
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                incident.cost = 1;
                wereChanges = true;
            }

            if (wereChanges)
            {
                Store_IncidentEditor.UpdatePriceSheet();
            }
        }

        internal static void ExecuteInMainThread(Action func)
        {
            if (TkSettings.MainThreadCommands && TkUtils.Context != null)
            {
                TkUtils.Context.Post(delegate { func(); }, null);
                return;
            }

            func();
        }
    }
}
