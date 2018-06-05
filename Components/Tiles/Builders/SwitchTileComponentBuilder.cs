using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
