using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDevice
{
    class MainViewModel
    {
        public MainViewModel()
        {
            DeviceModel = new DeviceModel();
            DeviceStatus = "DEVICE STATUS: UNREGISTERED";
        }
        public DeviceModel DeviceModel { get; set; }
        public string IoTHubName { get; set; }
        public string IoTSharedAccessKey { get; set; }

        public ObservableCollection<string> Logs = new ObservableCollection<string>();

        public string DeviceStatus { get; set; }

        public string DeviceIdStr { get; set; }

    }
}
