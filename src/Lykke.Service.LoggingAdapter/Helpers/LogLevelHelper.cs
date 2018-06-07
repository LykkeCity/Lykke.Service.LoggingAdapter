using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Lykke.Service.LoggingAdapter.Contracts.Log;

namespace Lykke.Service.LoggingAdapter.Helpers
{
    public static class LogLevelHelper
    {
        public static Microsoft.Extensions.Logging.LogLevel MapToMicrosoftLoglevel(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                case LogLevel.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                case LogLevel.Warning:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                case LogLevel.FatalError:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
                default:
                    throw new InvalidEnumArgumentException(nameof(logLevel), (int)logLevel, typeof(LogLevel));
            }
        }
    }
}
