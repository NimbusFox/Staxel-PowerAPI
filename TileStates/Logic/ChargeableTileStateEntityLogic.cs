﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Classes;
using NimbusFox.PowerAPI.Interfaces;
using NimbusFox.PowerAPI.Tiles.Builders;
using NimbusFox.PowerAPI.Tiles.Logic;
using NimbusFox.PowerAPI.TileStates.Builders;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace NimbusFox.PowerAPI.TileStates.Logic {
    public class ChargeableTileStateEntityLogic : TileStateEntityLogic {

        private EntityId _logicOwner = EntityId.NullEntityId;
        private bool _despawn;
        public TileConfiguration Configuration;

        public ChargeableTileStateEntityLogic(Entity entity) : base(entity) {
            Entity.Physics.PriorityChunkRadius(0, false);
        }
        public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

        public override void Update(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            if (_logicOwner == EntityId.NullEntityId) {
                var blob = BlobAllocator.Blob(true);

                blob.SetString("tile", Configuration.Code);
                blob.FetchBlob("location").SetVector3I(Location);
                blob.SetBool("ignoreSpawn", _despawn);

                var entities = new Lyst<Entity>();

                entityUniverseFacade.FindAllEntitiesInRange(entities, Location.ToVector3D(), 1F, entity => {
                    if (entity.Removed) {
                        return false;
                    }

                    if (entity.Logic is ChargeableTileEntityLogic logic) {
                        return Location == logic.Location;
                    }

                    return false;
                });

                var tileEntity = entities.FirstOrDefault();

                if (tileEntity != default(Entity)) {
                    _logicOwner = tileEntity.Id;
                } else {
                    _logicOwner = ChargeableTileEntityBuilder.Spawn(Location, blob, entityUniverseFacade).Id;
                }

            }
        }

        public override void PostUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) {
            Tile result;
            if (!entityUniverseFacade.ReadTile(Location, TileAccessFlags.None, out result) || result.Configuration == Configuration)
                return;
        }

        public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade) {
            _despawn = arguments.GetBool("despawn", false);
            Location = arguments.FetchBlob("location").GetVector3I();
            Configuration = GameContext.TileDatabase.GetTileConfiguration(arguments.GetString("tile"));
        }
        public override void Bind() { }
        public override bool Interactable() {
            return !Configuration.InteractActionTrigger.IsNullOrEmpty();
        }

        public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {
            if (!alt.DownClick) {
                return;
            }
            entity.PlayerEntityLogic.NextAction(Configuration.InteractActionTrigger);
        }
        public override bool CanChangeActiveItem() {
            return true;
        }

        public override bool IsPersistent() {
            return true;
        }

        public override bool IsLingering() {
            return false;
        }

        public override void KeepAlive() {
            
        }

        public override void BeingLookedAt(Entity entity) {
            KeepAlive();
        }
        public override bool IsBeingLookedAt() {
            return true;
        }

        public EntityId GetOwner() {
            return _logicOwner;
        }

        public void SetOwner(Entity tileEntity) {
            _logicOwner = tileEntity.Id;
        }

        public override void UpdateWithData(Blob blob) {
            _logicOwner = blob.GetLong("owner", 0L);
            if (_logicOwner == EntityId.NullEntityId) {
                return;
            }

            Configuration = GameContext.TileDatabase.GetTileConfiguration(blob.GetString("tile"));
        }

        public override void StorePersistenceData(Blob data) {
            base.StorePersistenceData(data);
            data.SetLong("logicOwner", _logicOwner.Id);
            data.SetBool("despawn", _despawn);
        }

        public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade) {
            base.RestoreFromPersistedData(data, facade);
            _logicOwner = (EntityId) data.GetLong("logicOwner", 0L);
            _despawn = data.GetBool("despawn", false);
        }
    }
}
