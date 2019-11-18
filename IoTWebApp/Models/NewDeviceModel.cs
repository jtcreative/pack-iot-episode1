using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTWebApp.Models
{
    public class NewDeviceModel
    {
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string Status { get; set; }

        public DateTime LastActivityUpdated { get; set; }
    }
}
