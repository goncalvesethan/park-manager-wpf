using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ParkManagerWPF.Assets;
using System.Windows.Forms;

namespace ParkManagerWPF
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var authManager = new AuthenticationManager();
                authManager.Login(email, password);
                CloseWindow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            this.Close();
        }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
    }
}
