using Plukit.Base;

namespace NimbusFox.PowerAPI.Components.Tiles {
    public class SwitchTileComponent {
        public string On { get; }
        public string Off { get; }

        public SwitchTileComponent(Blob config) {
            On = config.GetString("on");
            Off = config.GetString("off");
        }
    }
}
