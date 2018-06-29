using System;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;
using Lykke.SlackNotifications;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class HealthNotificationWriter : IHealthNotificationWriter
    {
        private readonly HealthSlackNotificationAzureQueueSettings _healthSlackNotificationAzureQueueSettings;

        public HealthNotificationWriter(HealthSlackNotificationAzureQueueSettings healthSlackNotificationAzureQueueSettings)
        {
            _healthSlackNotificationAzureQueueSettings = healthSlackNotificationAzureQueueSettings;
        }

        public void SendNotification(ILogFactory logFactory, LogInformationDto logInformation)
        {
            if (string.IsNullOrEmpty(logInformation.AppName))
            {
                throw new ArgumentNullException(nameof(logInformation.AppName));
            }

            using (var healthNotifier = CreateHealthNotifier(logFactory, logInformation.AppName, logInformation.AppVersion, logInformation.EnvInfo))
            {
                healthNotifier.Notify(logInformation.Message, logInformation.Context);  
            }
        }

        private IHealthNotifier CreateHealthNotifier(ILogFactory logFactory, string appName, string appVersion, string envInfo)
        {
            return new HealthNotifier(appName, appVersion ?? "?", envInfo ?? "?", logFactory, CreateSlackNotificationSender());
        }

        private ISlackNotificationsSender CreateSlackNotificationSender()
        {
            var factory = new HealthNotifierSlackSenderFactory();

            return factory.Create(_healthSlackNotificationAzureQueueSettings.SlackAzureQueueConnectionString,
                _healthSlackNotificationAzureQueueSettings.SlackAzureQueuesBaseName);
        }
    }
}
