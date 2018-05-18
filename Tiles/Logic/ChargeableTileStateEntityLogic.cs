using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components.Tiles;
using NimbusFox.PowerAPI.Tiles.Builders;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class ChargeableTileStateEntityLogic : TileStateEntityLogic {

        public Power TilePower {
            get {
                if (_tilePower.ContainsKey(Location)) {
                    return _tilePower[Location];
                }

                return null;
            }
            set {
                if (!_tilePower.ContainsKey(Location)) {
                    _tilePower.Add(Location, value);
                } else {
                    _tilePower[Location] = value;
                }
            }
        }

        private static readonly Dictionary<Vector3I, Power> _tilePower = new Dictionary<Vector3I, Power>();

        public ChargeableTileStateEntityLogic(Entity entity) : base(entity) {
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
        }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                if (tile.Configuration.Components.Contains<CableTileComponent>()) {
                    TilePower.GetPowerFromComponent(tile.Configuration.Components.Get<CableTileComponent>());
                }
            }
        }

        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            Location = arguments.FetchBlob("location").GetVector3I();
            if (TilePower == null) {
                TilePower = new Power(ModelUpdate);
            }
            if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                if (tile.Configuration.Components.Contains<CableTileComponent>()) {
                    TilePower.GetPowerFromComponent(tile.Configuration.Components.Get<CableTileComponent>());
                }
            }
        }
        public override void Bind() { }
        public override bool Interactable() {
            return false;
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }
        public override bool CanChangeActiveItem() {
            return false;
        }

        public override bool IsPersistent() {
            return false;
        }

        public override bool IsLingering() {
            return false;
        }

        public override void KeepAlive() { }
        public override void BeingLookedAt(Entity entity) { }
        public override bool IsBeingLookedAt() {
            return false;
        }

        public override void Store() {
            base.Store();

            if (TilePower != null) {
                Entity.Blob.SetLong("currentCharge", TilePower.CurrentCharge);
            }
        }

        public override void StorePersistenceData(Blob data) {
            base.StorePersistenceData(data);

            if (TilePower != null) {
                Entity.Blob.SetLong("currentCharge", TilePower.CurrentCharge);
            }
        }

        public override void Restore() {
            base.Restore();
            if (Entity.Blob.Contains("currentCharge")) {
                TilePower?.SetPower(Entity.Blob.GetLong("currentCharge"));
            }
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            base.RestoreFromPersistedData(data, facade);
            if (data.Contains("currentCharge")) {
                TilePower?.SetPower(data.GetLong("currentCharge"));
            }
        }

        private void ModelUpdate(bool update) {

        }
    }
}
