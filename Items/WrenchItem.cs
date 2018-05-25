using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Items.Builders;
using NimbusFox.PowerAPI.TileStates.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Entities;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Notifications;
using Staxel.Tiles;
using Staxel.Translation;

namespace NimbusFox.PowerAPI.Items {
    public class WrenchItem : Item {

        private WrenchItemBuilder _builder;

        private enum Mode {
            PickUp = 0,
            Rotate = 1,
            Power = 2
        }

        private Mode _mode = Mode.PickUp;

        public WrenchItem(WrenchItemBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            _builder = builder;
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
            
        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (alt.DownClick) {
                if (_mode == Mode.PickUp) {
                    _mode = Mode.Power;
                } else {
                    _mode = Mode.PickUp;
                }

                entity.Inventory.ItemStoreNeedsStorage();
            }

            if (!main.DownClick) {
                return;
            }

            if (_mode == Mode.Rotate) {
                if (entity.PlayerEntityLogic.LookingAtTile(out var target, out _)) {
                    if (facade.ReadTile(target, TileAccessFlags.SynchronousWait, out var tile)) {
                        facade.PlaceTile(entity, target, tile.Configuration.MakeTile(tile.Configuration.BuildRotationVariant(tile.Configuration.Rotation(0))),
                            TileAccessFlags.SynchronousWait);
                    }
                }
            } else if (_mode == Mode.PickUp) {
                if (entity.PlayerEntityLogic.LookingAtTile(out var target, out _)) {
                    if (facade.ReadTile(target, TileAccessFlags.SynchronousWait, out var tile)) {
                        if (tile.Configuration.Components.Contains<WrenchableComponent>()) {

                            var destructor = DestructionEntityBuilder.Spawn(entity, target, facade, "");
                            destructor.AttemptPickup();
                            destructor.EnqueueDeferredDestructionQueue(facade.Step, target, tile);

                            //if (facade.TryFetchTileStateEntityLogic(target, TileAccessFlags.SynchronousWait,
                            //    out var logic)) {
                            //    if (logic is ChargeableTileStateEntityLogic chargeable) {
                            //        facade.RemoveEntity(chargeable.Entity.Id);
                            //    }
                            //}
                        }
                    }
                }
            } else if (_mode == Mode.Power) {
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

                            //if (logic is ChargeableTileStateEntityLogic tileLogic) {
                            //    var notificationParams = new NotificationParams(2);

                            //    notificationParams.SetString(0, $"{tileLogic.TilePower?.CurrentCharge ?? 0:n0}");
                            //    notificationParams.SetString(1, $"{tileLogic.TilePower?.MaxCharge ?? 0:n0}");

                            //    var notification =
                            //        GameContext.NotificationDatabase.CreateNotificationFromCode(
                            //            "nimbusfox.powerapi.notifications.powerInformation", facade.Step,
                            //            notificationParams, true);

                            //    entity.PlayerEntityLogic?.ShowNotification(notification);
                            //}
                        }
                    }
                }
            }
        }

        protected override void AssignFrom(Item item) {

        }
        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return false;
        }

        public override bool HasAltAction() {
            return true;
        }

        public override ItemRenderer FetchRenderer() {
            return _builder.Renderer;
        }

        public override bool TryResolveAltInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {

            verb = "nimbusfox.powerapi.verb.changeMode";

            return true;
        }

        public override bool TryResolveMainInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {

            if (lookedAtTile.Components.Contains<WrenchableComponent>()) {
                verb = GetVerb();
                return true;
            } else {
                if (_mode == Mode.Rotate) {
                    verb = "nimbusfox.powerapi.verb.rotate";
                    return true;
                }
            }

            return base.TryResolveMainInteractVerb(entity, facade, location, lookedAtTile, out verb);
        }

        public override void Store(Blob blob) {
            base.Store(blob);

            blob.SetLong("mode", (long)_mode);
        }

        public override void StorePersistenceData(Blob blob) {
            base.StorePersistenceData(blob);

            blob.SetLong("mode", (long)_mode);
        }

        public override void Restore(ItemConfiguration configuration, Blob blob) {
            base.Restore(configuration, blob);

            _mode = (Mode) blob.GetLong("mode", 0);
        }

        public override string GetItemDescription(LanguageDatabase lang) {
            var text = base.GetItemDescription(lang);

            if (text.Contains("{0}")) {
                text = string.Format(text, lang.GetTranslationString(GetVerb()));
            }

            return text;
        }

        private string GetVerb() {
            if (_mode == Mode.PickUp) {
                return "nimbusfox.powerapi.verb.pickUp";
            }
            if (_mode == Mode.Power) {
                return "nimbusfox.powerapi.verb.powerCheck";
            }

            return "nimbusfox.powerapi.verb.rotate";
        }
    }
}
