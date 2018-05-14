using Plukit.Base;
using Staxel.Core;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    class ChargeableTileComponentBuilder : IComponentBuilder {
        public string Kind() {
            return "chargeable";
        }

        public object Instance(Blob config) {
            return new ChargeableComponent(config);
        }
    }
}
