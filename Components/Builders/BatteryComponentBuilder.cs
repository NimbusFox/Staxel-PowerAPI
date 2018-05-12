using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Builders {
    public class BatteryComponentBuilder : ChargeableComponentBuilder {
        public new string Kind() {
            return "battery";
        }

        public new object Instance(BaseItemConfiguration item, Blob config) {
            return new BatteryComponent(config);
        }
    }
}
