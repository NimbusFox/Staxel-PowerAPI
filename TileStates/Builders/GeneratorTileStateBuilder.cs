using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.TileStates.Builders {
    public class GeneratorTileStateBuilder : ITileStateBuilder, IDisposable {
        public void Dispose() { }
        public void Load() { }

        public string Kind() {
            return KindCode();
        }

        public Entity Instance(Vector3I location, Tile tile, Universe universe) {
            return GeneratorTileStateEntityBuilder.Spawn(location, tile, universe);
        }

        public static string KindCode() {
            return "nimbusfox.powerapi.tileState.generator";
        }
    }
}
