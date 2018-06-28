using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LoggingAdapter.Core.Services
{
    public class HealthSlackNotificationAzureQueueSettings
    {
        public string SlackAzureQueueConnectionString { get; set; }

        public string SlackAzureQueuesBaseName { get; set; }
    }
}
