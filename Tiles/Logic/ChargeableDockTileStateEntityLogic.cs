using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Interfaces;
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
    public class ChargeableDockTileStateEntityLogic : DockTileStateEntityLogic, ICycleRun {

        public Power TilePower {
            get {
                foreach (var dock in _dockSites) {
                    if (dock.DockedItem.Stack.Item is CapacitorItem capacitor) {
                        return capacitor.ItemPower;
                    }
                }

                return null;
            }
        }

        private Cycle _cycle = new Cycle();

        private EntityUniverseFacade _universe;

        public IReadOnlyList<DockSite> DockSites => _dockSites;

        public ChargeableDockTileStateEntityLogic(Entity entity) : base(entity) {
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

            if (config.SiteName == CapacitorDockSite.Name) {
                _dockSites.Add(new CapacitorDockSite(Entity, new DockSiteId(Entity.Id, _dockSites.Count), config));
                return;
            }
        }

        public override Vector3F InteractCursorColour() {
            return !HasAnyDockedItems() ? base.InteractCursorColour() : Constants.InteractCursorColour;
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            base.Construct(arguments, entityUniverseFacade);
            _universe = entityUniverseFacade;
            CycleHook.AddCycle(this);
        }

        public Dictionary<Tile, ChargeableDockTileStateEntityLogic> GetAdjacentTiles() {
            var surrounding = new List<Vector3I> {
                new Vector3I(Location.X + 1, Location.Y, Location.Z),
                new Vector3I(Location.X - 1, Location.Y, Location.Z),
                new Vector3I(Location.X, Location.Y + 1, Location.Z),
                new Vector3I(Location.X, Location.Y - 1, Location.Z),
                new Vector3I(Location.X, Location.Y, Location.Z + 1),
                new Vector3I(Location.X, Location.Y, Location.Z - 1)
            };

            var output = new Dictionary<Tile, ChargeableDockTileStateEntityLogic>();

            foreach (var location in surrounding.ToArray()) {
                if (_universe.ReadTile(location, TileAccessFlags.SynchronousWait, out var tile)) {
                    if (tile.Configuration.Code == "staxel.tile.Sky") {
                        surrounding.Remove(location);
                        continue;
                    }

                    if (_universe.TryFetchTileStateEntityLogic(location, TileAccessFlags.SynchronousWait, out var logic)
                    ) {
                        if (logic is ChargeableDockTileStateEntityLogic chargeableLogic) {
                            output.Add(tile, chargeableLogic);
                            continue;
                        }

                        surrounding.Remove(location);
                    }
                } else {
                    surrounding.Remove(location);
                }
            }

            return output;
        }

        public void RunCycle() {
            _cycle.RunCycle(() => {
                if (TilePower == null) {
                    return;
                }

                var batteryDocks = DockSites.Where(x => x is BatteryDockSite && !x.DockedItem.Stack.IsNull())
                    .Select(x => x.DockedItem.Stack.Item).ToList();

                var chargeableDocks = DockSites
                    .Where(x => x is ChargeableDockSite && x is BatteryDockSite == false &&
                                x is CapacitorDockSite == false && !x.DockedItem.Stack.IsNull())
                    .Select(x => x.DockedItem.Stack.Item).ToList();

                foreach (var item in batteryDocks) {
                    var battery = (BatteryItem) item;
                    if (battery.ItemPower.CurrentCharge == 0) {
                        continue;
                    }

                    var transfered = 0L;
                    var toTransfer = battery.ItemPower.GetTransferOut();

                    foreach (var item2 in chargeableDocks) {
                        var chargeable = (ChargeableItem) item2;

                        if (chargeable.ItemPower.CurrentCharge == chargeable.ItemPower.MaxCharge) {
                            continue;
                        }

                        var iToTransfer = chargeable.ItemPower.GetTransferIn(toTransfer - transfered);
                        var newCharge = chargeable.ItemPower.CurrentCharge + iToTransfer;

                        if (newCharge > chargeable.ItemPower.MaxCharge) {
                            chargeable.SetPower(chargeable.ItemPower.MaxCharge);
                            transfered += newCharge - chargeable.ItemPower.MaxCharge;
                        } else {
                            transfered = iToTransfer;
                            chargeable.SetPower(newCharge);
                        }

                        if (transfered == toTransfer) {
                            break;
                        }
                    }

                    if (transfered != toTransfer && TilePower != null) {
                        var iToTransfer = TilePower.GetTransferIn(toTransfer - transfered);
                        var newCharge = TilePower.CurrentCharge + iToTransfer;

                        if (newCharge > TilePower.MaxCharge) {
                            TilePower.SetPower(TilePower.MaxCharge);
                            transfered += newCharge - TilePower.MaxCharge;
                        } else {
                            transfered = iToTransfer;
                            TilePower.SetPower(newCharge);
                        }
                    }

                    battery.RemovePower(transfered);
                }

                if (TilePower != null) {
                    var toTransfer = TilePower.GetTransferOut();
                    var transfered = 0L;

                    foreach (var item in chargeableDocks) {
                        var chargeable = (ChargeableItem) item;

                        if (chargeable.ItemPower.CurrentCharge != chargeable.ItemPower.MaxCharge) {
                            var iToTransfer = chargeable.ItemPower.GetTransferIn(toTransfer - transfered);
                            var newCharge = chargeable.ItemPower.CurrentCharge + toTransfer;

                            if (newCharge > chargeable.ItemPower.MaxCharge) {
                                chargeable.SetPower(chargeable.ItemPower.MaxCharge);
                                transfered += newCharge - chargeable.ItemPower.MaxCharge;
                            } else {
                                transfered = iToTransfer;
                                chargeable.SetPower(newCharge);
                            }

                            if (toTransfer == transfered) {
                                break;
                            }
                        }
                    }

                    if (transfered != toTransfer && TilePower.OutputToTiles) {
                        var adjacentTiles = GetAdjacentTiles();

                        if (adjacentTiles.Any()) {

                        }
                    }

                    TilePower.RemovePower(transfered);
                }
            });
        }
    }
}
