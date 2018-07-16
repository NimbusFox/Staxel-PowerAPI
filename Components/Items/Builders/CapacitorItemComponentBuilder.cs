using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Items.Builders {
    public class CapacitorItemComponentBuilder : IItemComponentBuilder {
        public string Kind() {
            return "capacitor";
        }

        public object Instance(BaseItemConfiguration item, Blob config) {
            return new CapacitorItemComponent(config);
        }
    }
}
