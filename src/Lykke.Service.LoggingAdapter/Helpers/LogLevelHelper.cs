using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Lykke.Service.LoggingAdapter.Contracts.Log;

namespace Lykke.Service.LoggingAdapter.Helpers
{
    public static class LogLevelHelper
    {
        public static Microsoft.Extensions.Logging.LogLevel MapToMicrosoftLoglevel(this LogLevelContract logLevel)
        {
            switch (logLevel)
            {
                case LogLevelContract.Info:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                case LogLevelContract.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                case LogLevelContract.Warning:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                case LogLevelContract.FatalError:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
                default:
                    throw new InvalidEnumArgumentException(nameof(logLevel), (int)logLevel, typeof(LogLevelContract));
            }
        }
    }
}
