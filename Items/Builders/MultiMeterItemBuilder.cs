using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Items.Builders {
    public class MultiMeterItemBuilder : IItemBuilder, IDisposable {

        public ItemRenderer Renderer { get; private set; }

        public void Dispose() { }
        public void Load() {
            Renderer = new ItemRenderer();
        }
        public Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            if (spare is MultiMeterItem) {
                if (spare.Configuration != null) {
                    spare.Restore(configuration, blob);
                    return spare;
                }
            }

            var multiMeter = new MultiMeterItem(this);
            multiMeter.Restore(configuration, blob);
            return multiMeter;
        }

        public string Kind() {
            return KindCode();
        }

        public static string KindCode() => "nimbusfox.powerapi.item.multimeter";
    }
}
