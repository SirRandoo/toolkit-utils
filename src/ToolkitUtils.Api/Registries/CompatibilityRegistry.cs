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
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>A registry for housing compatibility providers used by the mod to provide aid in mod compatibility.</summary>
/// <param name="AllRegistrants"></param>
[PublicAPI]
public record CompatibilityRegistry(IReadOnlyList<ICompatibilityProvider> AllRegistrants) : FrozenRegistry<ICompatibilityProvider>(AllRegistrants)
{
    private static readonly IReadOnlyList<ICompatibilityProvider> Providers;
    private static readonly IReadOnlyDictionary<string, IPawnProvider> PawnProviders;
    private static readonly IReadOnlyDictionary<string, IHealProvider> HealProviders;
    private static readonly IReadOnlyDictionary<string, IItemProvider> ItemProviders;
    private static readonly IReadOnlyDictionary<string, ISurgeryProvider> SurgeryProviders;
    private static readonly IReadOnlyDictionary<string, ITraitProvider> TraitProviders;
    private static readonly IReadOnlyDictionary<string, IUsabilityProvider> UsabilityProviders;

    static CompatibilityRegistry()
    {
        var providers = new List<ICompatibilityProvider>();
        var pawnProviders = new Dictionary<string, IPawnProvider>();
        var healProviders = new Dictionary<string, IHealProvider>();
        var itemProviders = new Dictionary<string, IItemProvider>();
        var surgeryProviders = new Dictionary<string, ISurgeryProvider>();
        var traitProviders = new Dictionary<string, ITraitProvider>();
        var usabilityProviders = new Dictionary<string, IUsabilityProvider>();

        foreach (Type type in typeof(ICompatibilityProvider).AllSubclassesNonAbstract())
        {
            if (Activator.CreateInstance(type) is not ICompatibilityProvider provider) continue;

            providers.Add(provider);

            switch (provider)
            {
                case IUsabilityProvider usabilityProvider: usabilityProviders.Add(provider.Id, usabilityProvider); break;
                case IPawnProvider pawnProvider:           pawnProviders.Add(provider.Id, pawnProvider); break;
                case IHealProvider healProvider:           healProviders.Add(provider.Id, healProvider); break;
                case IItemProvider itemProvider:           itemProviders.Add(provider.Id, itemProvider); break;
                case ISurgeryProvider surgeryProvider:     surgeryProviders.Add(provider.Id, surgeryProvider); break;
                case ITraitProvider traitProvider:         traitProviders.Add(provider.Id, traitProvider); break;
            }
        }

        Providers = providers;
        HealProviders = healProviders;
        ItemProviders = itemProviders;
        PawnProviders = pawnProviders;
        TraitProviders = traitProviders;
        SurgeryProviders = surgeryProviders;
        UsabilityProviders = usabilityProviders;
    }

    /// <summary>Retrieves the pawn provider associated with the specified provider ID.</summary>
    /// <param name="providerId">The identifier of the pawn provider to be retrieved.</param>
    /// <returns>The <see cref="IPawnProvider" /> associated with the given provider ID, or null if not found.</returns>
    public IPawnProvider? GetPawnProvider(string providerId) => PawnProviders.GetValueOrDefault(providerId);

    /// <summary>Retrieves the heal provider associated with the specified provider ID.</summary>
    /// <param name="providerId">The identifier of the heal provider to be retrieved.</param>
    /// <returns>The <see cref="IHealProvider" /> associated with the given provider ID, or null if not found.</returns>
    public IHealProvider GetHealProvider(string providerId) => HealProviders.GetValueOrDefault(providerId);

    /// <summary>Retrieves the item provider associated with the specified provider ID.</summary>
    /// <param name="providerId">The identifier of the item provider to be retrieved.</param>
    /// <returns>The <see cref="IItemProvider" /> associated with the given provider ID, or null if not found.</returns>
    public IItemProvider GetItemProvider(string providerId) => ItemProviders.GetValueOrDefault(providerId);

    /// <summary>Retrieves the surgery provider associated with the specified provider ID.</summary>
    /// <param name="providerId">The identifier of the surgery provider to be retrieved.</param>
    /// <returns>The <see cref="ISurgeryProvider" /> associated with the given provider ID, or null if not found.</returns>
    public ISurgeryProvider GetSurgeryProvider(string providerId) => SurgeryProviders.GetValueOrDefault(providerId);

    /// <summary>Retrieves the trait provider associated with the specified provider ID.</summary>
    /// <param name="providerId">The identifier of the trait provider to be retrieved.</param>
    /// <returns>The <see cref="ITraitProvider" /> associated with the given provider ID, or null if not found.</returns>
    public ITraitProvider GetTraitProvider(string providerId) => TraitProviders.GetValueOrDefault(providerId);

    /// <summary>Retrieves the usability provider associated with the specified provider ID.</summary>
    /// <param name="providerId">The identifier of the usability provider to be retrieved.</param>
    /// <returns>The <see cref="IUsabilityProvider" /> associated with the given provider ID, or null if not found.</returns>
    public IUsabilityProvider GetUsabilityProvider(string providerId) => UsabilityProviders.GetValueOrDefault(providerId);

    /// <summary>
    ///     Creates a new instance of a compatibility registry preloaded with all the registered
    ///     <see cref="ICompatibilityProvider" /> implementations.
    /// </summary>
    public static CompatibilityRegistry CreateDefault() => new(Providers);
}
