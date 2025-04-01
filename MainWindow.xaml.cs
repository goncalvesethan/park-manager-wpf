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

namespace ParkManagerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Device Device { get; set; }

    private readonly HttpClient _httpClient;
    private DispatcherTimer _timer;

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
    }
    private void StartUpParams()
    {
        this.Title = "Gestion du parc";
        this.Height = 700;
        this.Width = 1100;

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
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMinutes(5);
        _timer.Tick += async (sender, e) => await SubmitDataAsync(Device);
        _timer.Start();

        _ = SubmitDataAsync(Device);
    }

}
