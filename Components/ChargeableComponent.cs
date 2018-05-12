using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components {
    public class ChargeableComponent {
        public long MaxWatts { get; }
        public long TransferRate { get; }
        public Dictionary<string, string> ChargeModels { get; }

        public ChargeableComponent(Blob config) {
            MaxWatts = config.GetLong("maxWatts", 300000);
            ChargeModels = new Dictionary<string, string>();
            TransferRate = config.GetLong("transferRate", 128);

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
