using Lykke.Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Domain.HealthNotification
{
    public interface IHealthNotifierStorage
    {
        IHealthNotifier GetOrCreateHealthNotifier(string appName, string appVersion, string envInfo, ILogFactory logFactory);
    }
}
