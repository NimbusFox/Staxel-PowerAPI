using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileEntities.Painters;
using Plukit.Base;
using Staxel.Logic;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.TileEntities.Builders {
    public class SwitchTileEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public virtual string Kind => KindCode;

        public static string KindCode => "nimbusfox.powerapi.entity.tile.switch";

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new SwitchTileEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new BasicTileStateEntityPainter();
        }

        public static Entity Spawn(Vector3I position, Blob config, EntityUniverseFacade universe) {
            var entity = new Entity(universe.AllocateNewEntityId(), false, KindCode, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("kind", KindCode);
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
