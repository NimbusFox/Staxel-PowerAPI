using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusFox.PowerAPI.Classes {
    public class Cycle {
        private DateTime _lastCycle = DateTime.MinValue;
        private static bool _paused;
        /// <summary>
        /// Runs function every 50 milliseconds equalling to 20 times a second.
        /// Will not run when game is paused
        /// </summary>
        /// <param name="function"></param>
        public void RunCycle(Action function) {
            if (!_paused) {
                var cyclesToRun = 0;

                if (_lastCycle == DateTime.MinValue) {
                    cyclesToRun = 1;
                } else {
                    var diff = new TimeSpan(DateTime.Now.Ticks - _lastCycle.Ticks);
                    if (diff.TotalMilliseconds >= 50) {
                        cyclesToRun = (int)Math.Floor(diff.TotalMilliseconds / 50);
                    }
                }

                for (var i = 0; i < cyclesToRun; i++) {
                    function();
                }

                if (cyclesToRun > 0) {
                    _lastCycle = DateTime.Now;
                }
            } else {
                _lastCycle = DateTime.MinValue;
            }
        }

        internal static void Pause() {
            _paused = true;
        }

        internal static void Resume() {
            _paused = false;
        }
    }
}
