using System.Linq;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.HealthNotification;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;

namespace Lykke.Service.LoggingAdapter.Services.HealthNotification
{
    public class HealthNotificationWriter : IHealthNotificationWriter
    {
        public void SendNotification(IHealthNotifier healthNotifier, LogInformationDto logInformation)
        {
            healthNotifier.Notify(PrepareHealthMessage(logInformation), logInformation.Context);
        }

        private string PrepareHealthMessage(LogInformationDto source)
        {
            var propsDesctiptions = new []
            {
                (propName: "component", propValue: source.Component),
                (propName: "process", propValue: source.Process),
                (propName: "callerLineNumber", propValue: source.CallerLineNumber?.ToString()),
                (propName: "callerFilePath", propValue: source.CallerFilePath),
                (propName: "message", propValue: source.Message),
                (propName: "callStack", propValue: source.CallStack),
                (propName: "exceptionType", propValue: source.ExceptionType),
            }
            .Where(p => !string.IsNullOrEmpty(p.propValue))
            .Select(p => $"{p.propName} : {p.propValue}");

            return string.Join(" | ", propsDesctiptions);
        }
    }
}
