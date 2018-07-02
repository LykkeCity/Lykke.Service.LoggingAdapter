using Lykke.Common.Log;
using Microsoft.Extensions.Logging;

namespace Lykke.Service.LoggingAdapter.Core.Domain.Log
{
    public interface ILogWriter
    {
        void Write(ILogFactory logFactory, LogLevel logLevel, LogInformationDto logInformation);
    }
}
