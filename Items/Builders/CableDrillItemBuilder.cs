using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Items.Builders {
    class CableDrillItemBuilder : ChargeableItemBuilder {
        public override Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            if (spare is CableDrillItem) {
                if (spare.Configuration != null) {
                    spare.Restore(configuration, blob);
                    return spare;
                }
            }

            var chargeableItem = new CableDrillItem(this, configuration);
            chargeableItem.Restore(configuration, blob);
            return chargeableItem;
        }

        public override string Kind() {
            return KindCode();
        }

        public new static string KindCode() => "nimbusfox.powerapi.item.cableDrill";
    }
}
