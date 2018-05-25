using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Interfaces {
    public interface ICycleRun {
        void RunCycle();
        Cycle Cycle { get; }
        Entity Entity { get; }
    }
}
