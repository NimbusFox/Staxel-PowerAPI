using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Components;
using NimbusFox.PowerAPI.Items;
using NimbusFox.PowerAPI.TileEntities.Logic;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Tiles;

namespace NimbusFox.PowerAPI.Hooks {
    class ClientHook : IModHookV3 {
        public void Dispose() { }
        public void GameContextInitializeInit() { }
        public void GameContextInitializeBefore() { }
        public void GameContextInitializeAfter() { }
        public void GameContextDeinitialize() { }
        public void GameContextReloadBefore() { }
        public void GameContextReloadAfter() { }

        public static readonly List<Vector3I> NameTags = new List<Vector3I>();
        public static readonly List<Vector3I> ShowCables = new List<Vector3I>();

        public void UniverseUpdateBefore(Universe universe, Timestep step) {
            if (!universe.Server) {
                var entities = new Lyst<Entity>();

                universe.GetPlayers(entities);

                Entity entity = null;

                foreach (var e in entities) {
                    if (ClientContext.PlayerFacade.IsLocalPlayer(e)) {
                        entity = e;
                    }
                }

                if (entity == null) {
                    return;
                }

                var tags = new List<Vector3I>();

                if (entity.Inventory.ActiveItem().Item is MultiMeterItem) {
                    universe.ForAllEntitiesInRange(entity.FeetLocation(), 5, tile => {
                        if (tile.Logic is ChargeableTileEntityLogic logic) {
                            if (!tags.Contains(logic.Location)) {
                                tags.Add(logic.Location);
                            }
                        }
                    });
                }

                NameTags.Clear();
                NameTags.AddRange(tags);

                var showCables = new List<Vector3I>();

                if (!entity.Inventory.ActiveItem().IsNull()) {
                    var item = entity.Inventory.ActiveItem().Item;

                    var config = GameContext.TileDatabase.AllMaterials()
                        .FirstOrDefault(x => x.Code == item.GetItemCode());

                    if (config != default(TileConfiguration) || item is CableDrillItem) {
                        var component = config?.Components.Select<ChargeableComponent>().FirstOrDefault();

                        if (component != default(ChargeableComponent) || item is CableDrillItem) {
                            universe.ForAllEntitiesInRange(entity.FeetLocation(), 5, tile => {
                                if (tile.Logic is InnerCableTileEntityLogic logic) {
                                    if (!showCables.Contains(logic.Location)) {
                                        showCables.Add(logic.Location);
                                    }
                                }
                            });
                        }
                    }
                }

                ShowCables.Clear();
                ShowCables.AddRange(showCables);
            }
        }
        public void UniverseUpdateAfter() { }
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) {
            return true;
        }

        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) {
            return true;
        }

        public void ClientContextInitializeInit() { }
        public void ClientContextInitializeBefore() { }
        public void ClientContextInitializeAfter() { }
        public void ClientContextDeinitialize() { }
        public void ClientContextReloadBefore() { }
        public void ClientContextReloadAfter() { }
        public void CleanupOldSession() { }
        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) {
            return true;
        }

        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) {
            return true;
        }
    }
}
