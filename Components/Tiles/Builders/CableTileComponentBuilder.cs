using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    public class CableTileComponentBuilder : ITileComponentBuilder {
        public string Kind() {
            return "cable";
        }

        public object Instance(TileConfiguration tile, Blob config) {
            return new CableComponent(config);
        }
    }
}
