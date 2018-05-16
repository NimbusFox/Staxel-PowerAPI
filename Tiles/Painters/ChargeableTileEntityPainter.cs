using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Tiles.Builders;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Rendering;
using Staxel.Tiles;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Painters {
    class ChargeableTileEntityPainter : EntityPainter {
        private EffectRenderer _effectRenderer = Allocator.EffectRenderer.Allocate();

        protected override void Dispose(bool disposing) {
            if (!disposing || this._effectRenderer == null)
                return;
            this._effectRenderer.Dispose();
            Allocator.EffectRenderer.Release(ref this._effectRenderer);
        }

        public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade, int updateSteps) {
            this._effectRenderer.RenderUpdate(timestep, entity.Effects, entity, (EntityPainter)this, facade, entity.Physics.Position);
        }

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
        }

        public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
        }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep) {
        }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) {
            this._effectRenderer.Render(entity, (EntityPainter)this, renderTimestep, graphics, matrix, renderOrigin, renderMode);
        }

        public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) {
        }
    }
}
