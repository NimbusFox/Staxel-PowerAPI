using Plukit.Base;

namespace NimbusFox.PowerAPI.Components {
    public class GeneratorComponent : ChargeableComponent{
        public long GeneratePerCycle { get; }
        public override bool OutputToTiles { get; }
        public override bool InputFromTiles { get; }
        public string Type { get; }
        public GeneratorComponent(Blob config) : base(config) {
            OutputToTiles = config.GetBool("outputToTiles", true);
            InputFromTiles = config.GetBool("inputFromTiles", false);
            GeneratePerCycle = config.GetLong("generatePerCycle", 100);
            Type = config.GetString("type", "solar");
        }
    }
}
