using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Tiles.DockSites;
using NimbusFox.PowerAPI.Tiles.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Logic;
using Staxel.Rendering;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.Painters {
    public class GeneratorTileStateEntityPainter : DockTileStateEntityPainter {
        private readonly List<KeyValuePair<NameTag, int>> _siteTags = new List<KeyValuePair<NameTag, int>>();
        private readonly string _defaultNameTagText;

        public GeneratorTileStateEntityPainter(DockTileStateEntityBuilder builder) : base(builder) {

        }

        public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity,
            AvatarController avatarController, Timestep renderTimestep) {
            base.BeforeRender(graphics, renderOrigin, entity, avatarController, renderTimestep);

            if (!(entity.Logic is GeneratorTileStateEntityLogic logic)) {
                return;
            }

            var sites = logic.GetSites();

            if (_siteTags.Count != sites.Count) {
                _siteTags.Clear();
                for (var i = 0; i < sites.Count; i++) {
                    _siteTags.Add(new KeyValuePair<NameTag, int>(ClientContext.NameTagRenderer.RegisterNameTag(entity.Id), -1));
                }
            }

            for (var i = 0; i < _siteTags.Count; i++) {
                if (sites[i] is GeneratorDockSite generatorSite) {
                    _siteTags[i].Key.Setup(generatorSite.GetWorldPosition(), Constants.NameTagDefaultOffset, "", false, false, false);
                }
            }
        }

        public override string GetItemTag(DockSite site, AvatarController avatar, out bool usePetalTag,
            out string icon) {
            usePetalTag = false;
            icon = "";
            return "";
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (!disposing) {
                return;
            }

            foreach (var siteTag in _siteTags) {
                siteTag.Key.Dispose();
            }

            _siteTags.Clear();
        }
    }
}
