using RabbitMQ.Client;
using RabbitQueueCaller.Models;
using RabbitQueueCaller.Services;
using System.Net;

namespace RabbitQueueCaller
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
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
                "Erro",
                "Falha ao se conectar ao RabbitMQ! (Porta inválida)",
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
                "Sucesso",
                "Conectado ao RabbitMQ!",
                "OK");
            }
            catch
            {
                LoadingOverlay.IsVisible = false;

                await DisplayAlertAsync(
                "Erro",
                "Falha ao se conectar ao RabbitMQ!",
                "OK");
            }
        }
    }
}
