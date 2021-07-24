﻿// ToolkitUtils
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

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Storytellers;
using TwitchToolkit.Votes;

namespace SirRandoo.ToolkitUtils.Harmony
{
#if RW13
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class StorytellerPatch
    {
        public static bool Prepare()
        {
            return RuntimeChecker.Do13Patches;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(StorytellerComp_TwitchToolkit), "MakeIntervalIncidents");
        }

        public static bool Prefix()
        {
            VoteHandler.voteActive = true;
            return true;
        }
    }
#endif
}