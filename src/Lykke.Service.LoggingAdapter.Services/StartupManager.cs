using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;
using Lykke.Service.LoggingAdapter.Core.Services;

namespace Lykke.Service.LoggingAdapter.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILogFactoryStorage _logFactoryStorage;
        private readonly int _logInitionMaxDegreeOfParallelism;

        public StartupManager(ILogFactory loggerFactory,
            ILogFactoryStorage logFactoryStorage,
            int logInitionMaxDegreeOfParallelism)
        {
            _logInitionMaxDegreeOfParallelism = logInitionMaxDegreeOfParallelism;
            _logFactoryStorage = logFactoryStorage;
            loggerFactory.CreateLog(this);
        }

        public  Task StartAsync()
        {
            return _logFactoryStorage.Init(_logInitionMaxDegreeOfParallelism);
        }
    }
}
