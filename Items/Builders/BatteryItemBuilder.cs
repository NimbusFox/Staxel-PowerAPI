using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Items.Builders {
    public class BatteryItemBuilder : ChargeableItemBuilder {

        public override Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            if (spare is BatteryItem) {
                if (spare.Configuration != null) {
                    spare.Restore(configuration, blob);
                    return spare;
                }
            }

            var batteryItem = new BatteryItem(this, configuration);
            batteryItem.Restore(configuration, blob);
            return batteryItem;
        }

        public override string Kind() {
            return "nimbusfox.item.battery";
        }
    }
}
