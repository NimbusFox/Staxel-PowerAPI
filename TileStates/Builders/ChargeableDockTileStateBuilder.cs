using System;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.TileStates.Builders {
    public class ChargeableDockTileStateBuilder : ITileStateBuilder, IDisposable {
        public void Dispose() { }
        public void Load() { }
        public string Kind() {
            return KindCode();
        }

        public Entity Instance(Vector3I location, Tile tile, Universe universe) {
            return ChargeableDockTileStateEntityBuilder.Spawn(universe, tile, location);
        }

        public static string KindCode() {
            return "nimbusfox.powerapi.tileState.dockChargeable";
        }
    }
}
