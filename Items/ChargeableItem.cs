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
        internal long MaxCharge { get; private set; }

        private long _currentCharge;

        public bool RunOnUpdateSecond { get; internal set; }

        internal long CurrentCharge {
            get => _currentCharge;
            private set {
                _currentCharge = value;
                SanityCheck();
            }
        }

        private Dictionary<int, ItemConfiguration> _chargeModels;
        internal TransferRate TransferRate { get; private set; }
        private string DescriptionCode { get; set; }

        private static bool _createChildren = true;

        public ChargeableItem(ChargeableItemBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            _builder = builder;
            Configuration = config;
            _chargeModels = new Dictionary<int, ItemConfiguration>();

            if (HasToolComponent(Configuration.Components)) {
                var component = Configuration.Components.Contains<BatteryComponent>() ? Configuration.Components.Get<BatteryComponent>() : Configuration.Components.Get<ChargeableComponent>();

                MaxCharge = component.MaxCharge;
                TransferRate = component.TransferRate;
                DescriptionCode = component.DescriptionCode;
                var chargeModels = component.ChargeModels;

                if (_createChildren) {
                    _createChildren = false;
                    var configs = GameContext.ItemDatabase.GetConfigsByKind(ChargeableItemBuilder.KindCode()).ToList();
                    foreach (var model in chargeModels) {
                        if (configs.Any(x => x.Value.Code == model.Value)) {
                            _chargeModels.Add(model.Key, configs.First(x => x.Value.Code == model.Value).Value);
                        }
                    }

                    _createChildren = true;
                }
            }
        }

        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) {
        }
        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }

        protected override void AssignFrom(Item item) {
            if (item is ChargeableItem charge) {
                MaxCharge = charge.MaxCharge;
                CurrentCharge = charge.CurrentCharge;
                _chargeModels = charge._chargeModels;
                TransferRate = charge.TransferRate;
                DescriptionCode = charge.DescriptionCode;
            }
        }

        public long GetTransferIn(long incoming) {
            return incoming > TransferRate.In ? TransferRate.In : incoming;
        }

        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        private bool HasToolComponent(Plukit.Base.Components components) {
            return HasAssociatedToolComponent(components);
        }

        public override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return components.Contains<ChargeableComponent>() || components.Contains<BatteryComponent>();
        }

        public override void StorePersistenceData(Blob blob) {
            base.StorePersistenceData(blob);
            blob.SetLong("currentWatts", CurrentCharge);
        }

        public override void Store(Blob blob) {
            base.Store(blob);
            blob.SetLong("currentWatts", CurrentCharge);
        }

        public override void Restore(ItemConfiguration configuration, Blob blob) {
            base.Restore(configuration, blob);
            CurrentCharge = blob.GetLong("currentWatts", 0);

            UpdateModel(true);
        }

        public override string GetItemDescription(LanguageDatabase lang) {
            var text = lang.GetTranslationString(DescriptionCode);

            return string.Format(text, CurrentCharge, MaxCharge, TransferRate.In, TransferRate.Out,
                lang.GetTranslationString("nimbusfox.powerapi.perCycle"),
                lang.GetTranslationString("nimbusfox.powerapi.verb.plural"));
        }

        public override ItemRenderer FetchRenderer() {
            return _builder.Renderer;
        }

        public long AddPower(long value) {
            var output = value;
            var orig = CurrentCharge;
            var calc = CurrentCharge + value;

            if (calc > MaxCharge) {
                var diff = calc - MaxCharge;
                output -= diff;
            }

            CurrentCharge += value;

            UpdateModel(orig != CurrentCharge);

            if (orig != CurrentCharge) {
                RunOnUpdateSecond = true;
            }

            return output;
        }

        public long RemovePower(long value) {
            var output = value;
            var orig = CurrentCharge;
            var calc = CurrentCharge - value;

            if (calc < 0) {
                var diff = -calc;
                output -= diff;
            }

            CurrentCharge -= value;

            UpdateModel(orig != CurrentCharge);

            if (orig != CurrentCharge) {
                RunOnUpdateSecond = true;
            }

            return output;
        }

        public long SetPower(long value) {
            var orig = CurrentCharge;
            CurrentCharge = value;

            UpdateModel(orig != CurrentCharge);

            if (orig != CurrentCharge) {
                RunOnUpdateSecond = true;
            }

            return CurrentCharge;
        }

        private void SanityCheck() {
            if (CurrentCharge < 0) {
                CurrentCharge = 0;
            }
            
            if (CurrentCharge > MaxCharge) {
                CurrentCharge = MaxCharge;
            }
        }

        private void UpdateModel(bool update) {
            if (_chargeModels.Any() && update) {
                var selectedIcon = _chargeModels.First().Value;
                if (CurrentCharge != 0) {
                    var value = (double)CurrentCharge * 100 / MaxCharge;
                    var percentage = Math.Round(value, 0);

                    foreach (var model in _chargeModels) {
                        if (percentage >= model.Key) {
                            selectedIcon = model.Value;
                        }
                    }
                }

                Configuration = selectedIcon;
            }
        }

        public override bool Same(Item item) {
            if (item is ChargeableItem chargeable) {
                if (chargeable.CurrentCharge != CurrentCharge) {
                    return false;
                }
            }

            return item.GetItemCode() == GetItemCode();
        }
    }
}
