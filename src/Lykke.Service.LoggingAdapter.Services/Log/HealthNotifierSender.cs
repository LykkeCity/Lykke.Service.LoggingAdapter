using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.SlackNotifications;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class HealthNotifierSender : IHealthNotifierSender
    {
        private readonly ILogFactoryStorage _logFactoryStorage;
        private readonly HealthSlackNotificationAzureQueueSettings _healthSlackNotificationAzureQueueSettings;

        public HealthNotifierSender(ILogFactoryStorage logFactoryStorage, HealthSlackNotificationAzureQueueSettings healthSlackNotificationAzureQueueSettings)
        {
            _logFactoryStorage = logFactoryStorage;
            _healthSlackNotificationAzureQueueSettings = healthSlackNotificationAzureQueueSettings;
        }

        public void SendNotification(string appName, string appVersion, string envInfo, string healthMessage, object context = null)
        {
            using (var healthNotifier = CreateHealthNotifier(appName, appVersion, envInfo))
            {
                healthNotifier.Notify(healthMessage, context);  
            }
        }

        private IHealthNotifier CreateHealthNotifier(string appName, string appVersion, string envInfo)
        {
            return new HealthNotifier(appName, appVersion, envInfo, _logFactoryStorage.GetLogFactoryOrDefault(appName), CreateSlackNotificationSender());
        }

        private ISlackNotificationsSender CreateSlackNotificationSender()
        {
            var factory = new HealthNotifierSlackSenderFactory();

            return factory.Create(_healthSlackNotificationAzureQueueSettings.SlackAzureQueueConnectionString,
                _healthSlackNotificationAzureQueueSettings.SlackAzureQueuesBaseName);
        }
    }
}
