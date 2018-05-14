using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Items.Builders {
    public class ChargeableItemComponentBuilder : IItemComponentBuilder {
        public virtual string Kind() {
            return "chargeable";
        }

        public virtual object Instance(BaseItemConfiguration item, Blob config) {
            return new ChargeableComponent(config);
        }
    }
}
