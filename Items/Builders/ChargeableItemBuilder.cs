﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Items.Builders {
    public class ChargeableItemBuilder : IItemBuilder, IDisposable {

        public ItemRenderer Renderer { get; private set; }

        public void Dispose() { }
        public void Load() { }
        public virtual Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            if (spare is ChargeableItem) {
                if (spare.Configuration != null) {
                    spare.Restore(configuration, blob);
                    return spare;
                }
            }

            var chargeableItem = new ChargeableItem(this, configuration);
            chargeableItem.Restore(configuration, blob);
            return chargeableItem;
        }

        public virtual string Kind() {
            return "nimbusfox.item.chargeable";
        }
    }
}
