using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.LoggingAdapter.Contracts.Log
{
    public class LogRequest:IValidatableObject
    {
        [Required]
        public string AppName { get; set; }

        [Required]
        public string AppVersion { get; set; }

        public string EnvInfo { get; set; }

        [Required]
        [EnumDataType(typeof(LogLevel))]
        public LogLevel LogLevel { get; set; }

        public string Component { get; set; }

        public string Process { get; set; }

        public string Context { get; set; }

        public string Message { get; set; }

        public string CallStack { get; set; }

        public string ExceptionType { get; set; }

        public IEnumerable<string> AdditionalSlackChannels { get; set; }

        #region Validation

        private static readonly HashSet<LogLevel> NormalLogLevels = new HashSet<LogLevel>
        {
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Monitor
        };

        private static readonly HashSet<LogLevel> ErrorLogLevels = new HashSet<LogLevel>
        {
            LogLevel.Error,
            LogLevel.FatalError
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            
            if (NormalLogLevels.Contains(LogLevel))
            {
                if (string.IsNullOrEmpty(Message))
                {
                    result.Add(new ValidationResult($"Message is required when loglevel {LogLevel} used", new[] { nameof(Message) }));
                }
            }

            if (ErrorLogLevels.Contains(LogLevel))
            {
                if (string.IsNullOrEmpty(CallStack))
                {
                    result.Add(new ValidationResult($"Callstack is required when loglevel {LogLevel} used", new[] { nameof(CallStack) }));
                }

                if (string.IsNullOrEmpty(ExceptionType))
                {
                    result.Add(new ValidationResult($"ExceptionType is required when loglevel {LogLevel} used", new[] { nameof(ExceptionType) }));
                }
            }

            return result;
        }


        #endregion


    }
}
