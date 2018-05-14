using Plukit.Base;

namespace NimbusFox.PowerAPI.Components.Items {
    public class BatteryComponent : ChargeableComponent {
        public bool ChargeInventory { get; }

        public BatteryComponent(Blob config) : base(config) {
            ChargeInventory = config.GetBool("chargeInventory", false);
        }
    }
}
