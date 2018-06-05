using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Logic {
    public class SolarPanelTileEntityLogic : GeneratorTileEntityLogic {
        public SolarPanelTileEntityLogic(Entity entity) : base(entity) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (!entityUniverseFacade.TryGetLightPower(Location, out var efficiency, out _, out _)) {
                base.Update(timestep, entityUniverseFacade, 0);
                return;
            }

            base.Update(timestep, entityUniverseFacade, efficiency);
        }
    }
}
