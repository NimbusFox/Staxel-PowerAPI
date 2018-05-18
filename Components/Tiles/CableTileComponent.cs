﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.PowerAPI.Components.Tiles {
    public class CableTileComponent : ChargeableComponent {
        public CableTileComponent(Blob config) : base(config, 200, 20, 20) {
        }
    }
}