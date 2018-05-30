using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Components;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Tiles.Logic {
    public class GeneratorTileEntityLogic : ChargeableTileEntityLogic {
        public GeneratorTileEntityLogic(Entity entity) : base(entity) { }

        private bool _generatePower = false;
        private bool _timePaused;
        private long _powerToGenerate;

        public virtual void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade, int efficiency = 100, bool inherited = false) {
            Update(timestep, entityUniverseFacade, true);
            _timePaused = entityUniverseFacade.IsTimeStopped();

            if (!inherited) {
                Cycle.RunCycle(RunCycle(efficiency));
            }
        }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            Update(timestep, entityUniverseFacade);
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            base.Construct(arguments, entityUniverseFacade);

            if (entityUniverseFacade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                if (tile.Configuration.Components.Select<GeneratorComponent>().Any()) {
                    var component = tile.Configuration.Components.Select<GeneratorComponent>().First();

                    _powerToGenerate = component.GeneratePerCycle;
                }
            }
        }

        public override void Store() {
            base.Store();

            Entity.Blob.SetLong("powerToGenerate", _powerToGenerate);
        }

        public override void Restore() {
            base.Restore();

            if (Entity.Blob.Contains("powerToGenerate")) {
                _powerToGenerate = Entity.Blob.GetLong("powerToGenerate");
            }
        }

        public virtual Action RunCycle(int efficency) {
            return () => {
                if (_timePaused) {
                    goto runBaseCycle;
                }

                if (_generatePower) {
                    goto runBaseCycle;
                }

                var toGenerate = _powerToGenerate * efficency;

                if (toGenerate == 0) {
                    goto runBaseCycle;
                }

                toGenerate = (long) Math.Floor((double) toGenerate / 100);

                TilePower.AddPower(toGenerate);

                runBaseCycle:
                base.RunCycle();
            };
        }
    }
}
