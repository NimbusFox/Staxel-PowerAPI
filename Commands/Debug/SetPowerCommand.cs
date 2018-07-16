using NimbusFox.PowerAPI.Items;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.PowerAPI.Commands.Debug {
    public class SetPowerCommand : ICommandBuilder{
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };
            if (!EnableDebugCommand.DebugEnabled) {
                return "nimbusfox.powerapi.command.debugRequired";
            }

            if (!api.TryGetEntity(connection.ConnectionEntityId, out var player)) {
                return "";
            }

            if (player.Inventory.ActiveItem().IsNull()) {
                return "nimbusfox.powerapi.command.noItemInHand";
            }

            var activeItem = player.Inventory.ActiveItem().Item;

            if (activeItem is ChargeableItem == false) {
                responseParams = new object[] { activeItem.Configuration.Code };
                return "nimbusfox.powerapi.command.setPower.nonChargeableItem";
            }

            responseParams = new object[2];

            responseParams[0] = 0.ToString();
            responseParams[1] = activeItem.Configuration.Code;

            if (bits.Length >= 2) {
                if (long.TryParse(bits[1], out var toRemove)) {
                    var chargeableItem = activeItem as ChargeableItem;
                    var diff = chargeableItem.SetPower(toRemove);
                    responseParams[0] = diff.ToString();
                }
            }

            return "nimbusfox.powerapi.command.setPower.set";
        }

        public string Kind => "setpower";
        public string Usage => "nimbusfox.powerapi.command.setPower.description";
        public bool Public => false;
    }
}
