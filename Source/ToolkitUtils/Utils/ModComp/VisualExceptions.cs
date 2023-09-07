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
using System.Reflection;
using HarmonyLib;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp;

[StaticConstructorOnStartup]
public static class VisualExceptions
{
    // ReSharper disable once StringLiteralTypo
    public static readonly bool Active = ModLister.GetActiveModWithIdentifier("brrainz.visualexceptions") != null;
    private static readonly Type ExceptionStateClass = AccessTools.TypeByName("VisualExceptions.ExceptionState");
    private static readonly MethodInfo HandleExceptionMethod = AccessTools.Method("VisualExceptions.ExceptionState:Handle", new[] { typeof(Exception) });

    public static void HandleException(Exception e)
    {
        try
        {
            HandleExceptionMethod.Invoke(ExceptionStateClass, new object[] { e });
        }
        catch (Exception exception)
        {
            TkUtils.Logger.Error("Could not pass exception to VisualExceptions", exception);
        }
    }
}
