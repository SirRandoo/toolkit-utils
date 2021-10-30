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
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class VisualExceptions
    {
        public static readonly bool Active;
        private static readonly Type ExceptionStateClass;
        private static readonly MethodInfo HandleExceptionMethod;

        static VisualExceptions()
        {
            if (ModLister.GetActiveModWithIdentifier("brrainz.visualexceptions") == null)
            {
                return;
            }

            try
            {
                ExceptionStateClass = AccessTools.TypeByName("VisualExceptions.ExceptionState");
                HandleExceptionMethod = AccessTools.Method(ExceptionStateClass, "Handle", new Type[1] { typeof(Exception) });
                Active = true;
            }
            catch (Exception e)
            {
                LogHelper.Error("Could not enable visual exception compatibility", e);
            }
        }

        public static void HandleException(Exception e)
        {
            try
            {
                HandleExceptionMethod.Invoke(ExceptionStateClass, new object[] { e });
            }
            catch (Exception exception)
            {
                LogHelper.Error("Could not pass exception to VisualExceptions", exception);
            }
        }
    }
}
