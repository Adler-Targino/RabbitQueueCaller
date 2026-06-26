using System.Text.Json;

namespace RabbitQueueCaller.Models
{
    internal class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public string Port { get; set; } = "5672";
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
