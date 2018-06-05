using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileEntities.Painters;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Builders {
    public class SolarPanelTileEntityBuilder : GeneratorTileEntityBuilder, IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public new static string KindCode => "nimbusfox.powerapi.entity.tile.generator.solar";

        public override string Kind => KindCode;

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new SolarPanelTileEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new SolarPanelTileEntityPainter();
        }
    }
}
