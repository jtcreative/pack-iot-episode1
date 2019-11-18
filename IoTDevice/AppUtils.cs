using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace IoTDevice
{
    public class AppUtils
    {
        private Guid appId;
        private Guid deviceId;

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        public Guid ApplicationId
        {
            get
            {
                if (this.deviceId == Guid.Empty)
                {
                    EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();
                    this.deviceId = deviceInformation.Id;
                }

                return this.deviceId;
            }
        }


        /// <summary>
        /// Gets the unique device identifier.
        /// </summary>
        /// <value>
        /// The unique device identifier.
        /// </value>
        public Guid DeviceId
        {
            get
            {
                if (this.appId == Guid.Empty)
                {
                    SystemIdentificationInfo systemId = SystemIdentification.GetSystemIdForPublisher();

                    // Make sure this device can generate the IDs
                    if (systemId.Source != SystemIdentificationSource.None)
                    {
                        // The Id property has a buffer with the unique ID
                        DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(systemId.Id);
                        this.appId = dataReader.ReadGuid();
                    }
                }

                return this.appId;
            }
        }
    }
}
