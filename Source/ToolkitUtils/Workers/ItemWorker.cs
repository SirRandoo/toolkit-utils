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

using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemWorker : ItemWorkerBase<TableWorker<TableSettingsItem<ThingItem>>, ThingItem>
    {
        public override void Prepare()
        {
            base.Prepare();
            Worker = new ItemTableWorker();
            Worker.Prepare();

            SelectorAdders = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.Fields.Price".Localize(),
                    () => AddSelector(new PriceSelector<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Name".Localize(),
                    () => AddSelector(new NameSelector<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.DefName".Localize(),
                    () => AddSelector(new DefNameSelector<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Mod".Localize(),
                    () => AddSelector(new ModSelector<ThingItem>())
                ),
                new FloatMenuOption("TKUtils.Fields.IsWeapon".Localize(), () => AddSelector(new WeaponSelector())),
                new FloatMenuOption(
                    "TKUtils.Fields.IsMeleeWeapon".Localize(),
                    () => AddSelector(new MeleeWeaponSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.IsRangedWeapon".Localize(),
                    () => AddSelector(new RangedWeaponSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.HasQuantityLimit".Localize(),
                    () => AddSelector(new HasQuantityLimitSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.QuantityLimit".Localize(),
                    () => AddSelector(new QuantityLimitSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Category".Localize(),
                    () => AddSelector(new CategorySelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Category".Localize().Pluralize(),
                    () => AddSelector(new CategoriesSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.State".Localize(),
                    () => AddSelector(new StateSelector<ThingItem>())
                ),
                new FloatMenuOption("TKUtils.Fields.CanBeStuff".Localize(), () => AddSelector(new StuffSelector())),
                new FloatMenuOption("TKUtils.Fields.Weight".Localize(), () => AddSelector(new WeightSelector())),
                new FloatMenuOption(
                    "TKUtils.Fields.CanStack".Localize(),
                    () => AddSelector(new StackabilitySelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Manufactured".Localize(),
                    () => AddSelector(new ManufacturedSelector())
                ),
                new FloatMenuOption("TKUtils.Fields.CanUse".Localize(), () => AddSelector(new UsabilitySelector())),
                new FloatMenuOption("TKUtils.Fields.CanWear".Localize(), () => AddSelector(new WearableSelector())),
                new FloatMenuOption(
                    "TKUtils.Fields.CanEquip".Localize(),
                    () => AddSelector(new EquippableSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Technology".Localize(),
                    () => AddSelector(new TechnologySelector())
                )
            };

            MutateAdders = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.Fields.HasQuantityLimit".Localize(),
                    () => AddMutator(new HasQuantityLimitMutator())
                ),
                new FloatMenuOption("TKUtils.Fields.Name".Localize(), () => AddMutator(new ItemNameMutator())),
                new FloatMenuOption("TKUtils.Fields.Price".Localize(), () => AddMutator(new ItemPriceMutator())),
                new FloatMenuOption(
                    "TKUtils.Fields.QuantityLimit".Localize(),
                    () => AddMutator(new QuantityLimitMutator())
                ),
                new FloatMenuOption("TKUtils.Fields.State".Localize(), () => AddMutator(new ItemStateMutator())),
                new FloatMenuOption("TKUtils.Fields.CanBeStuff".Localize(), () => AddMutator(new StuffMutator())),
                new FloatMenuOption("TKUtils.Fields.Weight".Localize(), () => AddMutator(new WeightMutator())),
                new FloatMenuOption("TKUtils.Fields.CanUse".Localize(), () => AddMutator(new UsableMutator())),
                new FloatMenuOption("TKUtils.Fields.CanWear".Localize(), () => AddMutator(new WearableMutator())),
                new FloatMenuOption(
                    "TKUtils.Fields.CanEquip".Localize(),
                    () => AddMutator(new EquippableMutator())
                ),
                new FloatMenuOption(
                    "TKUtils.EditorMutator.ResetName".Localize(),
                    () => AddMutator(new ResetNameMutator<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.EditorMutator.ResetPrice".Localize(),
                    () => AddMutator(new ResetPriceMutator<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.EditorMutator.ResetData".Localize(),
                    () => AddMutator(new ResetDataMutator<ThingItem>())
                )
            };
        }
    }
}
