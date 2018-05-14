using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Tiles.Logic;
using NimbusFox.PowerAPI.Tiles.Painters;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Builders {
    public class ChargeableTileStateEntityBuilder : DockTileStateEntityBuilder, IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public new static string KindCode => "nimbusfox.powerapi.tileStateEntity.Generator";
        public new string Kind => ChargeableTileStateEntityBuilder.KindCode;

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new ChargeableTileStateEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new ChargeableTileStateEntityPainter(this);
        }

        public new static Entity Spawn(EntityUniverseFacade facade, Tile tile, Vector3I location) {
            var entity = new Entity(facade.AllocateNewEntityId(), false, KindCode, true);

            entity.Construct(GetBaseBlob(tile, location), facade);
            facade.AddEntity(entity);
            return entity;
        }

        public static Blob GetBaseBlob(Tile tile, Vector3I location) {
            var blob = BlobAllocator.Blob(true);
            blob.SetString(nameof(tile), tile.Configuration.Code);
            blob.SetLong("variant", tile.Variant());
            blob.FetchBlob(nameof(location)).SetVector3I(location);
            blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);

            if (tile.Configuration.Components.Contains<ChargeableComponent>()) {
                var component = tile.Configuration.Components.Get<ChargeableComponent>();

                var current = blob.FetchBlob("chargeable");

                var transfer = current.FetchBlob("transfer");

                current.SetLong("maxWatts", component.MaxCharge);

                transfer.SetLong("in", component.TransferRate.In);
                transfer.SetLong("out", component.TransferRate.Out);

                current.SetString("descriptionCode", component.DescriptionCode);

                var chargeModels = current.FetchBlob("chargeModels");

                foreach (var model in component.ChargeModels) {
                    //chargeModels.SetString(model.Key.ToString(), model.Value);
                }
            }

            return blob;
        }
    }
}
