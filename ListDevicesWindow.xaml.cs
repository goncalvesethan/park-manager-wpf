using ParkManagerWPF.Assets;
using ParkManagerWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http.Json;
using Models;

namespace ParkManagerWPF
{
    /// <summary>
    /// Logique d'interaction pour ListDevicesWindow.xaml
    /// </summary>
    public partial class ListDevicesWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public ListDevicesWindow()
        {
            InitializeComponent();
            StartUpParams();
        }

        private void StartUpParams()
        {
            this.Title = "Liste des postes";
            this.Height = 700;
            this.Width = 1100;
        }

        private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Close();
            e.Cancel = true;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDevicesAsync();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            AuthenticationManager.Logout();
        }

        private async void RefreshButton(object sender, RoutedEventArgs e)
        {
            await LoadDevicesAsync();
        }

        private async Task LoadDevicesAsync()
        {
            try
            {
                string token = AuthenticationManager.GetJwtToken();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var devices = await _httpClient.GetFromJsonAsync<List<Device>>("http://localhost:5296/api/devices");

                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        device.Park = await _httpClient.GetFromJsonAsync<Park>($"http://localhost:5296/api/parks/{device.ParkId}");
                        device.Room = await _httpClient.GetFromJsonAsync<Room>($"http://localhost:5296/api/rooms/{device.RoomId}");
                        device.OnlineLabel = device.IsOnline ? "Oui" : "Non";
                    }
                }
                else
                {
                    WindowsNotification.make(3000, "Erreur lors du chargement des postes.", ToolTipIcon.Error);
                }

                DevicesDataGrid.ItemsSource = devices;
            }
            catch (Exception ex)
            {
                WindowsNotification.make(3000, $"Erreur : {ex.Message}", ToolTipIcon.Error);
            }
        }

        private async void LockDevice(object sender, RoutedEventArgs e)
        {
            if (DevicesDataGrid.SelectedItem is Device selectedDevice)
            {
                await SendDeviceActionAsync(selectedDevice, "lock");
            }
        }

        private async void RebootDevice(object sender, RoutedEventArgs e)
        {
            if (DevicesDataGrid.SelectedItem is Device selectedDevice)
            {
                await SendDeviceActionAsync(selectedDevice, "reboot");
            }
        }

        private async void ShutdownDevice(object sender, RoutedEventArgs e)
        {
            if (DevicesDataGrid.SelectedItem is Device selectedDevice)
            {
                await SendDeviceActionAsync(selectedDevice, "shutdown");
            }
        }

        private async Task SendDeviceActionAsync(Device device, string actionType)
        {
            try
            {
                var action = new DeviceAction
                {
                    DeviceId = device.Id,
                    Type = actionType,
                    Status = "pending"
                };

                string token = AuthenticationManager.GetJwtToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("http://localhost:5296/api/actions", action);

                Console.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    WindowsNotification.make(3000, $"Action '{actionType}' envoyée pour le poste {device.Name}.", ToolTipIcon.Info);
                }
                else
                {
                    WindowsNotification.make(3000, $"Erreur {response.StatusCode} lors de l'envoi de l'action.", ToolTipIcon.Error);
                }
            }
            catch (Exception ex)
            {
                WindowsNotification.make(3000, $"Exception : {ex.Message}", ToolTipIcon.Error);
            }
        }
        private void DevicesDataGrid_RightClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while (dep != null && !(dep is DataGridRow))
                dep = VisualTreeHelper.GetParent(dep);

            if (dep is DataGridRow row)
                row.IsSelected = true;
        }

    }
    public class DeviceAction
    {
        public int DeviceId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
