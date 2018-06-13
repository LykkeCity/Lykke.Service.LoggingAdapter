using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeAzureTable;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Service.LoggingAdapter.Core.Services;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class LogFactoryStorage:IDisposable, ILogFactoryStorage
    {
        private readonly IDictionary<string, ILogFactory> _logFactories;

        public LogFactoryStorage(IEnumerable<LoggerBuilderSettings> loggerBuilderSettings)
        {
            _logFactories = loggerBuilderSettings.ToDictionary(p => p.AppName, InitLogFactory);
        }

        public ILogFactory GetLogFactoryOrDefault(string appName)
        {
            if (_logFactories.ContainsKey(appName))
            {
                return _logFactories[appName];
            }

            return null;
        }

        private ILogFactory InitLogFactory(LoggerBuilderSettings settings)
        {
            return LogFactory.Create()
                .AddConsole()
                .AddAzureTable(settings.ConnectionString, settings.TableName)
                .AddEssentialSlackChannels(settings.SlackNotificationsConnectionString, settings.SlackNotificationsQueueName);
        }

        public void Dispose()
        {
            foreach (var lf in _logFactories.Values)
            {
                lf.Dispose();
            }
        }
    }
}
