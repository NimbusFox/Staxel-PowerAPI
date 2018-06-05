using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
