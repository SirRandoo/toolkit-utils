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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class PawnWorker : ItemWorkerBase<TableWorker<TableSettingsItem<PawnKindItem>>, PawnKindItem>
    {
        public override void Prepare()
        {
            base.Prepare();
            Worker = new PawnTableWorker();
            Worker.Prepare();

            SelectorAdders = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.Fields.Name".Localize(),
                    () => AddSelector(new NameSelector<PawnKindItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.State".Localize(),
                    () => AddSelector(new StateSelector<PawnKindItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Price".Localize(),
                    () => AddSelector(new PriceSelector<PawnKindItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.DefName".Localize(),
                    () => AddSelector(new DefNameSelector<PawnKindItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Mod".Localize(),
                    () => AddSelector(new ModSelector<PawnKindItem>())
                )
            };

            MutateAdders = new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.Fields.Name".Localize(), () => AddMutator(new PawnNameMutator())),
                new FloatMenuOption(
                    "TKUtils.Fields.Price".Localize(),
                    () => AddMutator(new PriceMutator<PawnKindItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.State".Localize(),
                    () => AddMutator(new StateMutator<PawnKindItem>())
                )
            };
        }
    }
}
