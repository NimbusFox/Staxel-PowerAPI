﻿using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components.Tiles;
//using NimbusFox.PowerAPI.Database;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Interfaces;
using Plukit.Base;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class ChargeableTileEntityLogic : EntityLogic, ITileWithPower {

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

        public Power TilePower { get; private set; }

        public bool OutputToTiles => TilePower.OutputToTiles;

        public bool InputFromTiles => TilePower.InputFromTiles;

        public Cycle Cycle { get; } = new Cycle();
        public Entity Entity { get; }
        public Vector3I Location { get; private set; }

        private EntityUniverseFacade _universe;

        public ChargeableTileEntityLogic(Entity entity) {
            Entity = entity;
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
        }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                if (tile.Configuration.Components.Contains<CableTileComponent>()) {
                    TilePower.GetPowerFromComponent(tile.Configuration.Components.Get<CableTileComponent>());
                }
            }

            Cycle.RunCycle(RunCycle);
        }

        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            Location = arguments.FetchBlob("location").GetVector3I();
            _universe = entityUniverseFacade;
            if (TilePower == null) {
                TilePower = new Power(ModelUpdate);
                if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                    if (tile.Configuration.Components.Contains<CableTileComponent>()) {
                        TilePower.GetPowerFromComponent(tile.Configuration.Components.Get<CableTileComponent>());
                    }
                }
            }
            CycleHook.AddCycle(Location);
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
            return default;
        }

        public override bool IsPersistent() {
            return true;
        }

        private void ModelUpdate(bool update) {
        }

        public override void Store() {
            Entity.Blob.SetLong("currentCharge", TilePower.CurrentCharge);
        }

        public override void Restore() {
            TilePower.SetPower(Entity.Blob.GetLong("currentCharge", 0));
        }

        public override void StorePersistenceData(Blob data) {
            data.SetLong("currentCharge", TilePower.CurrentCharge);
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            TilePower.SetPower(data.GetLong("currentCharge", 0));
        }

        public override bool IsCollidable() {
            return false;
        }

        public List<ITileWithPower> GetAdjacentTiles(bool ignoreFull = false) {
            var surrounding = new List<Vector3I> {
                new Vector3I(Location.X + 1, Location.Y, Location.Z),
                new Vector3I(Location.X - 1, Location.Y, Location.Z),
                new Vector3I(Location.X, Location.Y + 1, Location.Z),
                new Vector3I(Location.X, Location.Y - 1, Location.Z),
                new Vector3I(Location.X, Location.Y, Location.Z + 1),
                new Vector3I(Location.X, Location.Y, Location.Z - 1)
            };

            var output = new List<ITileWithPower>();

            foreach (var location in surrounding.ToArray()) {
                if (_universe.ReadTile(location, TileAccessFlags.SynchronousWait, out var tile)) {
                    if (tile.Configuration.Code == "staxel.tile.Sky" || !tile.Configuration.Components.Select<CableTileComponent>().Any()) {
                        continue;
                    }

                    var logic = _universe.FetchTileStateEntityLogic(location, TileAccessFlags.SynchronousWait).GetPowerForTile(_universe);
                    if (logic != null) {
                        if (ignoreFull) {
                            if (logic.GetPower() != logic.TilePower?.MaxCharge && logic.InputFromTiles) {
                                output.Add(logic);
                            }

                            continue;
                        }

                        if (logic.InputFromTiles) {
                            output.Add(logic);
                        }
                        continue;
                    }
                }
            }

            return output;
        }

        public virtual void RunCycle() {
            if (TilePower == null) {
                return;
            }

            if (!OutputToTiles) {
                return;
            }

            var adjacentTiles = GetAdjacentTiles(true);

            if (adjacentTiles.Any()) {
                var toTransfer = (long)Math.Floor((double)TilePower.GetTransferOut() / adjacentTiles.Count);
                var transfered = 0L;

                foreach (var tile in adjacentTiles) {
                    var newCharge = tile.TilePower.GetTransferIn(toTransfer) + tile.GetPower();

                    if (newCharge > tile.TilePower.MaxCharge) {
                        transfered += tile.TilePower.MaxCharge - tile.GetPower();
                        tile.SetPower(tile.TilePower.MaxCharge);
                    } else {
                        transfered += tile.TilePower.GetTransferIn(toTransfer);
                        tile.SetPower(newCharge);
                    }

                    if (transfered == TilePower.GetTransferOut()) {
                        break;
                    }
                }

                TilePower.RemovePower(transfered);
            }
        }
    }
}
