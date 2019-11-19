using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTDevice
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel;
        private DeviceClient DeviceClient;
        private string deviceId = new AppUtils().DeviceId.ToString();

        private ConnectionStatusChangesHandler ConnStatusDelegate;

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            ConnStatusDelegate = OnConnectionChanged;
            ViewModel.DeviceIdStr = $"UNIQUE ID: {deviceId}";
        }

        private async void Register_Device(object sender, RoutedEventArgs e)
        {
            DeviceClient = DeviceClient.CreateFromConnectionString($"HostName={ViewModel.IoTHubName}.azure-devices.net;SharedAccessKey={ViewModel.IoTSharedAccessKey}", deviceId);
            DeviceClient.SetConnectionStatusChangesHandler(ConnStatusDelegate);

            try
            {
                //you need to register your device through the portal
                await DeviceClient.OpenAsync();
            }
            catch(Exception ex)
            {
                ViewModel.DeviceStatus = "DEVICE STATUS: Unable to connect";
            }
        }

        private void OnConnectionChanged(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            if (status == ConnectionStatus.Connected)
            {
                DeviceClient.SetDesiredPropertyUpdateCallbackAsync(OnPropertyUpdate, this);
                DeviceClient.SetMethodDefaultHandlerAsync(OnMethodReceived, this);
            }

        }

        private async Task<Task> OnPropertyUpdate(TwinCollection desiredProperties, object userContext)
        {
            var message = await DeviceClient.ReceiveAsync();

            BatteryLevelInput.Text = message.Properties["batteryLevel"];
            TemperatureLevelInput.Text = message.Properties["temperatureLevel"];
            TemperatureLevelOutside.Text = message.Properties["temperatureLevelOutside"];

            var state = message.Properties["state"];
            if (state.Equals("On"))
            {
                OnRB.IsChecked = true;
            }
            else if (state.Equals("Off"))
            {
                OffRB.IsChecked = true;
            }
            else if (state.Equals("StandBy"))
            {
                StandByRB.IsChecked = true;
            }
            else if (state.Equals("Alert"))
            {
                AlertRB.IsChecked = true;
            }

            return DeviceClient.CompleteAsync("Properties updated");
        }

        private async Task<MethodResponse> OnMethodReceived(MethodRequest methodRequest, object userContext)
        {
            var receivedMessage = await DeviceClient.ReceiveAsync();
            var methodName = methodRequest.Name;

            ///do some method based on the name
            ///

            return new MethodResponse(200);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(sender.Equals(OnRB))
            {
                ViewModel.DeviceModel.State = DeviceModel.DeviceState.On;
            }
            else if(sender.Equals(OffRB))
            {
                ViewModel.DeviceModel.State = DeviceModel.DeviceState.Off;
            }
            else if (sender.Equals(StandByRB))
            {
                ViewModel.DeviceModel.State = DeviceModel.DeviceState.StandBy;
            }
            else if (sender.Equals(AlertRB))
            {
                ViewModel.DeviceModel.State = DeviceModel.DeviceState.Alert;
            }
        }

        private async void Send_Data(object sender, RoutedEventArgs e)
        {
            var twin = await DeviceClient.GetTwinAsync(); 
            var reportedProperties = twin.Properties.Reported;
            Message response = new Message();
            response.Properties["batteryLevel"] = ViewModel.DeviceModel.BatteryLevel.ToString();
            response.Properties["temperatureLevel"] = ViewModel.DeviceModel.BatteryLevel.ToString();
            response.Properties["temperatureLevelOutside"] = ViewModel.DeviceModel.BatteryLevel.ToString();

            if (OnRB.IsChecked.Value)
            {
                response.Properties["state"] = "On";
            }
            else if (OffRB.IsChecked.Value)
            {
                response.Properties["state"] = "Off";
            }
            else if (StandByRB.IsChecked.Value)
            {
                response.Properties["state"] = "StandBy";
            }
            else if (AlertRB.IsChecked.Value)
            {
                response.Properties["state"] = "Alert";
            }
            
            await DeviceClient.UpdateReportedPropertiesAsync(reportedProperties);
        }

        private void Simulate_Low_Battery(object sender, RoutedEventArgs e)
        {

        }

        private void Reset_Data(object sender, RoutedEventArgs e)
        {

        }
    }
}
