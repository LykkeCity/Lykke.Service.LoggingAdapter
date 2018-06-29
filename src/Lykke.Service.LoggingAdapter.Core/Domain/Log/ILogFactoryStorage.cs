using Lykke.Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Domain.Log
{
    public interface ILogFactoryStorage
    {
        ILogFactory GetLogFactoryOrDefault(string appName);

    }
}
