using System;
using System.Collections.Generic;
using System.Reflection;
using NimbusFox.PowerAPI.Interfaces;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.TileStates.Logic;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Player;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Classes {
    public static class Extensions {
        public static List<BatteryItem> GetBatteries(this EntityInventory inventory) {
            var list = new List<BatteryItem>();

            for (var i = 0; i < inventory.SlotCount(); i++) {
                var stack = inventory.GetItemStack(i);

                if (!stack.IsNull()) {
                    if (stack.Item != Item.NullItem) {
                        var item = stack.SingularItem();

                        if (item is BatteryItem battery) {
                            list.Add(battery);
                        }
                    }
                }
            }

            return list;
        }

        public static List<ChargeableItem> GetChargeables(this EntityInventory inventory) {
            var list = new List<ChargeableItem>();

            for (var i = 0; i < inventory.SlotCount(); i++) {
                var stack = inventory.GetItemStack(i);

                if (!stack.IsNull()) {
                    if (stack.Item != Item.NullItem) {
                        var item = stack.SingularItem();

                        if (item is ChargeableItem chargeable) {
                            if (chargeable is BatteryItem == false && chargeable is CapacitorItem == false) {
                                list.Add(chargeable);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public static List<BatteryItem> GetBatteries(this EntityInventoryItems inventory) {
            var list = new List<BatteryItem>();

            for (var i = 0; i < inventory.Slots.Length; i++) {
                var stack = inventory.GetItemStack(i);

                if (!stack.IsNull()) {
                    if (stack.Item != Item.NullItem) {
                        var item = stack.SingularItem();

                        if (item is BatteryItem battery) {
                            list.Add(battery);
                        }
                    }
                }
            }

            return list;
        }

        public static List<ChargeableItem> GetChargeables(this EntityInventoryItems inventory) {
            var list = new List<ChargeableItem>();

            for (var i = 0; i < inventory.Slots.Length; i++) {
                var stack = inventory.GetItemStack(i);

                if (!stack.IsNull()) {
                    if (stack.Item != Item.NullItem) {
                        var item = stack.SingularItem();

                        if (item is ChargeableItem chargeable) {
                            if (chargeable is BatteryItem == false && chargeable is CapacitorItem == false) {
                                list.Add(chargeable);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public static ITileWithPower GetPowerForTile(this object target, EntityUniverseFacade facade) {
            if (target is ITileWithPower power) {
                return power;
            }

            if (target is ChargeableTileStateEntityLogic logic) {
                var id = logic.GetOwner();

                if (id == EntityId.NullEntityId) {
                    return null;
                }

                if (facade.TryGetEntity(id, out var entity)) {
                    if (entity.Logic is ITileWithPower tilePower) {
                        return tilePower;
                    }
                }
            }

            return null;
        }
    }
}
