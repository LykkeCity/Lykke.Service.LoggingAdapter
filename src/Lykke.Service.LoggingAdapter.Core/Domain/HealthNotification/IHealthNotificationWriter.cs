using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;

namespace Lykke.Service.LoggingAdapter.Core.Domain.HealthNotification
{
    public interface IHealthNotificationWriter
    {
        void SendNotification(IHealthNotifier healthNotifier, LogInformationDto logInformation);
    }
}
