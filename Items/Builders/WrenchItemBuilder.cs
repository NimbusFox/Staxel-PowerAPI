using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Items;

namespace NimbusFox.PowerAPI.Items.Builders {
    public class WrenchItemBuilder : IItemBuilder, IDisposable {

        public ItemRenderer Renderer { get; private set; }

        public void Dispose() { }

        public void Load() {
            Renderer = new ItemRenderer();
        }
        public Item Build(Blob blob, ItemConfiguration configuration, Item spare) {
            if (spare is WrenchItem) {
                spare.Restore(configuration, blob);
                return spare;
            }

            var wrenchItem = new WrenchItem(this, configuration);
            wrenchItem.Restore(configuration, blob);
            return wrenchItem;
        }

        public string Kind() {
            return "nimbusfox.powerapi.item.wrench";
        }
    }
}
