using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    public class ChargeableTileComponentBuilder : ITileComponentBuilder {
        public string Kind() {
            return "chargeable";
        }

        public object Instance(TileConfiguration tile, Blob config) {
            return new ChargeableComponent(config);
        }
    }
}
