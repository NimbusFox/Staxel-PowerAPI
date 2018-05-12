using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components {
    public class BatteryComponent : ChargeableComponent {
        public bool ChargeInventory { get; }

        public BatteryComponent(Blob config) : base(config) {
            ChargeInventory = config.GetBool("chargeInventory", false);
        }
    }
}
