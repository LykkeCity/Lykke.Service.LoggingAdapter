using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.SlackNotifications;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class HealthNotifierSender : IHealthNotifierSender
    {
        private readonly HealthSlackNotificationAzureQueueSettings _healthSlackNotificationAzureQueueSettings;

        public HealthNotifierSender(HealthSlackNotificationAzureQueueSettings healthSlackNotificationAzureQueueSettings)
        {
            _healthSlackNotificationAzureQueueSettings = healthSlackNotificationAzureQueueSettings;
        }

        public void SendNotification(ILogFactory logFactory, string appName, string appVersion, string envInfo, string healthMessage, object context = null)
        {
            using (var healthNotifier = CreateHealthNotifier(logFactory, appName, appVersion, envInfo))
            {
                healthNotifier.Notify(healthMessage, context);  
            }
        }

        private IHealthNotifier CreateHealthNotifier(ILogFactory logFactory, string appName, string appVersion, string envInfo)
        {
            return new HealthNotifier(appName, appVersion, envInfo, logFactory, CreateSlackNotificationSender());
        }

        private ISlackNotificationsSender CreateSlackNotificationSender()
        {
            var factory = new HealthNotifierSlackSenderFactory();

            return factory.Create(_healthSlackNotificationAzureQueueSettings.SlackAzureQueueConnectionString,
                _healthSlackNotificationAzureQueueSettings.SlackAzureQueuesBaseName);
        }
    }
}
