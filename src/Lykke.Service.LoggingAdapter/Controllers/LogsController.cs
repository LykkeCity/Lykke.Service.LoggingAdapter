using System.Net;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.Log;
using Lykke.Service.LoggingAdapter.Contracts.Log;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;
using Lykke.Service.LoggingAdapter.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LoggingAdapter.Controllers
{
    public class LogsController:Controller
    {
        private readonly IHealthNotificationWriter _healthNotificationWriter;
        private readonly ILogFactoryStorage _logFactoryStorage;
        private readonly ILog _log;
        private readonly ILogWriter _logWriter;

        public LogsController(ILogFactory logFactory,
            IHealthNotificationWriter healthNotificationWriter, 
            ILogFactoryStorage logFactoryStorage,
            ILogWriter logWriter)
        {
            _healthNotificationWriter = healthNotificationWriter;
            _logFactoryStorage = logFactoryStorage;
            _logWriter = logWriter;
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
                return BadRequest(ErrorResponseFactory.Create(ModelState));
            }
            
            var logFactory = _logFactoryStorage.GetLogFactoryOrDefault(request.AppName);
            if (logFactory == null)
            {
                _log.Warning(nameof(Write), $"Logger for appName {request.AppName} not found", context: request);

                return BadRequest(ErrorResponse.Create($"Logger for  appName {request.AppName} not found"));
            }

            var logInformation = request.MapToLogInformationDto();
            if (request.LogLevel == LogLevelContract.Monitor)
            {
                _healthNotificationWriter.SendNotification(logFactory, logInformation);

                return Ok();
            }

            _logWriter.Write(logFactory, logInformation);

            return Ok();
        }
    }
}

