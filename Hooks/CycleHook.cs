using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Interfaces;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.Tiles.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Rendering;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Hooks {
    public class CycleHook : IModHookV3 {
        public void Dispose() {
        }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }

        public void GameContextInitializeAfter() {
        }

        public void GameContextDeinitialize() {
        }

        public void GameContextReloadBefore() {
        }

        public void GameContextReloadAfter() {

        }

        public static readonly List<Vector3I> Tags = new List<Vector3I>();

        private static readonly List<Vector3I> Cycles = new List<Vector3I>();

        public static void AddCycle(Vector3I cycleRun) {
            if (!Cycles.Contains(cycleRun)) {
                Cycles.Add(cycleRun);
            }
        }

        public static void RemoveCycle(Vector3I cycleRun) {
            if (!Cycles.Contains(cycleRun)) {
                Cycles.Remove(cycleRun);
            }
        }

        private DateTime _secondTick = DateTime.Now;

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            if (universe.Server) {
                if (universe.IsGamePaused()) {
                    Cycle.Pause();
                } else {
                    Cycle.Resume();

                    if (new TimeSpan(DateTime.Now.Ticks - _secondTick.Ticks).TotalSeconds > 1) {
                        var players = new Lyst<Entity>();

                        universe.GetPlayers(players);

                        foreach (var player in players) {
                            var runUpdate = false;
                            for (var i = 0; i < player.Inventory.SlotCount(); i++) {
                                var stack = player.Inventory.GetItemStack(i);

                                if (!stack.IsNull()) {
                                    if (stack.Item is ChargeableItem chargeable) {
                                        if (chargeable.RunOnUpdateSecond) {
                                            runUpdate = true;
                                            chargeable.RunOnUpdateSecond = false;
                                        }
                                    }
                                }
                            }

                            if (runUpdate) {
                                player.Inventory.ItemStoreNeedsStorage();
                            }
                        }

                        _secondTick = DateTime.Now;
                    }

                    //foreach (var location in new List<Vector3I>(Cycles)) {
                    //    if (universe.TryFetchTileStateEntityLogic(location, TileAccessFlags.OverrideUnbreakable, out var logic)) {
                    //        var logicParse = logic.GetPowerForTile(universe);
                    //        if (logicParse != null) {
                    //            logicParse.Cycle.RunCycle(logicParse.RunCycle);
                    //        } else {
                    //            Cycles.Remove(location);
                    //        }
                    //    }
                    //}
                }
            } else {
                var entities = new Lyst<Entity>();

                universe.GetPlayers(entities);

                Entity entity = null;

                foreach (var e in entities) {
                    if (ClientContext.PlayerFacade.IsLocalPlayer(e)) {
                        entity = e;
                    }
                }

                if (entity == null) {
                    return;
                }

                var tags = new List<Vector3I>();

                if (entity.Inventory.ActiveItem().Item is MultiMeterItem) {
                    universe.ForAllEntitiesInRange(entity.FeetLocation(), 5, tile => {
                        if (tile.Logic is ChargeableTileEntityLogic logic) {
                            if (!tags.Contains(logic.Location)) {
                                tags.Add(logic.Location);
                            }
                        }
                    });
                }

                Tags.Clear();
                Tags.AddRange(tags);
            }
        }
        public void UniverseUpdateAfter() { }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            return true;
        }

        public void ClientContextInitializeInit() { }
        public void ClientContextInitializeBefore() { }
        public void ClientContextInitializeAfter() { }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }
        public void CleanupOldSession() { }
        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
            return true;
        }

        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
            return true;
        }
    }
}
