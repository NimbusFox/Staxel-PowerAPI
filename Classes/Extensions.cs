using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using NimbusFox.PowerAPI.Interfaces;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileStates.Logic;
using Plukit.Base;
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

        public static bool TryGetLightPower(this EntityUniverseFacade facade, Vector3I location, out int efficiency, out float phase, out int reduction) {
            reduction = 0;
            phase = 0;
            efficiency = 0;

            var tileAccess = ClientContext.NameTagRenderer != null ? TileAccessFlags.None
                : TileAccessFlags.SynchronousWait;

            if (facade.ReadLighting(new Vector3I(location.X, location.Y + 1, location.Z),
                tileAccess, out var light, out _)) {
                var color = Color.FromArgb(light.Lighting.R, light.Lighting.G, light.Lighting.B);

                efficiency = (int) Math.Floor((double) color.GetBrightness() * 100);

                reduction = 100 - efficiency;

                phase = facade.DayNightCycle().Phase;

                if (phase < 0.4 || phase > 0.6) {
                    var test = (float) (0.5 - phase);
                    test = (float) (test / 0.5 * 100);
                    test = Math.Abs(test);
                    reduction += (int) Math.Floor(test);
                    efficiency = efficiency - reduction;
                }

                if (facade.DayNightCycle().IsNight || phase <= 0.1 || phase >= 0.9) {
                    efficiency = 0;
                }

                return true;
            }

            return false;
        }

        public static bool AnyInnerCableEntitiesInTile(this EntityUniverseFacade facade, Vector3I location, out Lyst<Entity> entities) {
            var output = false;

            var outputEntities = new Lyst<Entity>();

            facade.ForAllEntitiesInRange(location.ToTileCenterVector3D(), 1F, entity => {
                if (entity.Logic is InnerCableTileEntityLogic logic) {
                    if (logic.Location == location) {
                        output = true;
                        outputEntities.Add(entity);
                    }
                }
            });

            entities = outputEntities;

            return output;
        }
    }
}
