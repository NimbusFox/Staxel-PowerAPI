using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
//using NimbusFox.PowerAPI.Database;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Interfaces;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.TileStates.DockSites;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Docks;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.TileStates.Logic {
    public class ChargeableDockTileStateEntityLogic : DockTileStateEntityLogic, ITileWithPower {

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

        private bool _outputToTiles = false;

        private bool _inputFromTiles = true;

        private EntityUniverseFacade _universe;

        public bool OutputToTiles {
            get {
                if (TilePower.OutputToTiles) {
                    if (_outputToTiles) {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool InputFromTiles {
            get {
                if (TilePower.InputFromTiles) {
                    if (_inputFromTiles) {
                        return true;
                    }
                }

                return false;
            }
        }

        public void SetPower(long power) {
            TilePower.SetPower(power);

            //PowerDatabase.SetPower(Location, TilePower.CurrentCharge);
        }

        public long GetPower() {
            return TilePower.CurrentCharge;
        }

        public void AddPower(long power) {
            TilePower.AddPower(power);

            //PowerDatabase.SetPower(Location, TilePower.CurrentCharge);
        }

        public void RemovePower(long power) {
            TilePower.RemovePower(power);

            //PowerDatabase.SetPower(Location, TilePower.CurrentCharge);
        }

        public IReadOnlyList<DockSite> DockSites => _dockSites;

        public Cycle Cycle { get; } = new Cycle();

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

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade universe) {
            base.PreUpdate(timestep, universe);

            if (universe.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                if (tile.Configuration.Code == "staxel.tile.Sky") {
                    CycleHook.RemoveCycle(Location);
                    universe.RemoveEntity(Entity.Id);
                } else {
                    Cycle.RunCycle(RunCycle);
                }
            }
        }

        public override Vector3F InteractCursorColour() {
            return !HasAnyDockedItems() ? base.InteractCursorColour() : Constants.InteractCursorColour;
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            base.Construct(arguments, entityUniverseFacade);
            _universe = entityUniverseFacade;

            var tile = GameContext.TileDatabase.GetTileConfiguration(arguments.GetString("tile"));

            var component = tile.Components.Select<ChargeableComponent>().FirstOrDefault();

            if (component != default(ChargeableComponent)) {
                _outputToTiles = component.OutputToTiles;
                _inputFromTiles = component.InputFromTiles;
            }

            CycleHook.AddCycle(arguments.FetchBlob("location").GetVector3I());
        }

        public List<Power> GetAdjacentTiles(bool ignoreFull = false) {
            var surrounding = new List<Vector3I> {
                new Vector3I(Location.X + 1, Location.Y, Location.Z),
                new Vector3I(Location.X - 1, Location.Y, Location.Z),
                new Vector3I(Location.X, Location.Y + 1, Location.Z),
                new Vector3I(Location.X, Location.Y - 1, Location.Z),
                new Vector3I(Location.X, Location.Y, Location.Z + 1),
                new Vector3I(Location.X, Location.Y, Location.Z - 1)
            };

            var output = new List<Power>();

            foreach (var location in surrounding.ToArray()) {
                if (_universe.ReadTile(location, TileAccessFlags.SynchronousWait, out var tile)) {
                    if (tile.Configuration.Code == "staxel.tile.Sky") {
                        continue;
                    }

                    if (_universe.TryFetchTileStateEntityLogic(location, TileAccessFlags.SynchronousWait, out var logic)
                    ) {
                        if (logic is ITileWithPower chargeableDockLogic) {
                            if (ignoreFull) {
                                if (chargeableDockLogic.TilePower.CurrentCharge != chargeableDockLogic.TilePower.MaxCharge && chargeableDockLogic.InputFromTiles) {
                                    output.Add(chargeableDockLogic.TilePower);
                                }

                                continue;
                            }

                            if (chargeableDockLogic.InputFromTiles) {
                                output.Add(chargeableDockLogic.TilePower);
                            }
                            continue;
                        }
                    }
                }
            }

            return output;
        }

        public virtual void RunCycle() {
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
                var battery = (BatteryItem)item;
                if (battery.ItemPower.CurrentCharge == 0) {
                    continue;
                }

                var transfered = 0L;
                var toTransfer = battery.ItemPower.GetTransferOut();

                foreach (var item2 in chargeableDocks) {
                    var chargeable = (ChargeableItem)item2;

                    if (chargeable.ItemPower.CurrentCharge == chargeable.ItemPower.MaxCharge) {
                        continue;
                    }

                    var iToTransfer = chargeable.ItemPower.GetTransferIn(toTransfer - transfered);
                    var newCharge = chargeable.ItemPower.CurrentCharge + iToTransfer;

                    if (newCharge > chargeable.ItemPower.MaxCharge) {
                        transfered += chargeable.ItemPower.MaxCharge - chargeable.ItemPower.CurrentCharge;
                        chargeable.SetPower(chargeable.ItemPower.MaxCharge);
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
                        transfered += TilePower.MaxCharge - TilePower.CurrentCharge;
                        TilePower.SetPower(TilePower.MaxCharge);
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
                    var chargeable = (ChargeableItem)item;

                    if (chargeable.ItemPower.CurrentCharge != chargeable.ItemPower.MaxCharge) {
                        var iToTransfer = chargeable.ItemPower.GetTransferIn(toTransfer - transfered);
                        var newCharge = chargeable.ItemPower.CurrentCharge + toTransfer;

                        if (newCharge > chargeable.ItemPower.MaxCharge) {
                            transfered += chargeable.ItemPower.MaxCharge - chargeable.ItemPower.CurrentCharge;
                            chargeable.SetPower(chargeable.ItemPower.MaxCharge);
                        } else {
                            transfered = iToTransfer;
                            chargeable.SetPower(newCharge);
                        }

                        if (toTransfer == transfered) {
                            break;
                        }
                    }
                }

                if (transfered != toTransfer && OutputToTiles) {
                    var adjacentTiles = GetAdjacentTiles(true);

                    if (adjacentTiles.Any()) {
                        var iTotransfer = (long)Math.Floor((double)(toTransfer - transfered) / adjacentTiles.Count);

                        foreach (var tile in adjacentTiles) {
                            var newCharge =  tile.CurrentCharge + tile.GetTransferIn(iTotransfer);

                            if (newCharge > tile.MaxCharge) {
                                transfered += tile.MaxCharge - tile.CurrentCharge;
                                tile.SetPower(tile.MaxCharge);
                            } else {
                                transfered += tile.GetTransferIn(iTotransfer);
                                tile.SetPower(newCharge);
                            }

                            if (transfered == toTransfer) {
                                break;
                            }
                        }
                    }
                }



                TilePower.RemovePower(transfered);
            }
        }
    }
}
