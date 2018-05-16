using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.Tiles.DockSites;
using Plukit.Base;
using Staxel.Core;
using Staxel.Docks;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class ChargeableTileStateEntityLogic : DockTileStateEntityLogic {

        public Power TilePower { get; }

        internal IReadOnlyList<DockSite> DockSites => _dockSites;

        private readonly bool _server;

        public ChargeableTileStateEntityLogic(Entity entity, bool server) : base(entity) {
            TilePower = new Power(UpdateModel);
            _server = server;
        }

        protected override void AddSite(DockSiteConfiguration config) {
            if (config.SiteName.StartsWith(BatteryDockSite.Name, StringComparison.CurrentCultureIgnoreCase)) {
                _dockSites.Add(new BatteryDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
                return;
            }

            if (config.SiteName.StartsWith(ChargeableDockSite.Name, StringComparison.CurrentCultureIgnoreCase)) {
                _dockSites.Add(new ChargeableDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
                return;
            }
        }

        public override Vector3F InteractCursorColour() {
            return !HasAnyDockedItems() ? base.InteractCursorColour() : Constants.InteractCursorColour;
        }

        public override void StorePersistenceData(Blob blob) {
            base.StorePersistenceData(blob);

            blob.SetLong("currentCharge", TilePower.CurrentCharge);
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            base.RestoreFromPersistedData(data, facade);

            TilePower.SetPower(data.GetLong("currentCharge", 0));
        }

        public override void Store() {
            base.Store();

            Entity.Blob.SetLong("currentCharge", TilePower.CurrentCharge);
        }

        public override void Restore() {
            if (!_server) {
                base.Restore();

                if (Entity.Blob.Contains("currentCharge")) {
                    TilePower.SetPower(Entity.Blob.GetLong("currentCharge"));
                }
            }
        }

        public override void UpdateWithData(Blob blob) {
            base.UpdateWithData(blob);

            blob.FetchBlob("logic").MergeFrom(LogicBlob());
        }

        private void UpdateModel(bool update) {
            TilePower.GetTilePowerFromBlob(Entity.Blob.FetchBlob("chargeable"));
            if (TilePower.Models.Any() && update) {
                var selectedModel = TilePower.Models.First().Value;
                if (TilePower.CurrentCharge != 0) {
                    foreach (var model in TilePower.Models) {
                        if (TilePower.ChargePercentage >= model.Key) {
                            selectedModel = model.Value;
                        }
                    }
                }

                if (LogicBlob().GetString("tile") != selectedModel) {
                    LogicBlob().SetString("tile", selectedModel);
                }
            }
        }

        private Blob LogicBlob() {
            return Entity.Blob.FetchBlob("logic");
        }
    }
}
