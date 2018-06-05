using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
