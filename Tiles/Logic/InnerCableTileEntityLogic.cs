using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Components;
using Plukit.Base;
using Staxel;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class InnerCableTileEntityLogic : ChargeableTileEntityLogic {
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

            var config = GameContext.TileDatabase.GetTileConfiguration(Tile);
            if (config.Components.Select<ChargeableComponent>().Any()) {
                TilePower.GetPowerFromComponent(config.Components.Select<ChargeableComponent>().First());
            } else {
                entityUniverseFacade.RemoveEntity(Entity.Id);
                return;
            }

            Cycle.RunCycle(RunCycle);
        }
    }
}
