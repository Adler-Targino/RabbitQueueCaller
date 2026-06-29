using RabbitMQ.Client;
using RabbitQueueCaller.Services;
using System.Text;

namespace RabbitQueueCaller.Views;

public partial class ConsumerPage : ContentPage
{
    public ConsumerPage()
    {
        InitializeComponent();
    }

    private async void PublishMessages(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        LoadingOverlayLabel.Text = "Validating connection...";

        var settings = await SettingsService.LoadRabbitSettingsAsync();

        IConnection? connection = null;
        IChannel? channel = null;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                Port = int.Parse(settings.Port),
                UserName = settings.UserName,
                Password = settings.Password
            };

            connection = await factory.CreateConnectionAsync();
        }
        catch
        {
            LoadingOverlay.IsVisible = false;
            await DisplayAlertAsync(
                "Error",
                "Unable to connect to RabbitMQ!\n\nPlease check if your connection settings are correct.",
                "OK");

            return;
        }

        try
        {
            channel = await connection!.CreateChannelAsync();

            if (string.IsNullOrWhiteSpace(QueueEntry.Text))
                throw new Exception("Invalid Queue Name!");

            await channel.QueueDeclareAsync
            (
                queue: QueueEntry.Text,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );


            int count = int.TryParse(CountEntry.Text, out int c) ? c : 1;
            CountEntry.Text = count.ToString();

            if (string.IsNullOrWhiteSpace(MessageEditor.Text))
                throw new Exception("Message can't be null!");

            var body = Encoding.UTF8.GetBytes(MessageEditor.Text);
            var properties = new BasicProperties
            {
                ContentType = "application/json"
            };

            for (int i = 1; i <= count; i++)
            {
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: QueueEntry.Text,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                LoadingOverlayLabel.Text = $"{i} of {count} messages sent!";
            }


            LoadingOverlay.IsVisible = false;

            await DisplayAlertAsync(
                "Success",
                $"{CountEntry.Text} messages successfully sent to the queue: {QueueEntry.Text}!",
                "OK");
        }
        catch (Exception ex)
        {
            LoadingOverlay.IsVisible = false;
            await DisplayAlertAsync(
                "Error",
                $"{ex.Message}",
                "OK");
        }
    }
}