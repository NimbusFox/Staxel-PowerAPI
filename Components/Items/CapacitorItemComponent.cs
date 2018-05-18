using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components.Items {
    public class CapacitorItemComponent : ChargeableComponent {
        public bool OutputToTiles { get; }

        public CapacitorItemComponent(Blob config) : base(config) {
            OutputToTiles = config.GetBool("outputToTiles", true);
        }
    }
}
