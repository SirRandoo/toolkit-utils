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
        private bool forAnimals = true;
        private bool forItems = true;
        private bool forPawns = true;
        private List<Gizmo> gizmos;

        public bool ForItems => forItems;
        public bool ForPawns => forPawns;
        public bool ForAnimals => forAnimals;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref forItems, "forItems", true);
            Scribe_Values.Look(ref forPawns, "forPawns", true);
            Scribe_Values.Look(ref forAnimals, "forAnimals", true);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!gizmos.NullOrEmpty())
            {
                return gizmos;
            }

            gizmos ??= new List<Gizmo>();

            gizmos.Add(
                new Command_Action
                {
                    action = () => Destroy(),
                    icon = Textures.CloseGateway,
                    defaultLabel = "TKUtils.CloseGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.CloseGatewayGizmo.Description".TranslateSimple()
                }
            );

            gizmos.Add(
                new Command_Toggle
                {
                    icon = Textures.Snowman,
                    defaultLabel = "TKUtils.TogglePawnGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.TogglePawnGatewayGizmo.Description".TranslateSimple(),
                    isActive = () => forPawns,
                    toggleAction = () => forPawns = !forPawns
                }
            );

            gizmos.Add(
                new Command_Toggle
                {
                    icon = Textures.HumanMeat,
                    defaultLabel = "TKUtils.ToggleItemGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.ToggleItemGatewayGizmo.Description".TranslateSimple(),
                    isActive = () => forItems,
                    toggleAction = () => forItems = !forItems
                }
            );

            gizmos.Add(
                new Command_Toggle
                {
                    icon = DefDatabase<ThingDef>.GetNamed("Rat").uiIcon,
                    defaultLabel = "TKUtils.ToggleAnimalGatewayGizmo.Label".TranslateSimple(),
                    defaultDesc = "TKUtils.ToggleAnimalGatewayGizmo.Description".TranslateSimple(),
                    isActive = () => forAnimals,
                    toggleAction = () => forAnimals = !forAnimals
                }
            );

            foreach (Gizmo gizmo in base.GetGizmos())
            {
                gizmos.Add(gizmo);
            }

            return gizmos;
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
