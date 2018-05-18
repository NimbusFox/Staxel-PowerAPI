using Plukit.Base;
using Staxel.Items.ItemComponents;

namespace NimbusFox.PowerAPI.Components {
    public class WrenchableComponent : IToolable {

        public WrenchableComponent(Blob config) {
        }

        public string ToolCode() {
            return "nimbusfox.powerapi.item.wrench";
        }

        public string VerbTranslationCode() {
            return "nimbusfox.powerapi.needWrench";
        }
    }
}
