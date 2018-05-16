using Plukit.Base;
using Staxel.Items.ItemComponents;

namespace NimbusFox.PowerAPI.Components {
    public class WrenchableComponent : IToolable {
        public bool KeepCharge { get; }
        public bool KeepInventory { get; }

        public WrenchableComponent(Blob config) {
            KeepCharge = config.GetBool("keepCharge", true);
            KeepInventory = config.GetBool("keepInventory", false);
        }

        public string ToolCode() {
            return "nimbusfox.powerapi.item.wrench";
        }

        public string VerbTranslationCode() {
            return "nimbusfox.powerapi.needWrench";
        }
    }
}
