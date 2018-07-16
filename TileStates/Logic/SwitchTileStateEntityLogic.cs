using System.Linq;
using NimbusFox.PowerAPI.Components.Tiles;
using NimbusFox.PowerAPI.TileEntities.Builders;
using NimbusFox.PowerAPI.TileEntities.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.TileStates.Logic {
    public class SwitchTileStateEntityLogic : TileStateEntityLogic {

        private EntityId _logicOwner = EntityId.NullEntityId;
        public TileConfiguration Configuration;

        public TileConfiguration On { get; private set; }
        public TileConfiguration Off { get; private set; }

        public SwitchTileStateEntityLogic(Entity entity) : base(entity) {
            Entity.Physics.PriorityChunkRadius(0, false);
        }
        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
        }
        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }
        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            Location = arguments.FetchBlob("location").GetVector3I();
            Configuration = GameContext.TileDatabase.GetTileConfiguration(arguments.GetString("tile"));
            if (_logicOwner == EntityId.NullEntityId) {

                if (Configuration.Components.Contains<SwitchTileComponent>()) {
                    var components = Configuration.Components.Get<SwitchTileComponent>();

                    On = GameContext.TileDatabase.GetTileConfiguration(components.On);
                    Off = GameContext.TileDatabase.GetTileConfiguration(components.Off);
                }

                var blob = BlobAllocator.Blob(true);

                blob.SetString("tile", Configuration.Code);
                blob.FetchBlob("location").SetVector3I(Location);

                var entities = new Lyst<Entity>();

                entityUniverseFacade.FindAllEntitiesInRange(entities, Location.ToVector3D(), 1F, entity => {
                    if (entity.Removed) {
                        return false;
                    }

                    if (entity.Logic is SwitchTileEntityLogic logic) {
                        return Location == logic.Location;
                    }

                    return false;
                });

                var tileEntity = entities.FirstOrDefault();

                if (tileEntity != default(Entity)) {
                    _logicOwner = tileEntity.Id;
                } else {
                    _logicOwner = SwitchTileEntityBuilder.Spawn(Location, blob, entityUniverseFacade).Id;
                }
            }
        }
        public override void Bind() { }
        public override bool Interactable() {
            return true;
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (alt.DownClick) {
                if (facade.ReadTile(Location, TileAccessFlags.SynchronousWait, out var tile)) {
                    facade.DirectWriteTile(Location,
                        tile.Configuration == On ? Off.MakeTile(tile.Variant()) : On.MakeTile(tile.Variant()),
                        TileAccessFlags.SynchronousWait);
                }
            }
        }

        public override string AltInteractVerb() {
            return "nimbusfox.powerapi.verb.switch";
        }

        public override bool CanChangeActiveItem() {
            return false;
        }

        public override bool IsPersistent() {
            return false;
        }

        public override bool IsLingering() {
            return false;
        }

        public override void KeepAlive() { }
        public override void BeingLookedAt(Entity entity) { }
        public override bool IsBeingLookedAt() {
            return false;
        }
    }
}
