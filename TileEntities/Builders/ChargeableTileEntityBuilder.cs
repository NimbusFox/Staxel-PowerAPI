using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileEntities.Painters;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Builders {
    public class ChargeableTileEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public virtual string Kind => KindCode;

        public static string KindCode => "nimbusfox.powerapi.entity.tile.chargeable";

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new ChargeableTileEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new ChargeableTileEntityPainter();
        }

        public static Entity Spawn(Vector3I position, Blob config, EntityUniverseFacade universe, string kind = "nimbusfox.powerapi.entity.tile.chargeable") {
            var entity = new Entity(universe.AllocateNewEntityId(), false, kind, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("kind", kind);
            blob.FetchBlob("position").SetVector3D(position.ToTileCenterVector3D());
            blob.FetchBlob("location").SetVector3I(position);
            blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);
            blob.FetchBlob("config").MergeFrom(config);

            entity.Construct(blob, universe);

            universe.AddEntity(entity);

            return entity;
        }

        public virtual void Load() { }

        public virtual bool IsTileStateEntityKind() {
            return false;
        }
    }
}
