﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.TileEntities.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Rendering;

namespace NimbusFox.PowerAPI.TileEntities.Painters {
    public class ChargeableTileEntityPainter : EntityPainter {

        internal NameTag NameTag;

        internal EntityUniverseFacade Universe;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (NameTag != null) {
                    ClientContext.NameTagRenderer.Unregister(NameTag);
                    NameTag = null;
                }
            }
        }

        public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade,
            int updateSteps) { }

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
            Universe = facade;
            if (Universe == null) {
                return;
            }
            var fail = true;
            if (entity.Logic is ChargeableTileEntityLogic logic) {
                if (Universe.ReadTile(logic.Location, TileAccessFlags.None, out var tile)) {
                    if (tile.Configuration.Components.Select<ChargeableComponent>().Any()) {
                        if (ClientHook.NameTags.Contains(logic.Location)) {
                            if (NameTag == null) {
                                NameTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                            }

                            NameTag.Setup(entity.Physics.Position, Constants.NameTagLowerOffset, logic.TilePower.CurrentCharge.ToString("N0") + " / " + logic.TilePower.MaxCharge.ToString("N0"), false, false, false);
                            fail = false;
                        }
                    }
                }

                if (entity.Logic is InnerCableTileEntityLogic innerLogic) {
                    if (ClientHook.NameTags.Contains(innerLogic.Location)) {
                        if (NameTag == null) {
                            NameTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                        }

                        NameTag.Setup(entity.Physics.Position, Constants.NameTagLowerOffset, innerLogic.TilePower.CurrentCharge.ToString("N0") + " / " + innerLogic.TilePower.MaxCharge.ToString("N0"), false, false, false);
                        fail = false;
                    }
                }

                if (fail) {
                    if (NameTag != null) {
                        ClientContext.NameTagRenderer.Unregister(NameTag);
                        NameTag = null;
                    }
                }
            }
        }
        public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController,
            Timestep renderTimestep) {
        }

        public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode) { }

        public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) { }
    }
}
