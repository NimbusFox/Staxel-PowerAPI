using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.PowerAPI.Items;
using Plukit.Base;
using Staxel.Docks;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Player;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.DockSites {
    public class GeneratorDockSite : DockSite {
        public GeneratorDockSite(Entity entity, DockSiteId id, DockSiteConfiguration dockSiteConfig) : base(entity, id, dockSiteConfig) { }

        public override bool TryDock(Entity user, EntityUniverseFacade facade, ItemStack stack, uint rotation) {
            if (CanDock(stack) <= 0) {
                return false;
            }

            var entry = FindEntry(stack.Item);

            if (!entry.PlaceSoundGroup.IsNullOrEmpty()) {
                BaseEffects.PlaySound(_entity, entry.PlaceSoundGroup);
            }

            if (!entry.EffectTrigger.IsNullOrEmpty()) {
                EffectQueue.Trigger(new EffectTrigger(entry.EffectTrigger));
            }

            AddToDock(user, stack, entry, rotation);
            return true;
        }

        public override bool TryUndock(PlayerEntityLogic player, EntityUniverseFacade facade, int quantity) {
            if (IsEmpty() || DockedItem.DockingEntity != player.PlayerEntity.Id) {
                return false;
            }

            var flag = base.TryUndock(player, facade, quantity);

            return flag;
        }

        public override int CanDock(ItemStack stack) {
            if (stack.Item is ChargeableItem chargeable) {
                if (chargeable.MaxWatts == chargeable.CurrentWatts) {
                    return 0;
                }
            } else {
                return 0;
            }

            return base.CanDock(stack);
        }
    }
}
