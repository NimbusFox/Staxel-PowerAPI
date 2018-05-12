using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Builders {
    public class ChargeableComponentBuilder : IItemComponentBuilder {
        public virtual string Kind() {
            return "chargeable";
        }

        public virtual object Instance(BaseItemConfiguration item, Blob config) {
            return new ChargeableComponent(config);
        }
    }
}
