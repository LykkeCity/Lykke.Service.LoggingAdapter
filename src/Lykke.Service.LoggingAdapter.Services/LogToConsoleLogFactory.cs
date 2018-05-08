using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;
using Lykke.Service.LoggingAdapter.Core.Services;

namespace Lykke.Service.LoggingAdapter.Services
{
    public class LogToConsoleLogFactory:ILogFactory
    {
        public ILog GetLog(string appName)
        {
            return new LogToConsole();
        }
    }
}
