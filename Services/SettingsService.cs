using RabbitQueueCaller.Models;

namespace RabbitQueueCaller.Services
{
    internal class SettingsService
    {
        public static async Task SaveRabbitSettingsAsync(RabbitMqSettings settings)
        {
            Preferences.Set(nameof(settings.HostName), settings.HostName);
            Preferences.Set(nameof(settings.Port), settings.Port);
            Preferences.Set(nameof(settings.UserName), settings.UserName);

            await SecureStorage.SetAsync(nameof(settings.Password), settings.Password);
        }

        public static async Task<RabbitMqSettings> LoadRabbitSettingsAsync()
        {
            return new RabbitMqSettings
            {
                HostName = Preferences.Get(nameof(RabbitMqSettings.HostName), "localhost"),
                Port = Preferences.Get(nameof(RabbitMqSettings.Port), "5672"),
                UserName = Preferences.Get(nameof(RabbitMqSettings.UserName), ""),
                Password = await SecureStorage.GetAsync(nameof(RabbitMqSettings.Password)) ?? ""
            };
        }
    }
}
