using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Components;
using Plukit.Base;
using Staxel;
using Staxel.Entities;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.TileEntities.Logic {
    public class InnerCableTileEntityLogic : ChargeableTileEntityLogic {

        private bool _canTransferPower = true;

        public override bool OutputToTiles => _canTransferPower && base.OutputToTiles;

        public override bool InputFromTiles => _canTransferPower && base.InputFromTiles;

        public InnerCableTileEntityLogic(Entity entity) : base(entity) { }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            base.Construct(arguments, entityUniverseFacade);

            if (arguments.Contains("tile")) {
                Tile = arguments.GetString("tile");
            }
        }

        public override void Store() {
            base.Store();
            Entity.Blob.SetString("tile", Tile);
        }

        public override void Restore() {
            base.Restore();
            if (Entity.Blob.Contains("tile")) {
                Tile = Entity.Blob.GetString("tile");
            }
        }

        public override void StorePersistenceData(Blob data) {
            base.StorePersistenceData(data);

            data.SetString("tile", Tile);
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            base.RestoreFromPersistedData(data, facade);

            if (data.Contains("tile")) {
                Tile = data.GetString("tile");
            }
        }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (TilePower == null) {
                return;
            }

            Universe = entityUniverseFacade;

            if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                var config = GameContext.TileDatabase.GetTileConfiguration(Tile);
                if (config.Components.Select<ChargeableComponent>().Any() && tile.Configuration.Code != "staxel.tile.Sky") {
                    TilePower.GetPowerFromComponent(config.Components.Select<ChargeableComponent>().First());
                } else {
                    var itemBlob = BlobAllocator.Blob(true);
                    itemBlob.SetString("kind", "staxel.item.Placer");
                    itemBlob.SetString("tile", Tile);
                    var item = GameContext.ItemDatabase.SpawnItemStack(itemBlob, null);
                    ItemEntityBuilder.SpawnDroppedItem(Entity, entityUniverseFacade, item, Entity.Physics.Position, new Vector3D(0, 1, 0), Vector3D.Zero, SpawnDroppedFlags.None);
                    entityUniverseFacade.RemoveEntity(Entity.Id);
                    return;
                }
                Cycle.RunCycle(RunCycle);
            }
        }

        public void PowerState(bool transfer) {
            _canTransferPower = transfer;
        }
    }
}
