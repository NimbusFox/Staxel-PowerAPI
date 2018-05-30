using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Items.Builders;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Notifications;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Items {
    public class MultiMeterItem : Item {

        private MultiMeterItemBuilder _builder;

        public MultiMeterItem(MultiMeterItemBuilder builder) : base(builder.Kind()) {
            _builder = builder;
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
            
        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (!main.DownClick) {
                return;
            }

            if (entity.PlayerEntityLogic.LookingAtTile(out var target, out _)) {
                if (facade.TryGetLightPower(target, out var efficiency, out var phase, out var reductions)) {
                    var notificationParams = new NotificationParams(3);

                    notificationParams.SetString(0, efficiency + "%");
                    notificationParams.SetFloat(1, phase);
                    notificationParams.SetFloat(2, reductions);


                    var notification = GameContext.NotificationDatabase.CreateNotificationFromCode(
                        "nimbusfox.powerapi.notifications.lightInformation", facade.Step, notificationParams, true);

                    entity.PlayerEntityLogic.ShowNotification(notification);
                }
            }
        }
        protected override void AssignFrom(Item item) { }
        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return false;
        }

        public override bool TryResolveMainInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {

            verb = "nimbusfox.powerapi.verb.lightCheck";
            return true;
        }

        public override ItemRenderer FetchRenderer() {
            return _builder.Renderer;
        }
    }
}
