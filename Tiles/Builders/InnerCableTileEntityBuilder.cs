using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Tiles.Logic;
using NimbusFox.PowerAPI.Tiles.Painters;
using Plukit.Base;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Tiles.Builders {
    public class InnerCableTileEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public string Kind => "nimbusfox.powerapi.entity.tile.innercable";

        public static string KindCode => "nimbusfox.powerapi.entity.tile.innercable";

        EntityPainter IEntityPainterBuilder.Instance() {
            return new ChargeableTileEntityPainter();
        }

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new InnerCableTileEntityLogic(entity);
        }

        public bool IsTileStateEntityKind() {
            return false;
        }

        public void Load() {

        }

        public static Entity Spawn(Vector3I position, Blob config, EntityUniverseFacade universe) {
            var entity = new Entity(universe.AllocateNewEntityId(), false, KindCode, true);

            var blob = BlobAllocator.Blob(true);
            blob.SetString("kind", KindCode);
            blob.FetchBlob("position").SetVector3D(position.ToTileCenterVector3D());
            blob.FetchBlob("location").SetVector3I(position);
            blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);
            blob.SetString("tile", config.GetString("tile"));

            entity.Construct(blob, universe);

            universe.AddEntity(entity);

            return entity;
        }
    }
}
