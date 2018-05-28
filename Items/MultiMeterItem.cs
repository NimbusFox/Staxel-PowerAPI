using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Items.Builders;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Notifications;
using Staxel.Tiles;
using System.Drawing;

namespace NimbusFox.PowerAPI.Items {
    public class MultiMeterItem : Item {

        private MultiMeterItemBuilder _builder;

        public MultiMeterItem(MultiMeterItemBuilder builder) : base(builder.Kind()) {
            _builder = builder;
        }
        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) { }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (!main.DownClick && !alt.DownClick) {
                return;
            }

            if (main.DownClick) {
                if (entity.PlayerEntityLogic.LookingAtTile(out var target, out _)) {
                    if (facade.ReadTile(target, TileAccessFlags.SynchronousWait, out var tile)) {
                        if (tile.Configuration.Components.Contains<WrenchableComponent>()) {
                            var logic = facade.FetchTileStateEntityLogic(target,
                                TileAccessFlags.SynchronousWait).GetPowerForTile(facade);

                            if (logic != null) {

                                var notificationParams = new NotificationParams(2);

                                notificationParams.SetString(0, $"{logic.TilePower?.CurrentCharge ?? 0:n0}");
                                notificationParams.SetString(1, $"{logic.TilePower?.MaxCharge ?? 0:n0}");

                                var notification =
                                    GameContext.NotificationDatabase.CreateNotificationFromCode(
                                        "nimbusfox.powerapi.notifications.powerInformation", facade.Step,
                                        notificationParams, true);

                                entity.PlayerEntityLogic?.ShowNotification(notification);
                            }
                        }
                    }
                }
            }

            if (alt.DownClick) {
                if (entity.PlayerEntityLogic.LookingAtTile(out var target, out _)) {
                    if (facade.ReadLighting(new Vector3I(target.X, target.Y + 1, target.Z),
                        TileAccessFlags.SynchronousWait, out var light, out _)) {
                        var notificationParams = new NotificationParams(3);
                        var color = Color.FromArgb(light.Lighting.R, light.Lighting.G, light.Lighting.B);

                        var efficiency = (int) Math.Floor((double) color.GetBrightness() * 100);

                        var phase = facade.DayNightCycle().Phase;

                        float test = 0;

                        if (phase < 0.4) {
                            test = (float)(0.4 / phase);
                            efficiency = (int)Math.Floor(efficiency / test);
                        }

                        if (phase > 0.6) {
                            test = (float)(phase / 0.6);
                            efficiency = (int)Math.Floor(efficiency / test);
                        }

                        if (facade.DayNightCycle().IsNight) {
                            efficiency = 0;
                        }

                        notificationParams.SetString(0, efficiency + "%");
                        notificationParams.SetFloat(1, phase);
                        notificationParams.SetFloat(2, test);


                        var notification = GameContext.NotificationDatabase.CreateNotificationFromCode(
                            "nimbusfox.powerapi.notifications.lightInformation", facade.Step, notificationParams, true);

                        entity.PlayerEntityLogic.ShowNotification(notification);
                    }
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
            if (lookedAtTile.Components.Contains<WrenchableComponent>()) {
                verb = "nimbusfox.powerapi.verb.powerCheck";
                return true;
            }

            verb = "";
            return false;
        }

        public override bool TryResolveAltInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {

            verb = "nimbusfox.powerapi.verb.lightCheck";
            return true;
        }

        public override ItemRenderer FetchRenderer() {
            return _builder.Renderer;
        }
    }
}
