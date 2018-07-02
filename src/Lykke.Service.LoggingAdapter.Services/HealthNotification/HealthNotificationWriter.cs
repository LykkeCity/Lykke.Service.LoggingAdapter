using System.Linq;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.HealthNotification;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;

namespace Lykke.Service.LoggingAdapter.Services.HealthNotification
{
    public class HealthNotificationWriter : IHealthNotificationWriter
    {
        public void SendNotification(IHealthNotifier healthNotifier, LogInformationDto logInformation)
        {
            healthNotifier.Notify(PrepareHealthMessage(logInformation), logInformation.Context);
        }

        private string PrepareHealthMessage(LogInformationDto source)
        {
            return source.Message;
        }
    }
}
