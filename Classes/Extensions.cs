using System.Collections.Generic;
using NimbusFox.PowerAPI.Items;
using Staxel.Items;
using Staxel.Logic;

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
                            if (chargeable is BatteryItem == false) {
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
                            if (chargeable is BatteryItem == false) {
                                list.Add(chargeable);
                            }
                        }
                    }
                }
            }

            return list;
        }
    }
}
