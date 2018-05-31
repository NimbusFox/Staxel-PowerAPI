using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Hooks;
using NimbusFox.PowerAPI.Tiles.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Rendering;

namespace NimbusFox.PowerAPI.Tiles.Painters {
    public class SolarPanelTileEntityPainter : ChargeableTileEntityPainter {

        private NameTag _efficienyTag;

        private NameTag _chargeTag;

        public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {
            var fail = true;
            if (entity.Logic is SolarPanelTileEntityLogic logic) {
                if (facade.ReadTile(logic.Location, TileAccessFlags.None, out var tile)) {
                    if (tile.Configuration.Components.Select<ChargeableComponent>().Any()) {
                        if (CycleHook.Tags.Contains(logic.Location)) {
                            if (NameTag == null) {
                                NameTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                            }

                            if (_efficienyTag == null) {
                                _efficienyTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                            }

                            if (_chargeTag == null) {
                                _chargeTag = ClientContext.NameTagRenderer.RegisterNameTag(entity.Id);
                            }

                            if (facade.TryGetLightPower(logic.Location, out var efficiency, out _, out _)) {
                                var pos = Constants.NameTagLowerOffset;
                                NameTag.Setup(entity.Physics.Position, new Vector3D(pos.X, pos.Y + 0.2, pos.Z), logic.TilePower.CurrentCharge.ToString("N0") + " / " + logic.TilePower.MaxCharge.ToString("N0"), false, false, false);
                                _efficienyTag.Setup(entity.Physics.Position, new Vector3D(pos.X, pos.Y, pos.Z), efficiency + "%", false, false, false);
                                _chargeTag.Setup(entity.Physics.Position, new Vector3D(pos.X, pos.Y - 0.2, pos.Z), logic.Generated + ClientContext.LanguageDatabase.GetTranslationString("nimbusfox.powerapi.perCycle"), false, false, false);
                                fail = false;
                            }
                        }
                    }
                }

                if (fail) {
                    if (NameTag != null) {
                        ClientContext.NameTagRenderer.Unregister(NameTag);
                        NameTag = null;
                    }

                    if (_efficienyTag != null) {
                        ClientContext.NameTagRenderer.Unregister(_efficienyTag);
                        _efficienyTag = null;
                    }

                    if (_chargeTag != null) {
                        ClientContext.NameTagRenderer.Unregister(_chargeTag);
                        _chargeTag = null;
                    }
                }
            }
        }
    }
}
