using Hardcodet.Wpf.TaskbarNotification.Interop;
using ParkManagerWPF.Models;
using System.ComponentModel;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Windows;
using ParkManagerWPF.Assets;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography.Pkcs;
using System.Windows.Threading;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json.Bson;

namespace ParkManagerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Device Device { get; set; }

    private readonly HttpClient _httpClient;
    private DispatcherTimer _syncTimer;
    private DispatcherTimer _actionTimer;

    public MainWindow()
    {
        InitializeComponent();
        StartUpParams();
        SetAuthButtonLabel();
        System.Windows.Application.Current.Exit += OnAppExit;

        _httpClient = new HttpClient();

        Device = DeviceInformations.GetDeviceInfo();
        this.DataContext = this;
        SyncDataTask();
        SyncActionTask();
    }
    private void StartUpParams()
    {
        this.Title = "Gestion du parc";
        this.Height = 800;
        this.Width = 500;

        this.Closing += OnClosing;
        this.Hide();
        this.WindowState = WindowState.Minimized;
    }

    private void OnAppExit(object sender, ExitEventArgs e)
    {
        Debug.WriteLine("Déconnexion automatique à la fermeture de l'application.");
        AuthenticationManager.Logout();
    }

    private void ShowMainWindow(object sender, RoutedEventArgs e)
    {
        this.Show();
        this.WindowState = WindowState.Normal;
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        this.Hide();
    }

    private void ExitApplication(object sender, EventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private async Task SubmitDataAsync(object data)
    {
        string apiUrl = $"http://localhost:5296/api/devices/mac/{Device.MacAddress}";

        string jsonData = JsonConvert.SerializeObject(data);
        
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, content);
            WindowsNotification.make(3000, "Les informations sur le poste ont bien été remontées.", ToolTipIcon.Info);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
        }
    }

    private async Task CheckPendingActionAsync(Device device)
    {
        try
        {
            string url = $"http://localhost:5296/api/actions/mac/{device.MacAddress}";

            var action = await _httpClient.GetFromJsonAsync<Models.Action>(url);

            if (action != null)
            {
                WindowsNotification.make(3000, $"Action détectée : {action.Type}", ToolTipIcon.Info);

                await _httpClient.PatchAsync(url, null);

                switch (action.Type.ToLower())
                {
                    case "lock":
                        ExecuteAction.Lock();
                        break;
                    case "reboot":
                        ExecuteAction.Reboot();
                        break;
                    case "shutdown":
                        ExecuteAction.Shutdown();
                        break;
                }
            } else
            {
                Debug.WriteLine("Aucune action détecté pour ce poste");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de la vérification des actions : {ex.Message}");
        }
    }

    public void SetAuthButtonLabel()
    {
        if (AuthenticationManager.IsUserLoggedIn())
        {
            LoginButton.Header = "Déconnexion";
        }
        else
        {
            LoginButton.Header = "Connexion";
        }
    }
    private void AuthenticationButtonAction(object sender, RoutedEventArgs e)
    {
        if (AuthenticationManager.IsUserLoggedIn())
        {
            AuthenticationManager.Logout();
        }
        else
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
        SetAuthButtonLabel();
    }

    private async void SynchronizeData(object sender, RoutedEventArgs e)
    {
        await SubmitDataAsync(Device);
    }

    private void SyncDataTask()
    {
        _syncTimer = new DispatcherTimer();
        _syncTimer.Interval = TimeSpan.FromMinutes(5);
        _syncTimer.Tick += async (sender, e) => await SubmitDataAsync(Device);
        _syncTimer.Start();

        _ = SubmitDataAsync(Device);
    }

    private void SyncActionTask()
    {
        _actionTimer = new DispatcherTimer();
        _actionTimer.Interval = TimeSpan.FromMinutes(1);
        _actionTimer.Tick += async (sender, e) => await CheckPendingActionAsync(Device);
        _actionTimer.Start();

        _ = CheckPendingActionAsync(Device);
    }

}
