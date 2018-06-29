using Lykke.Common.Log;

namespace Lykke.Service.LoggingAdapter.Core.Domain.Log
{
    public interface ILogWriter
    {
        void Write(ILogFactory logFactory, LogInformationDto logInformation);
    }
}
