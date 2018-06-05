using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Components.Tiles;
using Plukit.Base;
using Staxel.Items;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Logic {
    public class WaterMillTileEntityLogic : GeneratorTileEntityLogic {
        public WaterMillTileEntityLogic(Entity entity) : base(entity) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            var locations = new List<Vector3I> {
                new Vector3I(Location.X + 1, Location.Y, Location.Z),
                new Vector3I(Location.X - 1, Location.Y, Location.Z),
                new Vector3I(Location.X, Location.Y, Location.Z + 1),
                new Vector3I(Location.X, Location.Y, Location.Z - 1)
            };

            var efficiency = 0;

            foreach (var location in locations) {
                if (entityUniverseFacade.ReadTile(location, TileAccessFlags.SynchronousWait, out var tile)) {
                    var component = tile.Configuration.Components.Select<WaterWheelTileComponent>().FirstOrDefault();

                    if (component != default(WaterWheelTileComponent)) {
                        efficiency = component.Efficency;
                        break;
                    }
                }
            }

            base.Update(timestep, entityUniverseFacade, efficiency);
        }
    }
}
