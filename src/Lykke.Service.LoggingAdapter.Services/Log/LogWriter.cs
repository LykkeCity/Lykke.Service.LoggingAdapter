using System;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class LogWriter:ILogWriter
    {
        public void Write(ILogFactory logFactory, LogInformationDto logInformation)
        {
            var log = logFactory.CreateLog(logInformation.Component ?? logInformation.AppName);

            Exception mockException = null;
            if (!string.IsNullOrEmpty(logInformation.ExceptionType) || !string.IsNullOrEmpty(logInformation.CallStack))
            {
                mockException = new Exception($"{logInformation.ExceptionType} : {logInformation.CallStack}");
            }
            
            log.Log(logInformation.LogLevel,
                0,
                new LogEntryParameters(logInformation.AppName,
                    logInformation.AppVersion,
                    logInformation.EnvInfo ?? "?",
                    logInformation.CallerFilePath ?? "?",
                    logInformation.Process ?? "?",
                    logInformation.CallerLineNumber > 0 ? logInformation.CallerLineNumber.Value : 1,
                    logInformation.Message,
                    logInformation.Context,
                    DateTime.UtcNow),
                mockException,
                (p, e) => p.Message);
        }
    }
}

