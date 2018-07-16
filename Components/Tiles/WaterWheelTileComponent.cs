using Plukit.Base;

namespace NimbusFox.PowerAPI.Components.Tiles {
    public class WaterWheelTileComponent {
        public int Efficency { get; }

        public WaterWheelTileComponent(Blob config) {
            if (int.TryParse(config.GetLong("efficency", 100).ToString(), out var amount)) {
                Efficency = amount;
            } else {
                Efficency = 100;
            }
        }
    }
}
