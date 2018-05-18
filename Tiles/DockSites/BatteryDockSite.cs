using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Items;
using Plukit.Base;
using Staxel.Docks;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Player;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.DockSites {
    public class BatteryDockSite : ChargeableDockSite {
        public BatteryDockSite(Entity entity, DockSiteId id, DockSiteConfiguration dockSiteConfig) : base(entity, id, dockSiteConfig) { }
        public override int CanDock(ItemStack stack) {
            if (stack.Item is BatteryItem battery) {
                if (battery.ItemPower.CurrentCharge == 0) {
                    return 0;
                }

                return 1;
            }

            return 0;
        }

        public new static string Name => "battery";
    }
}
