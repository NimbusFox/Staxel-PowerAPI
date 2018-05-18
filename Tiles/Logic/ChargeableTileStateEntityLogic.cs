using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.Tiles.DockSites;
using Plukit.Base;
using Staxel.Core;
using Staxel.Docks;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class ChargeableTileStateEntityLogic : DockTileStateEntityLogic {

        public Power TilePower {
            get {
                foreach (var dock in _dockSites) {
                    if (dock.DockedItem.Stack.Item is CapacitorItem capacitor) {
                        return capacitor.ItemPower;
                    }
                }

                return null;
            }
        }

        public IReadOnlyList<DockSite> DockSites => _dockSites;

        public ChargeableTileStateEntityLogic(Entity entity) : base(entity) {
        }

        protected override void AddSite(DockSiteConfiguration config) {
            if (config.SiteName.StartsWith(BatteryDockSite.Name, StringComparison.CurrentCultureIgnoreCase)) {
                _dockSites.Add(new BatteryDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
                return;
            }

            if (config.SiteName.StartsWith(ChargeableDockSite.Name, StringComparison.CurrentCultureIgnoreCase)) {
                _dockSites.Add(new ChargeableDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
                return;
            }

            if (config.SiteName == CapacitorDockSite.Name) {
                _dockSites.Add(new CapacitorDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
                return;
            }
        }

        public override Vector3F InteractCursorColour() {
            return !HasAnyDockedItems() ? base.InteractCursorColour() : Constants.InteractCursorColour;
        }

        public override void Update(Timestep timestep, EntityUniverseFacade universe) {
            base.Update(timestep, universe);
            PowerDockHook.AddLocation(Location);
        }
    }
}
