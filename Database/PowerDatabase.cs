//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NimbusFox.PowerAPI.Classes;
//using NimbusFox.PowerAPI.Components.Tiles;
//using NimbusFox.PowerAPI.Interfaces;
//using Plukit.Base;
//using Staxel;
//using Staxel.Items;
//using Staxel.Logic;

//namespace NimbusFox.PowerAPI.Database {
//    public static class PowerDatabase {
//        private static Blob _dbBlob;

//        private const string FileName = "powerapi.db";

//        private static DateTime _lastSave = DateTime.MinValue;

//        private static void Save(bool force = false) {
//            if (!Process.GetCurrentProcess().ProcessName.Contains("Staxel.Server")) {
//                return;
//            }
//            if (new TimeSpan(DateTime.Now.Ticks - _lastSave.Ticks).TotalMinutes > 5 || force) {
//                var stream = new MemoryStream();

//                _dbBlob.SaveJsonStream(stream);

//                stream.Seek(0, SeekOrigin.Begin);

//                GameContext.ContentLoader.WriteLocalStream(FileName, stream);
//                _lastSave = DateTime.Now;
//            }
//        }

//        public static void Init() {
//            if (_dbBlob == null) {
//                _dbBlob = BlobAllocator.AcquireAllocator().NewBlob(true);
//                LoadDatabase();
//            }
//        }

//        private static void LoadDatabase() {
//            var dbStream = GameContext.ContentLoader.ReadLocalStream(FileName, false);

//            if (dbStream != null) {
//                _dbBlob.Read((MemoryStream)dbStream);
//            }
//        }

//        internal static void SaveDatabase() {
//            Save(true);
//        }

//        private static string GetKey(Vector3I location) {
//            return $"{location.X},{location.Y},{location.Z}";
//        }

//        public static void SetPower(Vector3I location, long power) {
//            var key = GetKey(location);

//            _dbBlob.FetchBlob(key).SetLong("power", power);
//            Save();
//        }

//        public static long GetPower(Vector3I location) {
//            return _dbBlob.FetchBlob(GetKey(location)).GetLong("power", -1);
//        }

//        public static void Remove(Vector3I location) {
//            if (Exists(location)) {
//                _dbBlob.Delete(GetKey(location));
//            }
//        }

//        public static bool Exists(Vector3I location) {
//            return _dbBlob.Contains(GetKey(location));
//        }

//        public static ITileWithPower GetPower(Vector3I location, EntityUniverseFacade facade) {
//            if (facade.TryFetchTileStateEntityLogic(location, TileAccessFlags.SynchronousWait, out var logic)) {
//                if (logic is ITileWithPower tileLogic) {
//                    return tileLogic;
//                }
//            }
//            if (Exists(location)) {
//                if (facade.ReadTile(location, TileAccessFlags.SynchronousWait, out var tile)) {
//                    var components = tile.Configuration.Components.Select<CableTileComponent>().FirstOrDefault();

//                    if (components != default(CableTileComponent)) {
//                        return new DummyTileStateEntityLogic(location, components);
//                    }
//                }
//            }

//            return null;
//        }

//        public static List<Vector3I> GetPowerTiles() {
//            var output = new List<Vector3I>();

//            foreach (var key in _dbBlob.KeyValueIteratable.Keys) {
//                if (key.Count(x => x == ',') == 2) {
//                    var coords = key.Split(',');
//                    if (int.TryParse(coords[0], out var x)) {
//                        if (int.TryParse(coords[1], out var y)) {
//                            if (int.TryParse(coords[2], out var z)) {
//                                output.Add(new Vector3I(x, y, z));
//                            }
//                        }
//                    }
//                }
//            }

//            return output;
//        }

//        public static void SetExtras(Vector3I location, Blob extras) {
//            if (Exists(location)) {
//                var blob = _dbBlob.FetchBlob(GetKey(location));
//                if (blob.Contains("extras")) {
//                    blob.Delete("extras");
//                }

//                var extrasBlob = blob.FetchBlob("extras");
//                extrasBlob.MergeFrom(extras);

//                Save();
//            }
//        }

//        public static void MergeExtras(Vector3I location, Blob extras) {
//            if (Exists(location)) {
//                var blob = _dbBlob.FetchBlob(GetKey(location));
//                var extrasBlob = blob.FetchBlob("extras");
//                extrasBlob.MergeFrom(extras);

//                Save();
//            }
//        }
//    }
//}
