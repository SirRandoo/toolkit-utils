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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Workers;
using Verse;

namespace SirRandoo.ToolkitUtils.TMagic.Patches
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ClassSelectorPatch
    {
        private static MethodBase _traitWorkerPrepare;
        private static AccessTools.FieldRef<object, List<FloatMenuOption>> _field;
        private static MethodInfo _method;

        public static bool Prepare()
        {
            _field ??= AccessTools.FieldRefAccess<List<FloatMenuOption>>(typeof(TraitWorker), "SelectorAdders");
            _method ??= AccessTools.Method(typeof(TraitWorker), "AddSelector");

            return true;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return _traitWorkerPrepare ??= AccessTools.Method(typeof(TraitWorker), "Prepare");
        }

        public static void Postfix([CanBeNull] TraitWorker __instance)
        {
            if (__instance == null)
            {
                return;
            }

            _field.Invoke(__instance)
               .Add(
                    new FloatMenuOption(
                        "TKUtils.Fields.Class".Localize(),
                        () => _method?.Invoke(__instance, new object[] {new ClassSelector()})
                    )
                );
        }
    }
}
