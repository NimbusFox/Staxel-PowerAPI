using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Components.Tiles;
using NimbusFox.PowerAPI.Items.Builders;
using NimbusFox.PowerAPI.TileEntities.Builders;
using NimbusFox.PowerAPI.TileEntities.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Entities;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Notifications;
using Staxel.Tiles;
using Staxel.Translation;

namespace NimbusFox.PowerAPI.Items {
    public class CableDrillItem : ChargeableItem {
        public CableDrillItem(ChargeableItemBuilder builder, ItemConfiguration config) : base(builder, config) { }
        private string _targetCode = "nimbusfox.powerapi.verb.none";
        private int _slot;

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (!main.DownClick && !alt.DownClick) {
                return;
            }

            if (main.DownClick) {
                if (entity.PlayerEntityLogic.LookingAtTile(out var target, out _)) {
                    if (facade.AnyInnerCableEntitiesInTile(target, out var entities)) {
                        foreach (var lEntity in entities) {
                            var itemBlob = BlobAllocator.Blob(true);
                            itemBlob.SetString("code", ((InnerCableTileEntityLogic)lEntity.Logic).Tile);
                            var item = GameContext.ItemDatabase.SpawnItemStack(itemBlob, null);

                            if (item.IsNull()) {
                                itemBlob.Delete("code");
                                itemBlob.SetString("kind", "staxel.item.Placer");
                                itemBlob.SetString("tile", ((InnerCableTileEntityLogic)lEntity.Logic).Tile);
                                item = GameContext.ItemDatabase.SpawnItemStack(itemBlob, null);
                            }

                            ItemEntityBuilder.SpawnDroppedItem(entity, facade, item, target.ToVector3D(), Vector3D.Zero,
                                Vector3D.Zero, SpawnDroppedFlags.AttemptPickup);

                            facade.RemoveEntity(lEntity.Id);
                        }
                    } else {
                        if (_targetCode == "nimbusfox.powerapi.verb.none") {
                            var notification = GameContext.NotificationDatabase.CreateNotificationFromCode(
                                "nimbusfox.powerapi.notifications.noCableSelected", facade.Step,
                                new NotificationParams(), true);

                            entity.PlayerEntityLogic.ShowNotification(notification);
                            return;
                        }

                        if (!entity.Inventory.HasItemWithCode(_targetCode)) {
                            var nparams = new NotificationParams(1);

                            nparams.SetTranslation(0, _targetCode + ".name");

                            var notification =
                                GameContext.NotificationDatabase.CreateNotificationFromCode(
                                    "nimbusfox.powerapi.notifications.noCable", facade.Step, nparams, true);

                            entity.PlayerEntityLogic.ShowNotification(notification);
                            return;
                        }


                        if (facade.ReadTile(target, TileAccessFlags.SynchronousWait, out var tile)) {
                            if (!tile.Configuration.Components.Select<ChargeableComponent>().Any()) {
                                var config = BlobAllocator.Blob(true);
                                config.SetString("tile", _targetCode);
                                InnerCableTileEntityBuilder.Spawn(target, config, facade);
                                entity.Inventory.ConsumeItem(new ItemStack(entity.Inventory.GetItemStack(_slot).Item, 1));
                            }
                        }
                    }
                }
            }

            if (alt.DownClick) {
                var orig = _targetCode;
                var looped = false;
                loop:
                if (_slot + 1 >= entity.Inventory.SlotCount()) {
                    _slot = 0;
                }

                for (var i = _slot; i < entity.Inventory.SlotCount(); i++) {
                    var current = entity.Inventory.GetItemStack(i);

                    if (current.IsNull()) {
                        continue;
                    }

                    if (current.Item.Configuration.Components.Select<CableComponent>().Any()) {
                        _slot = i;
                        _targetCode = current.Item.GetItemCode();
                        break;
                    }
                }

                if (orig == _targetCode) {
                    if (!looped) {
                        looped = true;
                        _slot = 0;
                        goto loop;
                    }

                    _targetCode = "nimbusfox.powerapi.verb.none";
                }
            }
            
            entity.Inventory.ItemStoreNeedsStorage();
        }

        public override bool TryResolveMainInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {

            verb = facade.AnyInnerCableEntitiesInTile(location, out _) ? "nimbusfox.powerapi.verb.extract"
                : "nimbusfox.powerapi.verb.insert";

            return true;
        }

        public override bool TryResolveAltInteractVerb(Entity entity, EntityUniverseFacade facade, Vector3I location,
            TileConfiguration lookedAtTile, out string verb) {
            verb = "nimbusfox.powerapi.verb.changeCable";
            return true;
        }

        public override bool Same(Item item) {
            var same = true;
            if (item is CableDrillItem drill) {
                same = drill._targetCode == _targetCode;
            }

            return same && base.Same(item);
        }

        public override void StorePersistenceData(Blob blob) {
            base.StorePersistenceData(blob);
            blob.SetString("targetCode", _targetCode);
            blob.SetLong("slot", _slot);
        }

        public override void Store(Blob blob) {
            base.Store(blob);
            blob.SetString("targetCode", _targetCode);
            blob.SetLong("slot", _slot);
        }

        public override void Restore(ItemConfiguration configuration, Blob blob) {
            base.Restore(configuration, blob);
            _targetCode = blob.GetString("targetCode", _targetCode);
            if (int.TryParse(blob.GetLong("slot", 0).ToString(), out var slot)) {
                _slot = slot;
            }
        }

        public override string GetItemDescription(LanguageDatabase lang) {
            var text = DescriptionCode != null ? lang.GetTranslationString(DescriptionCode) : base.GetItemDescription(lang);

            return string.Format(text, ItemPower.CurrentCharge.ToString("N0"), ItemPower.MaxCharge.ToString("N0"), ItemPower.TransferRate.In.ToString("N0"), ItemPower.TransferRate.Out.ToString("N0"),
                lang.GetTranslationString("nimbusfox.powerapi.perCycle"),
                lang.GetTranslationString("nimbusfox.powerapi.verb.power"),
                lang.GetTranslationString(_targetCode == "nimbusfox.powerapi.verb.none" ? _targetCode : _targetCode + ".name"));
        }
    }
}
