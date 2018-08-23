using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeAzureTable;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;

namespace Lykke.Service.LoggingAdapter.Services.Log
{
    public class LogFactoryStorage:IDisposable, ILogFactoryStorage
    {
        private readonly IEnumerable<LoggerBuilderSettings> _loggerBuilderSettingses;
        private readonly ILog _log;
        private readonly IDictionary<string, ILogFactory> _logFactories;
        private bool _isInitiated;

        public LogFactoryStorage(IEnumerable<LoggerBuilderSettings> loggerBuilderSettings, ILogFactory logFactory)
        {
            _loggerBuilderSettingses = loggerBuilderSettings;
            _log = logFactory.CreateLog(this);

            _logFactories = new Dictionary<string, ILogFactory>();
        }

        public ILogFactory GetLogFactoryOrDefault(string appName)
        {
            if (!_isInitiated)
            {
                throw new ArgumentException("LogFactory not inited");
            }

            _logFactories.TryGetValue(appName, out var result);

            return result;
        }

        public async Task Init(int maxDegreeOfParallelism)
        {
            var logCreation = new TransformBlock<LoggerBuilderSettings, (string appName, ILogFactory logFactory)>(p =>
                {
                    _log.Info($"Registering log: {p.AppName} -> {p.TableName}");

                    return (p.AppName, CreateLogFactory(p));
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });

            var fillStorage = new ActionBlock<(string appName, ILogFactory logFactory)>(p =>
                {
                    _logFactories[p.appName] = p.logFactory;
                }, new ExecutionDataflowBlockOptions{ MaxDegreeOfParallelism = 1});

            logCreation.LinkTo(fillStorage);

            var logCreationCompetion = logCreation.Completion.ContinueWith(p => fillStorage.Complete());

            foreach (var loggerBuilderSetting in _loggerBuilderSettingses)
            {
                logCreation.Post(loggerBuilderSetting);
            }

            logCreation.Complete();

            await Task.WhenAll(fillStorage.Completion, logCreationCompetion);

            _isInitiated = true;
        }

        private ILogFactory CreateLogFactory(LoggerBuilderSettings settings)
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
            Parallel.ForEach(_logFactories.Values, lf => lf.Dispose());
        }
    }
}
