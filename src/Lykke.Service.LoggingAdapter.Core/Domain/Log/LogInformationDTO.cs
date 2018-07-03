using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Lykke.Service.LoggingAdapter.Core.Domain.Log
{
    public class LogInformationDto
    {
        public string AppName { get; set; }

        public string AppVersion { get; set; }

        public string EnvInfo { get; set; }

        public string Component { get; set; }

        public string Process { get; set; }
        
        public int? CallerLineNumber { get; set; }
        
        public string CallerFilePath { get; set; }

        public string Context { get; set; }

        public string Message { get; set; }

        public string CallStack { get; set; }

        public string ExceptionType { get; set; }

        public IEnumerable<string> AdditionalSlackChannels { get; set; }
    }
}
