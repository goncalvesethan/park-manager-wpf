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

namespace ParkManagerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Device Device { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        StartUpParams();

        Device = DeviceInformations.GetDeviceInfo();
        this.DataContext = this;
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
        Application.Current.Shutdown();
    }
}
