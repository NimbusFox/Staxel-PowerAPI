using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Destruction;
using Staxel.Logic;

namespace NimbusFox.PowerAPI.Classes {
    internal sealed class DestructionEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2, IEntityLogicBuilder {
        EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server) {
            return (EntityLogic)new DestructionEntityLogic(entity);
        }

        public string Kind {
            get {
                return DestructionEntityBuilder.KindCode;
            }
        }

        public static string KindCode {
            get {
                return "staxel.entity.Destruction.nimbusfox";
            }
        }

        EntityPainter IEntityPainterBuilder.Instance() {
            return (EntityPainter)new DestructionEntityPainter();
        }

        public void Load() {
        }

        public static DestructionEntityLogic Spawn(Entity user, Vector3I position, EntityUniverseFacade facade, string hitSound) {
            Entity entity = new Entity(facade.AllocateNewEntityId(), false, DestructionEntityBuilder.KindCode, true);
            Blob blob = BlobAllocator.Blob(true);
            blob.SetString("kind", DestructionEntityBuilder.KindCode);
            blob.FetchBlob(nameof(position)).SetVector3D(position.ToTileCenterVector3D());
            blob.FetchBlob("velocity").SetVector3D(new Vector3D(0.0, 10.0, 0.0));
            blob.SetLong(nameof(user), user.Id.Id);
            blob.SetString(nameof(hitSound), hitSound);
            entity.Construct(blob, facade);
            Blob.Deallocate(ref blob);
            facade.AddEntity(entity);
            return (DestructionEntityLogic)entity.Logic;
        }

        public bool IsTileStateEntityKind() {
            return false;
        }
    }
}
