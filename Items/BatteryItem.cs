using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Items.Builders;
using Plukit.Base;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Items {
    public class BatteryItem : ChargeableItem {

        private readonly BatteryItemBuilder _builder;

        private bool _chargeInventory;

        public BatteryItem(BatteryItemBuilder builder, ItemConfiguration config) : base(builder, config) {
            _builder = builder;
            Configuration = config;

            if (HasAssociatedToolComponent(Configuration.Components)) {
                var component = Configuration.Components.Get<BatteryComponent>();
                _chargeInventory = component.ChargeInventory;
            }
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
            if (_chargeInventory) {
                var transfered = 0L;

                for (var i = 0; i < entity.Inventory.SlotCount(); i++) {
                    var currentStack = entity.Inventory.GetItemStack(i);

                    if (!currentStack.IsNull()) {
                        if (currentStack.SingularItem() != NullItem) {
                            var item = currentStack.SingularItem();

                            if (item is ChargeableItem chargeable) {
                                if (item is BatteryItem == false) {
                                    var newCharge = chargeable.CurrentWatts + (chargeable.TransferRate - transfered);
                                    if (newCharge > chargeable.MaxWatts) {
                                        chargeable.CurrentWatts = chargeable.MaxWatts;
                                        transfered += newCharge - chargeable.MaxWatts;
                                    } else {
                                        transfered = TransferRate;
                                        chargeable.CurrentWatts = newCharge;
                                    }
                                }
                            }
                        }
                    }

                    if (transfered == TransferRate) {
                        break;
                    }
                }
            }
        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }

        protected override void AssignFrom(Item item) {
            base.AssignFrom(item);

            if (item is BatteryItem battery) {
                _chargeInventory = battery._chargeInventory;
            }
        }

        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public sealed override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return components.Contains<BatteryComponent>();
        }
    }
}
