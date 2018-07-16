using NimbusFox.PowerAPI.Classes;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Interfaces {
    public interface ICycleRun {
        void RunCycle();
        Cycle Cycle { get; }
        Entity Entity { get; }
    }
}
