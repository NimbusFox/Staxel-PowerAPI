using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components {
    public class ChargeableComponent {
        public long MaxWatts { get; }
        public TransferRate TransferRate { get; }
        public Dictionary<string, string> ChargeModels { get; }
        public string DescriptionCode { get; }

        public ChargeableComponent(Blob config) {
            MaxWatts = config.GetLong("maxWatts", 300000);
            ChargeModels = new Dictionary<string, string>();
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
                    if (!ChargeModels.ContainsKey(charge.Key)) {
                        ChargeModels.Add(charge.Key, charge.Value.GetString());
                    }
                }
            }
        }
    }
}
