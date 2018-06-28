using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IReadOnlyDictionary<string, ILogFactory> _logFactories;

        public LogFactoryStorage(IEnumerable<LoggerBuilderSettings> loggerBuilderSettings, ILogFactory logFactory)
        {
            var log = logFactory.CreateLog(this);

            _logFactories = loggerBuilderSettings.ToDictionary(p => p.AppName, p =>
            {
                log.Info($"Registering log {p.AppName} -> {p.TableName}");

                return InitLogFactory(p);
            });
        }

        public ILogFactory GetLogFactoryOrDefault(string appName)
        {
            _logFactories.TryGetValue(appName, out ILogFactory result);

            return result ?? EmptyLogFactory.Instance;
        }

        private ILogFactory InitLogFactory(LoggerBuilderSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (string.IsNullOrEmpty(settings.AppName))
            {
                throw new ArgumentNullException(nameof(settings.AppName));
            }
            if (string.IsNullOrEmpty(settings.TableName))
            {
                throw new ArgumentNullException(nameof(settings.TableName));
            }

            return LogFactory.Create()
                .AddConsole()
                .AddAzureTable(settings.ConnectionString, settings.TableName)
                .AddEssentialSlackChannels(settings.SlackNotificationsConnectionString, settings.SlackNotificationsQueueName);
        }

        public void Dispose()
        {
            Parallel.ForEach(_logFactories.Values, lf =>
            {
                lf.Dispose();
            });
        }
    }
}
