using Lykke.Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Domain.Log
{
    public interface IHealthNotificationWriter
    {
        void SendNotification(ILogFactory logFactory, LogInformationDto logInformation);
    }
}
