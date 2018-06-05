using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Interfaces;
using NimbusFox.PowerAPI.Items.Builders;
using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileStates.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Destruction;
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
            Rotate = 1
        }

        private Mode _mode = Mode.PickUp;

        public WrenchItem(WrenchItemBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            _builder = builder;
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
            
        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            //if (alt.DownClick) {
            //    if (_mode == Mode.PickUp) {
            //        _mode = Mode.Power;
            //    } else {
            //        _mode = Mode.PickUp;
            //    }

            //    entity.Inventory.ItemStoreNeedsStorage();
            //}

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

                            if (facade.TryFetchTileStateEntityLogic(target, TileAccessFlags.SynchronousWait,
                                out var logic)) {
                                if (logic is ChargeableTileStateEntityLogic chargeable) {
                                    if (facade.TryGetEntity(chargeable.GetOwner(), out var owner)) {
                                        if (owner.Logic is ITileWithPower tileWithPower) {
                                            tileWithPower.ActiveNameTag = false;
                                        }
                                        facade.RemoveEntity(chargeable.GetOwner());
                                    }

                                    facade.RemoveEntity(chargeable.Entity.Id);
                                }
                            }

                            var destructor = DestructionEntityBuilder.Spawn(entity, target, facade, "");
                            destructor.AttemptPickup();
                            destructor.EnqueueDeferredDestructionQueue(facade.Step, target, tile);
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

            return false;
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

            if ((int) _mode > 1) {
                _mode = Mode.PickUp;
            }
        }

        public override string GetItemDescription(LanguageDatabase lang) {
            var text = base.GetItemDescription(lang);

            if (text.Contains("{0}")) {
                text = string.Format(text, lang.GetTranslationString(GetVerb()));
            }

            return text;
        }

        private string GetVerb() {
            if (_mode == Mode.Rotate) {
                return "nimbusfox.powerapi.verb.rotate";
            }

            return "nimbusfox.powerapi.verb.pickUp";
        }
    }
}
