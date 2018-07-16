using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Items.Builders {
    public class CapacitorItemBuilder : ChargeableItemBuilder {
        public override Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            if (spare is CapacitorItem) {
                if (spare.Configuration != null) {
                    spare.Restore(configuration, blob);
                    return spare;
                }
            }

            var capacitorItem = new CapacitorItem(this, configuration);
            capacitorItem.Restore(configuration, blob);
            return capacitorItem;
        }

        public override string Kind() {
            return "nimbusfox.powerapi.item.capacitor";
        }
    }
}
