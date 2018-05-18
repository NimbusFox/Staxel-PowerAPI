using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Tiles.Logic;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Builders {
    public class ChargeableTileStateEntityBuilder : DockTileStateEntityBuilder, IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public new static string KindCode => "nimbusfox.powerapi.tileStateEntity.Chargeable";
        public new string Kind => ChargeableTileStateEntityBuilder.KindCode;

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new ChargeableTileStateEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new DockTileStateEntityPainter(this);
        }

        public new static Entity Spawn(EntityUniverseFacade facade, Tile tile, Vector3I location) {
            var entity = new Entity(facade.AllocateNewEntityId(), false, KindCode, true);

            entity.Construct(GetBaseBlob(tile, location), facade);

            facade.AddEntity(entity);

            PowerDockHook.AddLocation(location);
            
            return entity;
        }

        public static Blob GetBaseBlob(Tile tile, Vector3I location) {
            var blob = BlobAllocator.Blob(true);
            blob.SetString("tile", tile.Configuration.Code);
            blob.SetLong("variant", tile.Variant());
            blob.FetchBlob("location").SetVector3I(location);
            blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);

            return blob;
        }
    }
}
