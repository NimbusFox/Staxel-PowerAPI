﻿using NimbusFox.PowerAPI.TileEntities.Logic;
using NimbusFox.PowerAPI.TileEntities.Painters;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Builders {
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
