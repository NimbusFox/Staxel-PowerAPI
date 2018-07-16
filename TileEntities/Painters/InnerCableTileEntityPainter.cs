using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.TileEntities.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.TileEntities.Painters {
    public class InnerCableTileEntityPainter : ChargeableTileEntityPainter {

        internal bool ThisClassMade = false;

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
            var fail = true;
            var runBase = true;
            if (entity.Logic is InnerCableTileEntityLogic logic) {
                if (ClientHook.ShowCables.Contains(logic.Location)) {
                    if (NameTag == null) {
                        NameTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                        ThisClassMade = true;
                    }

                    NameTag.Setup(logic.Location.ToTileCenterVector3D(), Constants.NameTagLowerOffset, ClientContext.LanguageDatabase.GetTranslationString(logic.Tile + ".name"), false, false, false);
                    fail = false;
                    runBase = false;
                }

                if (fail) {
                    if (NameTag != null && ThisClassMade) {
                        ClientContext.NameTagRenderer.Unregister(NameTag);
                        NameTag.Dispose();
                        NameTag = null;
                        runBase = false;
                        ThisClassMade = false;
                    }
                }
            }

            if (runBase) {
                base.ClientUpdate(timestep, entity, avatarController, facade);
            }
        }
    }
}
