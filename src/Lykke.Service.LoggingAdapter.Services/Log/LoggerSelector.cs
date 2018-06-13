﻿using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Core.Services;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class LoggerSelector:ILoggerSelector
    {
        private readonly ILogFactoryStorage _logFactoryStorage;

        public LoggerSelector(ILogFactoryStorage logFactoryStorage)
        {
            _logFactoryStorage = logFactoryStorage;
        }

        public ILog GetLog(string appName, string component)
        {
            return _logFactoryStorage.GetLogFactoryOrDefault(appName)?.CreateLog(component);
        }
    }
}