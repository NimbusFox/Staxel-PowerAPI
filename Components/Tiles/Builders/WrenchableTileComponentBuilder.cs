using Plukit.Base;
using Staxel.Core;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    public class WrenchableTileComponentBuilder : IComponentBuilder, IItemComponentBuilder {
        public string Kind() {
            return "wrenchable";
        }

        public object Instance(BaseItemConfiguration item, Blob config) {
            return new WrenchableComponent(config);
        }

        public object Instance(Blob config) {
            return new WrenchableComponent(config);
        }
    }
}
