using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Service.LoggingAdapter.Core.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Service.LoggingAdapter.Services
{
    public class LoggerSelector:ILoggerSelector
    {
        private readonly ILogFactory _logFactory;

        public LoggerSelector(ILogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        public ILog GetLog(string appName, string component)
        {
            //return DirectConsoleLogFactory.Instance.CreateLog(component);
            return _logFactory.CreateLog(component);
        }
    }
}
