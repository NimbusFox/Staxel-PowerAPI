using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Tiles.DockSites;
using Plukit.Base;
using Staxel.Core;
using Staxel.Docks;
using Staxel.Logic;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class ChargeableTileStateEntityLogic : DockTileStateEntityLogic {

        public readonly TilePower TilePower;

        internal bool RunTick = true;

        public ChargeableTileStateEntityLogic(Entity entity) : base(entity) {
            TilePower = TilePower.GetTilePowerFromBlob(entity.Blob.FetchBlob("chargeable"));
        }

        protected override void AddSite(DockSiteConfiguration config) {
            _dockSites.Add(new ChargeableDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
        }

        public override Vector3F InteractCursorColour() {
            return !HasAnyDockedItems() ? base.InteractCursorColour() : Constants.InteractCursorColour;
        }

        public override void Update(Timestep timestep, EntityUniverseFacade universe) {
            if (RunTick) {

            }
            base.Update(timestep, universe);
        }

        public override void Store() {
            RunTick = false;
        }

        public override void Restore() {
            RunTick = true;
        }

        public override void StorePersistenceData(Blob blob) {
            base.StorePersistenceData(blob);

            blob.SetLong("currentWatts", TilePower.CurrentWatts);
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            base.RestoreFromPersistedData(data, facade);

            TilePower.CurrentWatts = data.GetLong("currentWatts", 0);
        }
    }
}
