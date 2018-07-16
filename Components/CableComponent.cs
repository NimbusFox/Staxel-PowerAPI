﻿using Plukit.Base;

namespace NimbusFox.PowerAPI.Components.Tiles {
    public class CableComponent {
        public bool AbleToPlaceInTile { get; }
        public CableComponent(Blob config) {
            AbleToPlaceInTile = config.GetBool("ableToPlaceInTile", true);
        }
    }
}
