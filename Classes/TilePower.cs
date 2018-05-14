using Plukit.Base;

namespace NimbusFox.PowerAPI.Classes {
    public class TilePower {
        public TransferRate TransferRate { get; }
        public long CurrentWatts { get; internal set; }
        public long MaxWatts { get; private set; }

        public TilePower() {
            TransferRate = new TransferRate {
                Out = 128,
                In = 128
            };

            CurrentWatts = 0;
            
            MaxWatts = 300000;
        }

        public static TilePower GetTilePowerFromBlob(Blob blob) {
            var defaults = new TilePower();

            if (blob.Contains("transferRate")) {
                var transferRate = blob.FetchBlob("transferRate");

                defaults.TransferRate.Out = transferRate.GetLong("out", 128);
                defaults.TransferRate.In = transferRate.GetLong("in", 128);
            }

            defaults.MaxWatts = blob.GetLong("maxWatts", 30000);

            return defaults;
        }
    }
}
