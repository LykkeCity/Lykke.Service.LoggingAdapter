using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.LoggingAdapter.Contracts.Log;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.Service.LoggingAdapter.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LoggingAdapter.Controllers
{
    public class LogsController:Controller
    {
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        public LogsController(ILogFactory logFactory, ILog log)
        {
            _logFactory = logFactory;
            _log = log;
        }

        /// <summary>
        /// Writes log message to preconfigured logger
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation("WriteLog")]
        [HttpPost("api/logs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Write([FromBody]LogRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.CreateErrorResponce());
            }

            var log = _logFactory.GetLog(request.AppName);

            if (log == null)
            {
                await _log.WriteWarningAsync(nameof(LogsController), nameof(Write), request.ToJson(),
                    $"Logger for {request.AppName} not found");

                return BadRequest(ErrorResponse.Create($"Log for  appName {request.AppName} not found"));
            }

            var mockException = new Exception($"{request.ExceptionType} : {request.Message} : {request.CallStack}");
            var component = request.Component ?? request.AppName;
            //Todo refact Lykke Logs
            switch (request.LogLevel)
            {
                case LogLevel.FatalError:
                {
                    await log.WriteFatalErrorAsync(component,
                        request.Process,
                        request.Context,
                        mockException);
                    break;
                }
                case LogLevel.Error:
                {
                    await log.WriteErrorAsync(component, 
                        request.Process,
                        request.Context,
                        mockException);
                    break;
                }
                case LogLevel.Warning:
                {
                    await log.WriteWarningAsync(component, 
                        request.Process, 
                        request.Context,
                        request.Message, 
                        mockException);
                    break;
                }
                case LogLevel.Monitor:
                {
                    await log.WriteMonitorAsync(component,
                        request.Process,
                        request.Context,
                        request.Message);
                    break;
                }
                case LogLevel.Info:
                {
                    await log.WriteInfoAsync(component,
                        request.Process,
                        request.Context,
                        request.Message);
                    break;
                }
            }

            return Ok();
        }
    }
}
