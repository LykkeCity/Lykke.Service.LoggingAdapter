using System;
using System.Net;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Contracts.Log;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.Service.LoggingAdapter.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LoggingAdapter.Controllers
{
    public class LogsController:Controller
    {
        private readonly ILoggerSelector _loggerSelector;
        private readonly ILog _log;

        public LogsController(ILoggerSelector loggerSelector, ILogFactory logFactory)
        {
            _loggerSelector = loggerSelector;
            _log = logFactory.CreateLog(this);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("api/logs")]
        public IActionResult WriteFromForm([FromForm] LogRequest request)
        {
            return Write(request);
        }

        /// <summary>
        /// Writes log message to preconfigured logger
        /// </summary>
        /// <returns></returns>
        [Consumes("application/json")]
        [SwaggerOperation("WriteLog")]
        [HttpPost("api/logs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public IActionResult Write([FromBody]LogRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.CreateErrorResponce());
            }

            var component = request.Component ?? request.AppName;
            var log = _loggerSelector.GetLog(request.AppName, component);

            if (log == null)
            {
                _log.Warning(nameof(Write),  $"Logger for {request.AppName} not found", context: request);
                
                return BadRequest(ErrorResponse.Create($"Log for  appName {request.AppName} not found"));
            }

            Exception mockException = null;
            if (!string.IsNullOrEmpty(request.ExceptionType) || !string.IsNullOrEmpty(request.CallStack))
            {
                mockException = new Exception($"{request.ExceptionType} : {request.CallStack}");
            }

            //null check is in validation of LogRequest
            // ReSharper disable once PossibleInvalidOperationException
            log.Log(request.LogLevel.Value.MapToMicrosoftLoglevel(),
                0,
                new LogEntryParameters(request.AppName, 
                    request.AppVersion, 
                    request.EnvInfo ?? "?", 
                    request.CallerFilePath ?? "?", 
                    request.Process ?? "?", 
                    request.CallerLineNumber > 0 ? request.CallerLineNumber.Value : 1, 
                    request.Message,
                    request.Context,
                    DateTime.UtcNow), 
                mockException,
                (p, e) => p.Message);

            return Ok();
        }
    }
}
