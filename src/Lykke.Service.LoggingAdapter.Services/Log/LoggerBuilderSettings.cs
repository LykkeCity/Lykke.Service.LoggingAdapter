using Lykke.SettingsReader;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class LoggerBuilderSettings
    {
        public IReloadingManager<string> ConnectionString { get; set; }
        public string TableName { get; set; }

        public string AppName { get; set; }

        public string SlackNotificationsConnectionString { get; set; }
        public string SlackNotificationsQueueName { get; set; }
    }
}
