using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTWebApp.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Provisioning.Service;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IoTWebApp.Service
{
    public class HubService
    {
        const string ProvisionServiceConnectionString = "{Device Provisioning Service connection string}";
        const string ServiceConnectionString = "HostName=TestPackIotHub1.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=gL0bX64Nn6aJS5jHVps5yiU4RH7dg0WNoBdcGUgfjqA=";//"{IoT Hub Service connection string}";
        const string SampleTpmEndorsementKey =
                "AToAAQALAAMAsgAgg3GXZ0SEs/gakMyNRqXXJP1S124GUgtk8qHaGzMUaaoABgCAAEMAEAgAAAAAAAEAxsj2gUS" +
                "cTk1UjuioeTlfGYZrrimExB+bScH75adUMRIi2UOMxG1kw4y+9RW/IVoMl4e620VxZad0ARX2gUqVjYO7KPVt3d" +
                "yKhZS3dkcvfBisBhP1XH9B33VqHG9SHnbnQXdBUaCgKAfxome8UmBKfe+naTsE5fkvjb/do3/dD6l4sGBwFCnKR" +
                "dln4XpM03zLpoHFao8zOwt8l/uP3qUIxmCYv9A7m69Ms+5/pCkTu/rK4mRDsfhZ0QLfbzVI6zQFOKF/rwsfBtFe" +
                "WlWtcuJMKlXdD8TXWElTzgh7JS4qhFzreL0c1mI0GCj+Aws0usZh7dLIVPnlgZcBhgy1SSDQMQ==";
        //const string OptionalDeviceId = "myCSharpDevice";
        //const string SampleRegistrationId = "sample-individual-csharp";
        const ProvisioningStatus OptionalProvisioningStatus = ProvisioningStatus.Enabled;

        static ProvisioningServiceClient _provisioningServiceClient;
        static ServiceClient _serviceClient;
        static RegistryManager _registryManager;


        private static ProvisioningServiceClient GetProvisioningServiceClient()
        {
            if(_provisioningServiceClient == null)
            {
                _provisioningServiceClient = ProvisioningServiceClient.CreateFromConnectionString(ProvisionServiceConnectionString);
            }

            return _provisioningServiceClient;
        }

        private static ServiceClient GetServiceClient()
        {
            if(_serviceClient == null)
            {
                _serviceClient = ServiceClient.CreateFromConnectionString(ServiceConnectionString);
            }

            return _serviceClient;
        }

        private static RegistryManager GetRegistryManager()
        {
            if(_registryManager == null)
            {
                _registryManager = RegistryManager.CreateFromConnectionString(ServiceConnectionString);
            }

            return _registryManager;
        }


        public static async Task<bool> RegisterDevice(string deviceId, string deviceName)
        {
            if (String.IsNullOrEmpty(deviceId))
            {
                return false;
            }

            Console.WriteLine("Starting SetRegistrationData");

            Attestation attestation = new TpmAttestation(SampleTpmEndorsementKey);

            IndividualEnrollment individualEnrollment = new IndividualEnrollment(deviceName, attestation);

            individualEnrollment.DeviceId = deviceId;
            individualEnrollment.ProvisioningStatus = OptionalProvisioningStatus;

            Console.WriteLine("\nAdding new individualEnrollment...");

            var serviceClient = GetProvisioningServiceClient();
            
            IndividualEnrollment individualEnrollmentResult =
                await serviceClient.CreateOrUpdateIndividualEnrollmentAsync(individualEnrollment).ConfigureAwait(false);
            
            Console.WriteLine("\nIndividualEnrollment created with success.");
            Console.WriteLine(individualEnrollmentResult);

            return true;
        }

        public static async Task<IEnumerable<NewDeviceModel>> GetDevices()
        {
            var registryManager = GetRegistryManager();
            var devices = await registryManager.CreateQuery("SELECT * FROM devices", 100).GetNextAsTwinAsync();
            var deviceModels = devices.Select(dev => new NewDeviceModel
            {
                DeviceId = dev.DeviceId,
                DeviceName = dev.ETag,
                Status = dev.Status.ToString(),
                LastActivityUpdated = dev.LastActivityTime.Value
            });

            return deviceModels;
        }
    }
}
