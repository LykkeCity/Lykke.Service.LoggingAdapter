using Lykke.Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Services
{
    public interface IHealthNotifierSender
    {
        void SendNotification(ILogFactory logFactory, string appName, string appVersion, string envInfo, string healthMessage, object context = null);
    }
}
