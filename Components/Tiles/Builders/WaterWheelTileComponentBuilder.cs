using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    class WaterWheelTileComponentBuilder : ITileComponentBuilder {
        public string Kind() {
            return "waterWheel";
        }

        public object Instance(TileConfiguration tile, Blob config) {
            return new WaterWheelTileComponent(config);
        }
    }
}
