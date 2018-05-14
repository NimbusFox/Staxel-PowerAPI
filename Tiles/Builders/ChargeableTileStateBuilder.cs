using System;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.Tiles.Builders {
    public class ChargeableTileStateBuilder : ITileStateBuilder, IDisposable {
        public void Dispose() { }
        public void Load() { }
        public string Kind() {
            return KindCode();
        }

        public Entity Instance(Vector3I location, Tile tile, Universe universe) {
            return ChargeableTileStateEntityBuilder.Spawn(universe, tile, location);
        }

        public static string KindCode() {
            return "nimbusfox.powerapi.tileState.Generator";
        }
    }
}
