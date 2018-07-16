using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    public class SwitchTileComponentBuilder : ITileComponentBuilder {
        public string Kind() {
            return "switch";
        }

        public object Instance(TileConfiguration tile, Blob config) {
            return new SwitchTileComponent(config);
        }
    }
}
