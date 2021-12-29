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
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class ToolkitGateway : Building
    {
        private bool _forAnimals = true;
        private bool _forItems = true;
        private bool _forPawns = true;
        private List<Gizmo> _gizmos;

        public bool ForItems => _forItems;
        public bool ForPawns => _forPawns;
        public bool ForAnimals => _forAnimals;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref _forItems, "forItems", true);
            Scribe_Values.Look(ref _forPawns, "forPawns", true);
            Scribe_Values.Look(ref _forAnimals, "forAnimals", true);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!_gizmos.NullOrEmpty())
            {
                return _gizmos;
            }

            _gizmos ??= new List<Gizmo>();

            _gizmos.Add(
                new Command_Action
                {
                    action = () => Destroy(),
                    icon = Textures.CloseGateway,
                    defaultLabel = "TKUtils.CloseGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.CloseGatewayGizmo.Description".TranslateSimple()
                }
            );

            _gizmos.Add(
                new Command_Toggle
                {
                    icon = Textures.Snowman,
                    defaultLabel = "TKUtils.TogglePawnGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.TogglePawnGatewayGizmo.Description".TranslateSimple(),
                    isActive = () => _forPawns,
                    toggleAction = () => _forPawns = !_forPawns
                }
            );

            _gizmos.Add(
                new Command_Toggle
                {
                    icon = Textures.HumanMeat,
                    defaultLabel = "TKUtils.ToggleItemGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.ToggleItemGatewayGizmo.Description".TranslateSimple(),
                    isActive = () => _forItems,
                    toggleAction = () => _forItems = !_forItems
                }
            );

            _gizmos.Add(
                new Command_Toggle
                {
                    icon = DefDatabase<ThingDef>.GetNamed("Rat").uiIcon,
                    defaultLabel = "TKUtils.ToggleAnimalGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.ToggleAnimalGatewayGizmo.Description".TranslateSimple(),
                    isActive = () => _forAnimals,
                    toggleAction = () => _forAnimals = !_forAnimals
                }
            );

            foreach (Gizmo gizmo in base.GetGizmos())
            {
                _gizmos.Add(gizmo);
            }

            return _gizmos;
        }

        public override void TickLong()
        {
            if (!TkSettings.EasterEggs)
            {
                return;
            }

            bool shouldSpawnRat = Rand.Chance(0.01f);
            bool shouldSpawnBoomRat = Rand.Chance(0.1f);

            if (shouldSpawnRat)
            {
                GenSpawn.Spawn(PawnGenerator.GeneratePawn(shouldSpawnBoomRat ? PawnKindDefOf.Boomrat : PawnKindDefOf.Rat), Position, Map);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Current.Game.GetComponent<Coordinator>()?.RemovePortal(this);
            base.Destroy(mode);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Current.Game.GetComponent<Coordinator>()?.RegisterPortal(this);
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = true;
        }
    }
}
