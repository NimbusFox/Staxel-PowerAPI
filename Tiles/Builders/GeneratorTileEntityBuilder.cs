using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Tiles.Logic;
using NimbusFox.PowerAPI.Tiles.Painters;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Tiles.Builders {
    public class GeneratorTileEntityBuilder : ChargeableTileEntityBuilder, IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public new static string KindCode => "nimbusfox.powerapi.entity.tile.generator";

        public override string Kind => KindCode;

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new GeneratorTileEntityLogic(entity);
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return new ChargeableTileEntityPainter();
        }
    }
}
