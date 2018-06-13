using Lykke.Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Services
{
    public interface ILogFactoryStorage
    {
        ILogFactory GetLogFactoryOrDefault(string appName);
    }
}
