using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Interfaces;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class ChargeableTileEntityLogic : EntityLogic, ITileWithPower {

        public void SetPower(long power) {
            TilePower.SetPower(power);
        }

        public long GetPower() {
            return TilePower.CurrentCharge;
        }

        public void AddPower(long power) {
            TilePower.AddPower(power);
        }

        public void RemovePower(long power) {
            TilePower.RemovePower(power);
        }

        public void AddIgnore(Vector3I location) {
            if (!_ignoreInputs.Contains(location)) {
                _ignoreInputs.Add(location);
            }
        }

        private readonly List<Vector3I> _ignoreInputs = new List<Vector3I>();

        public Power TilePower { get; private set; }

        public bool OutputToTiles => TilePower.OutputToTiles;

        public bool InputFromTiles => TilePower.InputFromTiles;

        public Cycle Cycle { get; } = new Cycle();
        public Entity Entity { get; }
        public Vector3I Location { get; private set; }

        private EntityUniverseFacade _universe;

        private long _charge;

        public ChargeableTileEntityLogic(Entity entity) {
            Entity = entity;
            
        }

        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
        }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (TilePower == null) {
                return;
            }
            if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                if (tile.Configuration.Components.Contains<ChargeableComponent>()) {
                    TilePower.GetPowerFromComponent(tile.Configuration.Components.Get<ChargeableComponent>());
                }
            }

            if (_charge != 0) {
                TilePower.AddPower(_charge);
                _charge = 0;
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
                    if (tile.Configuration.Components.Contains<ChargeableComponent>()) {
                        TilePower.GetPowerFromComponent(tile.Configuration.Components.Get<ChargeableComponent>());
                    }
                }
            }

            if (arguments.Contains("charge")) {
                TilePower.SetPower(arguments.GetLong("charge"));
            }

            Entity.Physics.ForcedPosition(Location.ToTileCenterVector3D());
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
            if (TilePower != null) {
                Entity.Blob.SetLong("currentCharge", TilePower.CurrentCharge);
            }
        }

        public override void Restore() {
            if (TilePower == null) {
                TilePower = new Power(ModelUpdate);
                var logicBlob = Entity.Blob.FetchBlob("logic");
                var configBlob = logicBlob.FetchBlob("config");
                var tile = configBlob.GetString("tile", null);

                if (tile != null) {
                    var tileConfig = GameContext.TileDatabase.GetTileConfiguration(tile);

                    if (tileConfig.Components.Contains<ChargeableComponent>()) {
                        TilePower.GetPowerFromComponent(tileConfig.Components.Get<ChargeableComponent>());
                    }
                }
            }

            if (Entity.Blob.Contains("currentCharge")) {
                _charge = Entity.Blob.GetLong("currentCharge");
            }
        }

        public override void StorePersistenceData(Blob data) {
            if (TilePower != null) {
                data.SetLong("charge", TilePower.CurrentCharge);
            }
            data.FetchBlob("location").SetVector3I(Location);
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            Construct(data, facade);
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
                if (_ignoreInputs.Contains(location)) {
                    continue;
                }
                if (_universe.ReadTile(location, TileAccessFlags.SynchronousWait, out var tile)) {
                    if (tile.Configuration.Code == "staxel.tile.Sky" || !tile.Configuration.Components.Select<ChargeableComponent>().Any()) {
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

            if (TilePower.CurrentCharge == 0) {
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

                    tile.AddIgnore(Location);

                    if (transfered == TilePower.GetTransferOut()) {
                        break;
                    }
                }

                TilePower.RemovePower(transfered);
            }

            _ignoreInputs.Clear();
        }
    }
}
