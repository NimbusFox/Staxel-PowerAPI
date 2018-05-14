using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.PowerAPI.Commands {
    public class EnableDebugCommand : ICommandBuilder {

        public static bool DebugEnabled { get; internal set; } = false;

        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            if (bits.Length >= 2) {
                if (bits[1].ToLower() == "confirm") {
                    api.Facade().SetCheated();
                    DebugEnabled = true;
                    return "nimbusfox.powerapi.command.debug.enabled";
                }
            }

            return "nimbusfox.powerapi.command.debug.confirmation";
        }

        public string Kind => "powerdebug";
        public string Usage => "nimbusfox.powerapi.command.debug.description";
        public bool Public => false;
    }
}
