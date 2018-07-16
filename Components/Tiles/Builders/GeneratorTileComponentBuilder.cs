using Plukit.Base;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Components.Tiles.Builders {
    public class GeneratorTileComponentBuilder : ITileComponentBuilder {
        public string Kind() {
            return "generator";
        }

        public object Instance(TileConfiguration tile, Blob config) {
            return new GeneratorComponent(config);
        }
    }
}
