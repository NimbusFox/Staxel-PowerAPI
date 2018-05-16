using System.Collections.Generic;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Items.Builders;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components {
    public class ChargeableComponent {
        public long MaxCharge { get; }
        public TransferRate TransferRate { get; }
        public Dictionary<int, string> ChargeModels { get; }
        public string DescriptionCode { get; }
        private Blob _blob;

        public ChargeableComponent(Blob config) {
            _blob = config;
            MaxCharge = config.GetLong("maxCharge", 300000);
            ChargeModels = new Dictionary<int, string>();
            TransferRate = new TransferRate();
            DescriptionCode = config.GetString("descriptionCode", null);

            if (!config.Contains("transferRate")) {
                TransferRate.In = 128;
                TransferRate.Out = 128;
            } else {
                if (config.KeyValueIteratable["transferRate"].Kind == BlobEntryKind.Int) {
                    var rate = config.GetLong("transferRate", 128);

                    TransferRate.In = rate;
                    TransferRate.Out = rate;
                } else if (config.KeyValueIteratable["transferRate"].Kind == BlobEntryKind.Blob) {
                    var rates = config.GetBlob("transferRate");

                    TransferRate.In = rates.GetLong("in", 128);
                    TransferRate.Out = rates.GetLong("out", 128);
                } else {
                    TransferRate.In = 128;
                    TransferRate.Out = 128;
                }
            }

            var blob = config.FetchBlob("chargeModels");

            foreach (var charge in blob.KeyValueIteratable) {
                if (charge.Value.Kind == BlobEntryKind.String) {

                    var text = charge.Key;

                    if (text.Contains("%")) {
                        text = text.Replace("%", "");
                    }

                    if (byte.TryParse(text, out var chargePercent)) {
                        if (!ChargeModels.ContainsKey(chargePercent)) {
                            ChargeModels.Add(chargePercent, charge.Value.GetString());
                        }
                    }
                }
            }
        }

        public Blob GetBlob() {
            return _blob;
        }
    }
}
