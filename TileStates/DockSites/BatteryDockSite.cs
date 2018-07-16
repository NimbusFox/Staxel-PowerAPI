using NimbusFox.PowerAPI.Items;
using Staxel.Docks;
using Staxel.Logic;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.TileStates.DockSites {
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
