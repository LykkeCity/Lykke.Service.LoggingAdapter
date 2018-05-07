using System;
using Common.Log;

namespace Lykke.Service.LoggingAdapter.Client
{
    public class LoggingAdapterClient : ILoggingAdapterClient, IDisposable
    {
        private readonly ILog _log;

        public LoggingAdapterClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
