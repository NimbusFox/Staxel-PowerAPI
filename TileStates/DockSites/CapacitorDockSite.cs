using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Items;
using Staxel.Docks;
using Staxel.Logic;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.TileStates.DockSites {
    public class CapacitorDockSite : ChargeableDockSite {
        public CapacitorDockSite(Entity entity, DockSiteId id, DockSiteConfiguration dockSiteConfig) : base(entity, id, dockSiteConfig) { }

        public override int CanDock(ItemStack stack) {
            if (stack.Item is CapacitorItem battery) {
                return 1;
            }

            return 0;
        }

        public new static string Name => "capacitor";
    }
}