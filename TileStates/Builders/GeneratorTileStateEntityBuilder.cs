using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.TileStates.Logic;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.TileStates.Builders {
    public class GeneratorTileStateEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public static string KindCode => "nimbusfox.powerapi.tileStateEntity.generator";
        public string Kind => KindCode;

        public bool IsTileStateEntityKind() {
            return true;
        }

        public void Dispose() { }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new BasicTileStateEntityPainter();
        }

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new GeneratorTileStateEntityLogic(entity);
        }

        public void Load() {

        }

        public static Entity Spawn(Vector3I location, Tile tile, Universe facade) {
            var entity = new Entity(facade.AllocateNewEntityId(), false, KindCode, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("tile", tile.Configuration.Code);
            blob.SetLong("variant", tile.Variant());
            blob.FetchBlob("location").SetVector3I(location);
            blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);

            entity.Construct(blob, facade);

            facade.AddEntity(entity);

            return entity;
        }
    }
}
