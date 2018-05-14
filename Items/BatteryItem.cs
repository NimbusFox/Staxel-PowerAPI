using NimbusFox.PowerAPI.Components.Items;
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

        public bool ChargeInventory { get; private set; }

        public BatteryItem(BatteryItemBuilder builder, ItemConfiguration config) : base(builder, config) {
            _builder = builder;
            Configuration = config;

            if (HasAssociatedToolComponent(Configuration.Components)) {
                var component = Configuration.Components.Get<BatteryComponent>();
                ChargeInventory = component.ChargeInventory;
            }
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
        }

        public long GetTransferOut() {
            if (CurrentCharge >= TransferRate.Out) {
                return TransferRate.Out;
            }

            return CurrentCharge;
        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }

        protected override void AssignFrom(Item item) {
            base.AssignFrom(item);

            if (item is BatteryItem battery) {
                ChargeInventory = battery.ChargeInventory;
            }
        }

        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public sealed override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return components.Contains<BatteryComponent>();
        }

        public override ItemRenderer FetchRenderer() {
            return _builder.Renderer;
        }
    }
}
