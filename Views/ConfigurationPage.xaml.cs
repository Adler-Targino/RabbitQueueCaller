using RabbitMQ.Client;
using RabbitQueueCaller.Models;
using RabbitQueueCaller.Services;
using System.Net;

namespace RabbitQueueCaller.Views
{
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();

            LoadSettings();
        }

        private async void LoadSettings()
        {
            var settings = await SettingsService.LoadRabbitSettingsAsync();

            HostNameEntry.Text = settings.HostName;
            PortEntry.Text = settings.Port;
            UserNameEntry.Text = settings.UserName;
            PasswordEntry.Text = settings.Password;
        }

        private async void SaveAndVerify(object? sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            string host = HostNameEntry.Text ?? "";
            string port = PortEntry.Text ?? "";
            string user = UserNameEntry.Text ?? "";
            string password = PasswordEntry.Text ?? "";

            await SettingsService.SaveRabbitSettingsAsync(new RabbitMqSettings
            {
                HostName = host,
                Port = port,
                UserName = user,
                Password = password
            });

            if (!int.TryParse(port, out int httpPort))
            {
                await DisplayAlertAsync(
                "Error",
                "Unable to connect to RabbitMQ!\n\n(Invalid port.)",
                "OK");
            }

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = host,
                    Port = httpPort,
                    UserName = user,
                    Password = password
                };

                using var connection = await factory.CreateConnectionAsync();

                LoadingOverlay.IsVisible = false;

                await DisplayAlertAsync(
                "Success",
                "Connected to RabbitMQ!",
                "OK");
            }
            catch
            {
                LoadingOverlay.IsVisible = false;

                await DisplayAlertAsync(
                "Error",
                "Unable to connect to RabbitMQ!\n\nPlease check if your connection settings are correct.",
                "OK");
            }
        }
    }
}
