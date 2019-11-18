using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDevice
{
    class DeviceModel
    {
        public enum DeviceState
        {
            On = 0,
            Off,
            StandBy,
            Alert
        }

        public int BatteryLevel { get; set; }
        public int TemperatureSet { get; set; }
        public int TemperatureOutside { get; set; }
        public DeviceState State { get; set; }
    }
}
