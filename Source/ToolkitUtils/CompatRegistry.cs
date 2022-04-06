// MIT License
// 
// Copyright (c) 2021 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public static class CompatRegistry
    {
        private static readonly List<ICompatibilityProvider> CompatibilityProviders = new List<ICompatibilityProvider>();
        private static readonly List<ISurgeryHandler> SurgeryHandlers = new List<ISurgeryHandler>();
        private static readonly List<IUsabilityHandler> UsabilityHandlers = new List<IUsabilityHandler>();
        private static readonly List<IHealHandler> HealHandlers = new List<IHealHandler>();
        private static readonly List<IPawnPowerHandler> PawnPowerHandlers = new List<IPawnPowerHandler>();

        public static IMagicCompatibilityProvider Magic { get; private set; }
        public static IAlienCompatibilityProvider Alien { get; private set; }
        public static IEnumerable<ISurgeryHandler> AllSurgeryHandlers => SurgeryHandlers;
        public static IEnumerable<IUsabilityHandler> AllUsabilityHandlers => UsabilityHandlers;
        public static IEnumerable<IHealHandler> AllHealHandlers => HealHandlers;
        public static IEnumerable<IPawnPowerHandler> AllPawnPowerHandlers => PawnPowerHandlers;
        public static IEnumerable<ICompatibilityProvider> AllCompatibilityProviders => CompatibilityProviders;

        internal static void ProcessType([NotNull] Type type)
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

            RegisterAndCatalogue((ICompatibilityProvider) Activator.CreateInstance(type));
        }

        private static void RegisterAndCatalogue([NotNull] ICompatibilityProvider provider)
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
}
