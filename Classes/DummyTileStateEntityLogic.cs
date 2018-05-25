//using NimbusFox.PowerAPI.Components.Tiles;
//using NimbusFox.PowerAPI.Database;
//using NimbusFox.PowerAPI.Interfaces;
//using Plukit.Base;
//using Staxel.Logic;

//namespace NimbusFox.PowerAPI.Classes {
//    public class DummyTileStateEntityLogic : ITileWithPower {
//        public void RunCycle() { }
//        public Cycle Cycle { get; } = null;
//        public Entity Entity { get; } = null;
//        public Power TilePower { get; }
//        public bool OutputToTiles { get; }
//        public bool InputFromTiles { get; }

//        public void SetPower(long power) {
//            TilePower.SetPower(power);

//            PowerDatabase.SetPower(Location, TilePower.CurrentCharge);
//        }

//        public long GetPower() {
//            return TilePower.CurrentCharge;
//        }

//        public void AddPower(long power) {
//            TilePower.AddPower(power);

//            PowerDatabase.SetPower(Location, TilePower.CurrentCharge);
//        }

//        public void RemovePower(long power) {


//            TilePower.RemovePower(power);

//            PowerDatabase.SetPower(Location, TilePower.CurrentCharge);
//        }

//        private Vector3I Location { get; set; }

//        public DummyTileStateEntityLogic(Vector3I location, CableTileComponent component) {
//            TilePower = new Power(UpdateModel);

//            TilePower.GetPowerFromComponent(component);
//            OutputToTiles = TilePower.OutputToTiles;
//            InputFromTiles = TilePower.InputFromTiles;

//            Location = location;

//            if (PowerDatabase.Exists(location)) {
//                TilePower.SetPower(PowerDatabase.GetPower(location));
//            }
//        }

//        private void UpdateModel(bool _) {
//        }
//    }
//}
