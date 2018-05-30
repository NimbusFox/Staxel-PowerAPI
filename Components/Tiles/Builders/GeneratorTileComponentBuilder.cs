using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
