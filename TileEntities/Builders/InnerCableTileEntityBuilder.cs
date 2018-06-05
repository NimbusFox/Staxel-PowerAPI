using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileEntities.Painters;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Builders {
    public class InnerCableTileEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public string Kind => "nimbusfox.powerapi.entity.tile.innercable";

        public static string KindCode => "nimbusfox.powerapi.entity.tile.innercable";

        EntityPainter IEntityPainterBuilder.Instance() {
            return new InnerCableTileEntityPainter();
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


            //var itemBlob = BlobAllocator.Blob(true);
            //itemBlob.SetString("kind", "staxel.item.Placer");
            //itemBlob.SetString("tile", config.GetString("tile"));
            //var item = GameContext.ItemDatabase.SpawnItem(itemBlob, null);
            //if (item == Item.NullItem) {
            //    return entity;
            //}

            //if (entity.Inventory == null) {
            //    var inventory = new EntityInventory(entity);
            //    entity.GetType().GetProperty("Inventory").GetSetMethod(true).Invoke(entity, new[] { inventory });
            //    var inventoryBlob = BlobAllocator.Blob(true);
            //    inventoryBlob.SetLong("index", 0);
            //    inventoryBlob.SetLong("swapIndex", 0);
            //    inventoryBlob.SetLong("revision", 0);
            //    entity.Inventory.RestoreFrom(inventoryBlob);
            //}

            //if (entity.Collections == null) {
            //    var collections = new EntityCollections(entity);
            //    entity.GetType().GetProperty("Collections").GetSetMethod(true).Invoke(entity, new[] { collections });
            //    var collectionBlob = BlobAllocator.Blob(true);
            //    collectionBlob.SetLong("version", 2L);
            //    collectionBlob.FetchBlob("items");
            //    collectionBlob.SetLong("count", 0L);
            //    entity.Collections.RestoreFrom(collectionBlob);
            //}

            //entity.Inventory.SetActiveIndex(0);
            //entity.Inventory.GiveItem(new ItemStack(item, 1));

            return entity;
        }
    }
}
