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

        private Task OnPropertyUpdate(TwinCollection desiredProperties, object userContext)
        {
            if(desiredProperties.Contains("batteryLevel"))
            {
                ViewModel.DeviceModel.BatteryLevel = (int)desiredProperties["batteryLevel"];
            }

            if (desiredProperties.Contains("setTemperature"))
            {
                ViewModel.DeviceModel.TemperatureSet = (int)desiredProperties["setTemperature"];
            }

            if (desiredProperties.Contains("roomTemperature"))
            {
                ViewModel.DeviceModel.TemperatureOutside = (int)desiredProperties["roomTemperature"];
            }

            if (desiredProperties.Contains("state"))
            {
                var state = (int)desiredProperties["roostatemTemperature"];
                switch(state)
                {
                    case 0:
                        OnRB.IsChecked = true;
                        ViewModel.DeviceModel.State = DeviceModel.DeviceState.On;
                        break;
                    case 1:
                        OffRB.IsChecked = true;
                        ViewModel.DeviceModel.State = DeviceModel.DeviceState.Off;
                        break;
                    case 2:
                        StandByRB.IsChecked = true;
                        ViewModel.DeviceModel.State = DeviceModel.DeviceState.StandBy;
                        break;
                    default:
                        AlertRB.IsChecked = true;
                        ViewModel.DeviceModel.State = DeviceModel.DeviceState.Alert;
                        break;
                }
            }

            return DeviceClient.CompleteAsync("Properties updated");
        }

        private Task<MethodResponse> OnMethodReceived(MethodRequest methodRequest, object userContext)
        {
            return null;
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

        private void Send_Data(object sender, RoutedEventArgs e)
        {

        }

        private void Simulate_Low_Battery(object sender, RoutedEventArgs e)
        {

        }

        private void Reset_Data(object sender, RoutedEventArgs e)
        {

        }
    }
}
