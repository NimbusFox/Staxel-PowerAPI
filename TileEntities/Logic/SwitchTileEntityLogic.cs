using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.TileStates.Logic;
using Plukit.Base;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Logic {
    public class SwitchTileEntityLogic : EntityLogic {
        public Entity Entity { get; }
        public Vector3I Location { get; private set; }

        public SwitchTileEntityLogic(Entity entity) {
            Entity = entity;
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            var state = entityUniverseFacade.FetchTileStateEntityLogic(Location, TileAccessFlags.SynchronousWait, out var tile) as SwitchTileStateEntityLogic;

            entityUniverseFacade.ForAllEntitiesInRange(Location.ToTileCenterVector3D(), 2, entity => {
                if (entity.Logic is InnerCableTileEntityLogic logic) {
                    logic.PowerState(state.On == tile.Configuration);
                }
            });
        }
        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }
        public override void Store() { }
        public override void Restore() { }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            Location = arguments.FetchBlob("location").GetVector3I();
        }
        public override void Bind() { }
        public override bool Interactable() {
            return false;
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }
        public override bool CanChangeActiveItem() {
            return false;
        }

        public override Heading Heading() {
            return new Heading();
        }

        public override bool IsPersistent() {
            return true;
        }

        public override void StorePersistenceData(Blob data) { }
        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) { }
        public override bool IsCollidable() {
            return false;
        }
    }
}
