using System.Linq;
using NimbusFox.PowerAPI.Classes;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Hooks {
    public class BatteryInventoryHook : IModHookV3 {
        public void Dispose() { }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        private readonly Cycle _cycle = new Cycle();

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            _cycle.RunCycle(() => {
                if (universe.Server) {
                    var players = new Lyst<Entity>();

                    universe.GetPlayers(players);

                    foreach (var player in players) {
                        var batteries = player.Inventory.GetBatteries();
                        var chargeables = player.Inventory.GetChargeables();

                        foreach (var battery in batteries.Where(x => x.ChargeInventory && x.CurrentCharge != 0)) {

                            var transfered = 0L;
                            var toTransfer = battery.GetTransferOut();
                            foreach (var chargeable in chargeables.Where(x => x.CurrentCharge != x.MaxCharge)) {
                                var iToTransfer = chargeable.GetTransferIn(toTransfer - transfered);
                                var newCharge = chargeable.CurrentCharge + iToTransfer;
                                if (newCharge > chargeable.MaxCharge) {
                                    chargeable.SetPower(chargeable.MaxCharge);
                                    transfered += newCharge - chargeable.MaxCharge;
                                } else {
                                    transfered = iToTransfer;
                                    chargeable.SetPower(newCharge);
                                }

                                if (transfered == battery.TransferRate.Out) {
                                    break;
                                }
                            }

                            battery.RemovePower(transfered);
                        }
                    }
                }
            });
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
