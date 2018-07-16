using NimbusFox.PowerAPI.Components.Tiles;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Items.Builders {
    public class CableItemComponentBuilder : IItemComponentBuilder {
        public string Kind() {
            return "cable";
        }

        public object Instance(BaseItemConfiguration item, Blob config) {
            return new CableComponent(config);
        }
    }
}
