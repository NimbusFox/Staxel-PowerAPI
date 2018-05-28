using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class SolarPanelTileEntityLogic : ChargeableTileEntityLogic {
        public SolarPanelTileEntityLogic(Entity entity) : base(entity) { }

        private bool _generatePower = false;
        private bool _timePaused = false;

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            base.Update(timestep, entityUniverseFacade);

            if (entityUniverseFacade.ReadLighting(new Vector3I(Location.X, Location.Y + 1, Location.Z), TileAccessFlags.SynchronousWait, out var tileLight,
                out var tileEmissive)) {
                _timePaused = entityUniverseFacade.DayNightCycle().GamePaused();

                if (entityUniverseFacade.DayNightCycle().IsNight) {
                    _generatePower = false;
                    goto runCycle;
                }
            }
            runCycle:
            Cycle.RunCycle(RunCycle);
        }

        public override void RunCycle() {
            if (!_generatePower) {
                goto runBaseCycle;
            }

            runBaseCycle:
            base.RunCycle();
        }
    }
}
