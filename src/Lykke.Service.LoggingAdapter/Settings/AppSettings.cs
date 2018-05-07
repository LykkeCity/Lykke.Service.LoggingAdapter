using JetBrains.Annotations;
using Lykke.Service.LoggingAdapter.Settings.ServiceSettings;
using Lykke.Service.LoggingAdapter.Settings.SlackNotifications;

namespace Lykke.Service.LoggingAdapter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public LoggingAdapterSettings LoggingAdapterService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
