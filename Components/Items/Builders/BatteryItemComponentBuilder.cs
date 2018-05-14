using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Items.Builders {
    public class BatteryItemComponentBuilder : ChargeableItemComponentBuilder {
        public override string Kind() {
            return "battery";
        }

        public override object Instance(BaseItemConfiguration item, Blob config) {
            return new BatteryComponent(config);
        }
    }
}
