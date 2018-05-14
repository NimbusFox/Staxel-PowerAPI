using Plukit.Base;
using Staxel.Draw;

namespace NimbusFox.PowerAPI.Classes {
    public class ChargeableItemIcons {
        public Drawable Icon { get; set; }
        public Drawable SecondaryIcon { get; set; }
        public MatrixDrawable InHandIcon { get; set; }
        public Shape CollisionBox { get; set; }
        public Drawable CompactDrawable { get; set; }
    }
}
