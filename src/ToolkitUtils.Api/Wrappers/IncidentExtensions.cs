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
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>A collection of extension for asynchronously executing incidents in the game in a safe way.</summary>
[PublicAPI]
public static class IncidentExtensions
{
    /// <summary>Returns whether an incident can be executed.</summary>
    /// <param name="worker">A <see cref="IncidentWorker" /> instance representing the incident being executed.</param>
    /// <param name="params">A <see cref="IncidentParms" /> instance containing the relevant context for the incident.</param>
    public static async Task<bool> CanFireNowAsync(this IncidentWorker worker, IncidentParms @params) => await MainThreadExtensions.OnMainAsync(worker.CanFireNow, @params);

    /// <summary>Executes an incident.</summary>
    /// <param name="worker">A <see cref="IncidentWorker" /> instance representing the incident being executed.</param>
    /// <param name="params">A <see cref="IncidentParms" /> instance containing the relevant context for the incident.</param>
    /// <returns>Whether the incident was successfully executed.</returns>
    public static async Task<bool> TryExecuteAsync(this IncidentWorker worker, IncidentParms @params) => await MainThreadExtensions.OnMainAsync(worker.TryExecute, @params);
}
