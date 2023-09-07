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
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils;

/// <summary>
///     A registry for housing the various compatibility providers within
///     the mod.
/// </summary>
public static class CompatRegistry
{
    private static readonly List<ICompatibilityProvider> CompatibilityProviders = new List<ICompatibilityProvider>();
    private static readonly List<ISurgeryHandler> SurgeryHandlers = new List<ISurgeryHandler>();
    private static readonly List<IUsabilityHandler> UsabilityHandlers = new List<IUsabilityHandler>();
    private static readonly List<IHealHandler> HealHandlers = new List<IHealHandler>();
    private static readonly List<IPawnPowerHandler> PawnPowerHandlers = new List<IPawnPowerHandler>();

    /// <summary>
    ///     The main compatibility provider for A RimWorld of Magic.
    /// </summary>
    public static IMagicCompatibilityProvider? Magic { get; private set; }

    /// <summary>
    ///     Whether Twitch Toolkit is compatible with the current version of
    ///     RimWorld.
    /// </summary>
    public static bool ToolkitCompatible { get; } = Toolkit.Mod.Content.ModMetaData.VersionCompatible;

    /// <summary>
    ///     The main compatibility provider for Humanoid Alien Races.
    /// </summary>
    public static IAlienCompatibilityProvider? Alien { get; private set; }

    /// <summary>
    ///     A collection of surgery handlers used to ensure surgeries
    ///     requested by viewers are executed properly on their pawn for
    ///     their pawn's given race.
    /// </summary>
    public static IEnumerable<ISurgeryHandler> AllSurgeryHandlers => SurgeryHandlers;

    /// <summary>
    ///     A collection of usability handlers used to ensure items used by
    ///     viewers are executed properly on their pawn.
    /// </summary>
    public static IEnumerable<IUsabilityHandler> AllUsabilityHandlers => UsabilityHandlers;

    /// <summary>
    ///     A collection of heal handlers used to ensure certain hediffs
    ///     aren't healed from viewer's pawns, like blindsight.
    /// </summary>
    public static IEnumerable<IHealHandler> AllHealHandlers => HealHandlers;

    /// <summary>
    ///     A collection of power handlers used to provide an index of the
    ///     various powers a pawn can have across mods.
    /// </summary>
    public static IEnumerable<IPawnPowerHandler> AllPawnPowerHandlers => PawnPowerHandlers;

    /// <summary>
    ///     A collection of compatibility providers, including specialized
    ///     providers, currently registered within the mod.
    /// </summary>
    public static IEnumerable<ICompatibilityProvider> AllCompatibilityProviders => CompatibilityProviders;

    internal static void ProcessType(Type type)
    {
        if (!(Activator.CreateInstance(type) is ICompatibilityProvider provider))
        {
            return;
        }

        bool dependencyLoaded = provider.ModId.StartsWith("Ludeon")
            ? ModLister.GetExpansionWithIdentifier(provider.ModId)?.Status == ExpansionStatus.Active
            : ModLister.GetActiveModWithIdentifier(provider.ModId) != null;

        if (!dependencyLoaded)
        {
            return;
        }

        RegisterAndCatalogue((ICompatibilityProvider)Activator.CreateInstance(type));
    }

    private static void RegisterAndCatalogue(ICompatibilityProvider provider)
    {
        CompatibilityProviders.Add(provider);

        switch (provider)
        {
            case ISurgeryHandler surgery:
                SurgeryHandlers.Add(surgery);

                break;
            case IUsabilityHandler usability:
                UsabilityHandlers.Add(usability);

                break;
            case IHealHandler heal:
                HealHandlers.Add(heal);

                break;
            case IPawnPowerHandler pawnPower:
                PawnPowerHandlers.Add(pawnPower);

                break;
            case IMagicCompatibilityProvider magic:
                Magic = magic;

                break;
            case IAlienCompatibilityProvider alien:
                Alien = alien;

                break;
        }
    }

    /// <summary>
    ///     Returns whether a hediff should be healed.
    /// </summary>
    /// <param name="hediff">The hediff to check</param>
    /// <returns>Whether it should be healed</returns>
    public static bool IsHealable(Hediff hediff)
    {
        foreach (IHealHandler handler in HealHandlers)
        {
            if (!handler.CanHeal(hediff))
            {
                TkUtils.Logger.Info($"The handler {handler.GetType().FullDescription()} requested that the hediff {hediff.def.defName} not be healed.");

                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Returns whether a body part record should be healed.
    /// </summary>
    /// <param name="record">The record to check</param>
    /// <returns>Whether it should be healed</returns>
    public static bool IsHealable(BodyPartRecord record)
    {
        foreach (IHealHandler handler in HealHandlers)
        {
            if (!handler.CanHeal(record))
            {
                TkUtils.Logger.Info($"The handler {handler.GetType().FullDescription()} requested that the body part {record.def.defName} not be healed.");

                return false;
            }
        }

        return true;
    }
}
