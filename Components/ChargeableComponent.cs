using System.Collections.Generic;
using System.Linq;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Items.Builders;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components {
    public class ChargeableComponent {
        public long MaxCharge { get; }
        public TransferRate TransferRate { get; }
        public Dictionary<int, string> ChargeModels { get; }
        public string DescriptionCode { get; }
        public virtual bool OutputToTiles { get; }
        public virtual bool InputFromTiles { get; }
        public string PowerVerb { get; }
        public IReadOnlyList<string> CompatiblePower { get; }
        private Blob _blob;

        public ChargeableComponent(Blob config, long maxCharge = 300000, long transferIn = 128, long transferOut = 128) {
            _blob = config;
            MaxCharge = config.GetLong("maxCharge", maxCharge);
            ChargeModels = new Dictionary<int, string>();
            TransferRate = new TransferRate();
            DescriptionCode = config.GetString("descriptionCode", null);
            OutputToTiles = config.GetBool("outputToTiles", false);
            InputFromTiles = config.GetBool("inputFromTiles", true);

            PowerVerb = config.GetString("powerVerb", "nimbusfox.powerapi.verb.power");

            try {
                CompatiblePower = config.GetStringList("compatiblePower").ToList();
            } catch {
                CompatiblePower = new List<string> {PowerVerb};
            }

            if (!config.Contains("transferRate")) {
                TransferRate.In = transferIn;
                TransferRate.Out = transferOut;
            } else {
                if (config.KeyValueIteratable["transferRate"].Kind == BlobEntryKind.Int) {
                    var rate = config.GetLong("transferRate", transferIn);

                    TransferRate.In = rate;
                    TransferRate.Out = rate;
                } else if (config.KeyValueIteratable["transferRate"].Kind == BlobEntryKind.Blob) {
                    var rates = config.GetBlob("transferRate");

                    TransferRate.In = rates.GetLong("in", transferIn);
                    TransferRate.Out = rates.GetLong("out", transferOut);
                } else {
                    TransferRate.In = transferIn;
                    TransferRate.Out = transferOut;
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
