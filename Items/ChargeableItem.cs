using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Components.Items;
using NimbusFox.PowerAPI.Items.Builders;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Collections;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.Translation;

namespace NimbusFox.PowerAPI.Items {
    public class ChargeableItem : Item {

        private readonly ChargeableItemBuilder _builder;

        public bool RunOnUpdateSecond { get; internal set; }

        internal Power ItemPower;

        private Dictionary<int, ItemConfiguration> _chargeModels;
        private string DescriptionCode { get; set; }

        public ChargeableItem(ChargeableItemBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            _builder = builder;
            Configuration = config;
            _chargeModels = new Dictionary<int, ItemConfiguration>();
            ItemPower = new Power(UpdateModel);

            if (HasToolComponent(Configuration.Components)) {
                var component = Configuration.Components.Select<ChargeableComponent>().First();
                
                ItemPower = Power.GetTilePowerFromBlob(component.GetBlob(), UpdateModel);
                DescriptionCode = component.DescriptionCode;
                var chargeModels = component.ChargeModels;
                var configs = GameContext.ItemDatabase.GetConfigsByKind(_builder.Kind()).ToList();
                foreach (var model in chargeModels) {
                    if (configs.Any(x => x.Value.Code == model.Value)) {
                        _chargeModels.Add(model.Key, configs.First(x => x.Value.Code == model.Value).Value);
                    }
                }
            }
        }

        protected override void AssignFrom(Item item) {
            if (item is ChargeableItem charge) {
                ItemPower = charge.ItemPower;
                _chargeModels = charge._chargeModels;
                DescriptionCode = charge.DescriptionCode;
            }
        }

        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        private bool HasToolComponent(Plukit.Base.Components components) {
            return HasAssociatedToolComponent(components);
        }

        public override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return components.Select<ChargeableComponent>().Any();
        }

        public override void StorePersistenceData(Blob blob) {
            base.StorePersistenceData(blob);
            blob.SetLong("currentCharge", ItemPower.CurrentCharge);
        }

        public override void Store(Blob blob) {
            base.Store(blob);
            blob.SetLong("currentCharge", ItemPower.CurrentCharge);
        }

        public override void Restore(ItemConfiguration configuration, Blob blob) {
            base.Restore(configuration, blob);
            SetPower(blob.GetLong("currentCharge", 0));

            UpdateModel(true);
        }

        public override string GetItemDescription(LanguageDatabase lang) {
            var text = DescriptionCode != null ? lang.GetTranslationString(DescriptionCode) : base.GetItemDescription(lang);

            return string.Format(text, ItemPower.CurrentCharge, ItemPower.MaxCharge, ItemPower.TransferRate.In, ItemPower.TransferRate.Out,
                lang.GetTranslationString("nimbusfox.powerapi.perCycle"),
                lang.GetTranslationString("nimbusfox.powerapi.verb.plural"));
        }

        public override ItemRenderer FetchRenderer() {
            return _builder.Renderer;
        }

        public long AddPower(long value) {
            var orig = ItemPower.CurrentCharge;
            var output = ItemPower.AddPower(value);

            if (orig != ItemPower.CurrentCharge) {
                RunOnUpdateSecond = true;
            }

            return output;
        }

        public long RemovePower(long value) {
            var orig = ItemPower.CurrentCharge;
            var output = ItemPower.RemovePower(value);

            if (orig != ItemPower.CurrentCharge) {
                RunOnUpdateSecond = true;
            }

            return output;
        }

        public long SetPower(long value) {
            var orig = ItemPower.CurrentCharge;
            var output = ItemPower.SetPower(value);

            if (orig != ItemPower.CurrentCharge) {
                RunOnUpdateSecond = true;
            }

            return output;
        }

        private void UpdateModel(bool update) {
            if (_chargeModels.Any() && update) {
                var selectedIcon = _chargeModels.First().Value;
                if (ItemPower.CurrentCharge != 0) {
                    foreach (var model in _chargeModels) {
                        if (ItemPower.ChargePercentage >= model.Key) {
                            selectedIcon = model.Value;
                        }
                    }
                }

                Configuration = selectedIcon;
            }
        }

        public override bool Same(Item item) {
            if (item is ChargeableItem chargeable) {
                if (chargeable.ItemPower.CurrentCharge != ItemPower.CurrentCharge) {
                    return false;
                }
            }

            return item.GetItemCode() == GetItemCode();
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
        }

        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
        }
    }
}
