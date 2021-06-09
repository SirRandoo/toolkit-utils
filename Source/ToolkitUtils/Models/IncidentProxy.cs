// ToolkitUtils
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Store;
using TwitchToolkit.Votes;

namespace SirRandoo.ToolkitUtils.Models
{
    public class IncidentProxy
    {
        public IncidentHelper SimpleIncident { get; set; }
        public IncidentHelperVariables VariablesIncident { get; set; }
        public string DefName => SimpleIncident?.storeIncident.defName ?? VariablesIncident.storeIncident.defName;
        public Viewer Viewer => SimpleIncident?.Viewer ?? VariablesIncident.Viewer;

        public bool TryExecute()
        {
            ViewerState state = Viewer.GetState();

            try
            {
                TryQueueNextMessage();
                SimpleIncident?.TryExecute();
                VariablesIncident?.TryExecute();
            }
            catch (Exception e)
            {
                LogHelper.Error(@$"The incident ""{DefName}"" encountered an exception while executing", e);

                // Reset their coins and karma prior to the event executing as
                // events shouldn't charge viewers until *after* the event
                // executes successfully. This doesn't affect simple incidents
                // as they're always taken from the viewer's balance before
                // being queued.
                Viewer.SetViewerCoins(state.Coins);
                Viewer.SetViewerKarma(state.Karma);
                Helper.playerMessages.Clear();
                return false;
            }

            if (VariablesIncident != null)
            {
                Purchase_Handler.viewerNamesDoingVariableCommands.Remove(
                    VariablesIncident.Viewer.username.ToLowerInvariant()
                );
            }

            Helper.playerMessages.Clear();
            return true;
        }

        private void TryQueueNextMessage()
        {
            if (SimpleIncident is VotingHelper)
            {
                return;
            }

            if (SimpleIncident != null)
            {
                Purchase_Handler.QueuePlayerMessage(SimpleIncident.Viewer, SimpleIncident.message);
            }

            if (VariablesIncident != null)
            {
                Purchase_Handler.QueuePlayerMessage(
                    VariablesIncident.Viewer,
                    VariablesIncident.message,
                    VariablesIncident.storeIncident.variables
                );
            }
        }
    }
}
