﻿using System;
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
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.Translation;

namespace NimbusFox.PowerAPI.Items {
    public class ChargeableItem : Item {

        private readonly ChargeableItemBuilder _builder;
        internal long MaxWatts { get; private set; }
        internal long CurrentWatts;
        private Dictionary<string, string> _chargeModels;
        internal TransferRate TransferRate { get; private set; }
        private string DescriptionCode { get; set; }

        public ChargeableItem(ChargeableItemBuilder builder, ItemConfiguration config) : base(builder.Kind()) {
            _builder = builder;
            Configuration = config;

            if (HasAssociatedToolComponent(Configuration.Components)) {
                var component = Configuration.Components.Get<ChargeableComponent>();

                MaxWatts = component.MaxWatts;
                _chargeModels = component.ChargeModels;
                TransferRate = component.TransferRate;
                DescriptionCode = component.DescriptionCode;
            }
        }
        public override void Update(Entity entity, Timestep step, EntityUniverseFacade entityUniverseFacade) { }
        public override void Control(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }

        protected override void AssignFrom(Item item) {
            if (item is ChargeableItem charge) {
                MaxWatts = charge.MaxWatts;
                CurrentWatts = charge.CurrentWatts;
                _chargeModels = charge._chargeModels;
                TransferRate = charge.TransferRate;
                DescriptionCode = charge.DescriptionCode;
            }
        }

        public long GetTransferIn(long incoming) {
            if (incoming > TransferRate.In) {
                return TransferRate.In;
            }

            return incoming;
        }

        public override bool PlacementTilePreview(AvatarController avatar, Entity entity, Universe universe, Vector3IMap<Tile> previews) {
            return false;
        }

        public override bool HasAssociatedToolComponent(Plukit.Base.Components components) {
            return components.Contains<ChargeableComponent>();
        }

        public override void StorePersistenceData(Blob blob) {
            blob.SetLong("currentWatts", CurrentWatts);
        }

        public override void Store(Blob blob) {
            blob.SetLong("currentWatts", CurrentWatts);
        }

        public override void Restore(ItemConfiguration configuration, Blob blob) {
            CurrentWatts = blob.GetLong("currentWatts", 0);
        }

        public override string GetItemDescription(LanguageDatabase lang) {
            var text = lang.GetTranslationString(DescriptionCode);

            return string.Format(text, CurrentWatts, MaxWatts, TransferRate.In, TransferRate.Out);
        }
    }
}
