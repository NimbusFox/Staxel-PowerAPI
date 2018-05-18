using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Components.Tiles;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Classes {
    public class Power {
        public TransferRate TransferRate { get; private set; }
        private long _currentCharge;

        public byte ChargePercentage {
            get {
                var value = (double)CurrentCharge * 100 / MaxCharge;
                var percentage = (byte)Math.Round(value, 0);

                return percentage;
            }
        }

        public long CurrentCharge {
            get => _currentCharge;
            private set {
                _currentCharge = value;
                SanityCheck();
            }
        }
        public long MaxCharge { get; private set; }

        public IReadOnlyDictionary<int, string> Models { get; private set; }

        private Action<bool> _modelUpdate;

        public Power(Action<bool> modelUpdate) {
            TransferRate = new TransferRate {
                Out = 128,
                In = 128
            };

            CurrentCharge = 0;

            MaxCharge = 30000;

            _modelUpdate = modelUpdate;

            Models = new Dictionary<int, string>();
        }

        public static Power GetTilePowerFromBlob(Blob blob, Action<bool> modelUpdate) {
            var defaults = new Power(modelUpdate);

            if (blob.Contains("transferRate")) {
                var transferRate = blob.FetchBlob("transferRate");

                defaults.TransferRate.Out = transferRate.GetLong("out", 128);
                defaults.TransferRate.In = transferRate.GetLong("in", 128);
            }

            defaults.MaxCharge = blob.GetLong("maxCharge", 30000);

            if (blob.Contains("chargeModels")) {
                var models = new Dictionary<int, string>();

                foreach (var data in blob.FetchBlob("chargeModels").KeyValueIteratable) {
                    if (int.TryParse(data.Key, out var bit)) {
                        models.Add(bit, data.Value.GetString());
                    }
                }

                defaults.Models = models;
            }


            return defaults;
        }

        public void GetTilePowerFromBlob(Blob blob) {

            if (blob.Contains("transferRate")) {
                var transferRate = blob.FetchBlob("transferRate");

                TransferRate.Out = transferRate.GetLong("out", 128);
                TransferRate.In = transferRate.GetLong("in", 128);
            }

            MaxCharge = blob.GetLong("maxCharge", 30000);

            if (blob.Contains("chargeModels")) {
                var models = new Dictionary<int, string>();

                foreach (var data in blob.FetchBlob("chargeModels").KeyValueIteratable) {
                    if (int.TryParse(data.Key, out var bit)) {
                        models.Add(bit, data.Value.GetString());
                    }
                }

                Models = models;
            }
        }

        public static Power GetPowerFromComponent<T>(T component, Action<bool> modelUpdate) {
            if (component is ChargeableComponent chargeable) {
                var defaults = new Power(modelUpdate);

                defaults.TransferRate = chargeable.TransferRate;
                defaults.MaxCharge = chargeable.MaxCharge;
                defaults.Models = chargeable.ChargeModels;
            }

            return null;
        }

        public void GetPowerFromComponent<T>(T component) {
            if (component is CableTileComponent chargeable) {
                TransferRate = chargeable.TransferRate;
                MaxCharge = chargeable.MaxCharge;
                Models = chargeable.ChargeModels;
            }
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

            _modelUpdate?.Invoke(orig != CurrentCharge);

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

            _modelUpdate?.Invoke(orig != CurrentCharge);

            return output;
        }

        public long SetPower(long value) {
            var orig = CurrentCharge;
            CurrentCharge = value;

            _modelUpdate?.Invoke(orig != CurrentCharge);

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

        public long GetTransferIn(long incoming) {
            return incoming > TransferRate.In ? TransferRate.In : incoming;
        }

        public long GetTransferOut() {
            if (CurrentCharge >= TransferRate.Out) {
                return TransferRate.Out;
            }

            return CurrentCharge;
        }
    }
}
