using System;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Items;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Hooks {
    public class CycleHook : IModHookV3 {
        public void Dispose() { }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

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
                }
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
