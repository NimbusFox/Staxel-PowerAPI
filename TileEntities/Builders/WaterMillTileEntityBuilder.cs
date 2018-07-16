using NimbusFox.PowerAPI.TileEntities.Logic;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Builders {
    public class WaterMillTileEntityBuilder : GeneratorTileEntityBuilder, IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        public new static string KindCode => "nimbusfox.powerapi.entity.tile.generator.waterMill";

        public override string Kind => KindCode;

        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return new WaterMillTileEntityLogic(entity);
        }
    }
}
