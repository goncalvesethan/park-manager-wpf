using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace ParkManagerWPF.Assets
{
    public class AuthenticationManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static bool IsUserLoggedIn()
        {
            // Vérifie si le token est non nul et non vide
            return !string.IsNullOrEmpty(Properties.Settings.Default.JwtToken);
        }

        // Retourne le token JWT de l'utilisateur
        public static string GetJwtToken()
        {
            return Properties.Settings.Default.JwtToken;
        }

        // Connexion de l'utilisateur, stocke le token JWT
        public static void SetJwtToken(string token)
        {
            Properties.Settings.Default.JwtToken = token;
            Properties.Settings.Default.Save();
        }

        // Déconnexion de l'utilisateur, supprime le token JWT
        public static void Logout()
        {
            WindowsNotification.make(1000, "Utilisateur déconnecté !", ToolTipIcon.Info);

            Properties.Settings.Default.JwtToken = string.Empty;
            Properties.Settings.Default.Save();
        }

        private static async Task<string> AuthenticateUser(string email, string password)
        {
            string apiUrl = "http://localhost:5296/api/auth/login";

            var credentials = new { email, password };
            string jsonData = JsonSerializer.Serialize(credentials);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Token ?? string.Empty;
            }
            return string.Empty;
        }

        public async void Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                WindowsNotification.make(1500, "Veuillez remplir tous les champs.", ToolTipIcon.Error);
                return;
            }

            try
            {
                string token = await AuthenticationManager.AuthenticateUser(email, password);

                if (!string.IsNullOrEmpty(token))
                {
                    WindowsNotification.make(1000, "Connexion réussie !", ToolTipIcon.Info);

                    SetJwtToken(token);
                }
                else
                {
                    WindowsNotification.make(1500, "Identifiant incorrect !", ToolTipIcon.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

