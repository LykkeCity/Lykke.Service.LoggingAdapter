using System;
using System.Linq;
using Lykke.Service.LoggingAdapter.Contracts.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;

namespace Lykke.Service.LoggingAdapter.Helpers
{
    public static class LogRequestMapper
    {
        public static LogInformationDto MapToLogInformationDto(this LogRequest request)
        {
            if (request.LogLevel == null)
            {
                throw new ArgumentNullException(nameof(request.LogLevel));
            }

            return new LogInformationDto
            {
                AppName = request.AppName,
                AppVersion = request.AppVersion,
                Message = request.Message,
                CallStack = request.CallStack,
                Context = request.Context,
                EnvInfo = request.EnvInfo,
                LogLevel = request.LogLevel.Value.MapToMicrosoftLoglevel(),
                CallerFilePath = request.CallerFilePath,
                CallerLineNumber = request.CallerLineNumber,
                Process = request.Process,
                Component = request.Component,
                ExceptionType = request.ExceptionType,
                AdditionalSlackChannels = request.AdditionalSlackChannels.ToList()
            };
        }
    }
}
