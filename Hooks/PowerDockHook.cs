//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NimbusFox.PowerAPI.Classes;
//using NimbusFox.PowerAPI.Items;
//using NimbusFox.PowerAPI.Tiles.DockSites;
//using NimbusFox.PowerAPI.Tiles.Logic;
//using Plukit.Base;
//using Staxel.Items;
//using Staxel.Logic;
//using Staxel.Modding;
//using Staxel.Tiles;

//namespace NimbusFox.PowerAPI.Hooks {
//    public class PowerDockHook : IModHookV3 {

//        private static List<Vector3I> ChargeableDocks = new List<Vector3I>();

//        private Cycle _cycle = new Cycle();

//        private static bool PermitRemove = false;

//        public static void AddLocation(Vector3I location) {
//            if (!ChargeableDocks.Contains(location)) {
//                ChargeableDocks.Add(location);
//            }
//        }

//        public void Dispose() { }
//        public void GameContextInitializeInit() { }
//        public void GameContextInitializeBefore() { }
//        public void GameContextInitializeAfter() { }
//        public void GameContextDeinitialize() { }
//        public void GameContextReloadBefore() { }
//        public void GameContextReloadAfter() { }

//        public void UniverseUpdateBefore(Universe universe, Timestep step) {
//            if (!universe.Server) {
//                return;
//            }
//            _cycle.RunCycle(() => {
//                foreach (var location in ChargeableDocks.ToArray()) {
//                    if (universe.TryFetchTileStateEntityLogic(location, TileAccessFlags.SynchronousWait, out var logic)
//                    ) {
//                        if (logic is ChargeableDockTileStateEntityLogic dockLogic) {
//                            if (dockLogic.TilePower == null) {
//                                continue;
//                            }

//                            var batteryDocks = dockLogic.DockSites
//                                .Where(x => x is BatteryDockSite && !x.DockedItem.Stack.IsNull())
//                                .Select(x => x.DockedItem.Stack.Item).ToList();
//                            var chargeableDocks = dockLogic.DockSites
//                                .Where(x => x is ChargeableDockSite && x is BatteryDockSite == false &&
//                                            x is CapacitorDockSite == false && !x.DockedItem.Stack.IsNull())
//                                .Select(x => x.DockedItem.Stack.Item).ToList();

//                            PermitRemove = true;

//                            foreach (var item in batteryDocks) {
//                                var battery = (BatteryItem)item;
//                                if (battery.ItemPower.CurrentCharge != 0) {
//                                    var transfered = 0L;
//                                    var toTransfer = battery.ItemPower.GetTransferOut();
//                                    foreach (var item2 in chargeableDocks) {
//                                        var chargeable = (ChargeableItem)item2;
//                                        if (chargeable.ItemPower.CurrentCharge == chargeable.ItemPower.MaxCharge) {
//                                            continue;
//                                        }

//                                        var iToTransfer = chargeable.ItemPower.GetTransferIn(toTransfer - transfered);
//                                        var newCharge = chargeable.ItemPower.CurrentCharge + iToTransfer;
//                                        if (newCharge > chargeable.ItemPower.MaxCharge) {
//                                            chargeable.SetPower(chargeable.ItemPower.MaxCharge);
//                                            transfered += newCharge - chargeable.ItemPower.MaxCharge;
//                                        } else {
//                                            transfered = iToTransfer;
//                                            chargeable.SetPower(newCharge);
//                                        }

//                                        if (transfered == battery.ItemPower.TransferRate.Out) {
//                                            break;
//                                        }
//                                    }

//                                    if (transfered != battery.ItemPower.TransferRate.Out && dockLogic.TilePower != null) {
//                                        var iToTransfer = dockLogic.TilePower.GetTransferIn(toTransfer - transfered);
//                                        var newCharge = dockLogic.TilePower.CurrentCharge + iToTransfer;
//                                        if (newCharge > dockLogic.TilePower.MaxCharge) {
//                                            dockLogic.TilePower.SetPower(dockLogic.TilePower.MaxCharge);
//                                            transfered += newCharge - dockLogic.TilePower.MaxCharge;
//                                        } else {
//                                            transfered = iToTransfer;
//                                            dockLogic.TilePower.SetPower(newCharge);
//                                        }
//                                    }

//                                    battery.RemovePower(transfered);
//                                }
//                            }

//                            if (dockLogic.TilePower != null) {
//                                var tileTransfer = dockLogic.TilePower.GetTransferOut();
//                                var tileTransfered = 0L;

//                                foreach (var item in chargeableDocks) {
//                                    var chargeable = (ChargeableItem)item;
//                                    if (chargeable.ItemPower.CurrentCharge != chargeable.ItemPower.MaxCharge) {
//                                        var iToTransfer =
//                                            chargeable.ItemPower.GetTransferIn(tileTransfer - tileTransfered);
//                                        var newCharge = chargeable.ItemPower.CurrentCharge + iToTransfer;
//                                        if (newCharge > chargeable.ItemPower.MaxCharge) {
//                                            chargeable.SetPower(chargeable.ItemPower.MaxCharge);
//                                            tileTransfered += newCharge - chargeable.ItemPower.MaxCharge;
//                                        } else {
//                                            tileTransfered = iToTransfer;
//                                            chargeable.SetPower(newCharge);
//                                        }

//                                        if (tileTransfered == tileTransfer) {
//                                            break;
//                                        }
//                                    }
//                                }

//                                dockLogic.TilePower.RemovePower(tileTransfered);
//                            }
//                        }
//                    } else {
//                        if (PermitRemove) {
//                            ChargeableDocks.Remove(location);
//                        }
//                    }
//                }
//            });
//        }
//        public void UniverseUpdateAfter() { }
//        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
//            return true;
//        }

//        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
//            return true;
//        }

//        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
//            return true;
//        }

//        public void ClientContextInitializeInit() { }
//        public void ClientContextInitializeBefore() { }
//        public void ClientContextInitializeAfter() { }
//        public void ClientContextDeinitialize() { }
//        public void ClientContextReloadBefore() { }
//        public void ClientContextReloadAfter() { }
//        public void CleanupOldSession() { }
//        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
//            return true;
//        }

//        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
//            return true;
//        }
//    }
//}
