using NimbusFox.PowerAPI.Items;
using Plukit.Base;
using Staxel;
using Staxel.Docks;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Player;
using Staxel.TileStates.Docks;

namespace NimbusFox.PowerAPI.Tiles.DockSites {
    public class ChargeableDockSite : DockSite {
        public ChargeableDockSite(Entity entity, DockSiteId id, DockSiteConfiguration dockSiteConfig) : base(entity, id, dockSiteConfig) { }

        private string _oldItemCode = "";

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

        public override int CanDock(ItemStack stack) {
            if (stack.Item is ChargeableItem chargeable) {
                if (chargeable.ItemPower.MaxCharge == chargeable.ItemPower.CurrentCharge) {
                    return 0;
                }
            } else {
                return 0;
            }

            return base.CanDock(stack);
        }

        public override void Update(Timestep timestep, EntityUniverseFacade universe) {
            base.Update(timestep, universe);

            if (DockedItem.Stack.Item is ChargeableItem chargeable) {
                if (_oldItemCode != chargeable.Configuration.Code) {
                    var newItem = (ChargeableItem)GameContext.ItemDatabase.InstanceFromItemConfiguration(chargeable.Configuration);

                    newItem.SetPower(chargeable.ItemPower.CurrentCharge);

                    EmptyWithoutExploding(universe);

                    AddToDock(_entity, new ItemStack(newItem, 1));

                    EffectQueue.StopAll();
                }
            }
        }

        public static string Name => "chargeable";
    }
}
