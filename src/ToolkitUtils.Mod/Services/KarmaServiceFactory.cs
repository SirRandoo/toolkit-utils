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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;
using ToolkitUtils.Mod.Domain.Settings;

namespace ToolkitUtils.Mod.Services;

/// <summary>
///     Provides a factory for creating instances of <see cref="IKarmaService" /> based on the specified configuration
///     settings defined in <see cref="KarmaSettings" />.
/// </summary>
/// <remarks>
///     The factory is designed to choose the appropriate implementation of <see cref="IKarmaService" /> depending on
///     the <see cref="KarmaServiceType" /> specified in the provided settings.
/// </remarks>
public sealed class KarmaServiceFactory(KarmaSettings settings)
{
    private readonly IKarmaService _classicKarmaService = new ClassicKarmaService();
    private readonly IKarmaService _momentumKarmaService = new MomentumKarmaService();

    /// <summary>
    ///     Determines and returns an appropriate implementation of <see cref="IKarmaService" /> based on the configured
    ///     service type.
    /// </summary>
    public IKarmaService GetKarmaService()
    {
        if (settings.ServiceType == KarmaServiceType.Classic) return _classicKarmaService;

        return settings.ServiceType == KarmaServiceType.Momentum
            ? _momentumKarmaService
            : throw new NotImplementedException("Karma service type not implemented: " + settings.ServiceType.Name);
    }
}
