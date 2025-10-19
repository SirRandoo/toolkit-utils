// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using TwitchToolkit;
using TwitchToolkit.Incidents;
using Karma = TwitchToolkit.Karma;
using Viewer = ToolkitUtils.Mod.Data.Viewer;

namespace ToolkitUtils.Api.Extensions;

public static class ViewerExtensions
{
    public static void Charge(this Viewer viewer, StoreIncident incident, int amount = 1)
    {
        int cost = incident.cost * amount;

        if (!ToolkitSettings.UnlimitedCoins) viewer.RemoveCoins(amount: cost);

        viewer.SetKarma(Karma.CalculateNewKarma(viewer.Karma, incident.karmaType, cost));
    }
}
