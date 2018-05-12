using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Tiles.Logic;
using NimbusFox.PowerAPI.Tiles.Painters;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Builders {
    public class GeneratorTileStateEntityBuilder : DockTileStateEntityBuilder, IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public new static string KindCode => "nimbusfox.powerapi.tileStateEntity.Generator";
        public new string Kind => GeneratorTileStateEntityBuilder.KindCode;

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new GeneratorTileStateEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new GeneratorTileStateEntityPainter(this);
        }

        public new static Entity Spawn(EntityUniverseFacade facade, Tile tile, Vector3I location) {
            var entity = new Entity(facade.AllocateNewEntityId(), false, KindCode, true);
            var blob = BlobAllocator.Blob(true);
            blob.SetString(nameof(tile), tile.Configuration.Code);
            blob.SetLong("variant", tile.Variant());
            blob.FetchBlob(nameof(location)).SetVector3I(location);
            blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);
            entity.Construct(blob, facade);
            facade.AddEntity(entity);
            return entity;
        }
    }
}
