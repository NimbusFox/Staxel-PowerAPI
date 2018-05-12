using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Tiles.DockSites;
using Plukit.Base;
using Staxel.Core;
using Staxel.Docks;
using Staxel.Logic;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class GeneratorTileStateEntityLogic : DockTileStateEntityLogic {
        public GeneratorTileStateEntityLogic(Entity entity) : base(entity) {
            
        }

        protected override void AddSite(DockSiteConfiguration config) {
            _dockSites.Add(new GeneratorDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
        }

        public override Vector3F InteractCursorColour() {
            return !HasAnyDockedItems() ? base.InteractCursorColour() : Constants.InteractCursorColour;
        }

        public override void Update(Timestep timestep, EntityUniverseFacade universe) {
            base.Update(timestep, universe);
        }
    }
}
