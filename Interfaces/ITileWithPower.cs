using NimbusFox.PowerAPI.Classes;

namespace NimbusFox.PowerAPI.Interfaces {
    public interface ITileWithPower : ICycleRun {
        Power TilePower { get; }
        bool OutputToTiles { get; }
        bool InputFromTiles { get; }
        void SetPower(long power);
        long GetPower();
        void AddPower(long power);
        void RemovePower(long power);
    }
}