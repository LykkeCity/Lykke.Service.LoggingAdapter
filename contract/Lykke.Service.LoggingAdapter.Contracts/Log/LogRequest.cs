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
        [EnumDataType(typeof(LogLevelContract))]
        public LogLevelContract LogLevel { get; set; }

        public string Component { get; set; }

        public string Process { get; set; }

        /// <summary>
        /// Source code file line number, where the entry was made.
        /// </summary>
        public int? CallerLineNumber { get; set; }

        /// <summary>
        /// Path of the source code file, where the entry was made. 
        /// </summary>
        public string CallerFilePath { get; set; }

        public string Context { get; set; }

        public string Message { get; set; }

        public string CallStack { get; set; }

        public string ExceptionType { get; set; }

        public IEnumerable<string> AdditionalSlackChannels { get; set; }

        #region Validation

        private static readonly HashSet<LogLevelContract> NormalLogLevels = new HashSet<LogLevelContract>
        {
            LogLevelContract.Info,
            LogLevelContract.Warning,
            LogLevelContract.Monitor
        };

        private static readonly HashSet<LogLevelContract> ErrorLogLevels = new HashSet<LogLevelContract>
        {
            LogLevelContract.Error,
            LogLevelContract.FatalError
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (LogLevel == LogLevelContract.None)
            {
                result.Add(new ValidationResult("Log Level field is required", new[] { nameof(LogLevel) }));

                return result;
            }
            
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
