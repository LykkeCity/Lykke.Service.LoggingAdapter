using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Service.LoggingAdapter.Core.Domain.HealthNotification;
using Lykke.SlackNotifications;

namespace Lykke.Service.LoggingAdapter.Services.HealthNotification
{
    public class HealthNotifierStorage: IHealthNotifierStorage, IDisposable
    {
        private readonly ConcurrentDictionary<HealthNotifierBuilderSettings, Lazy<IHealthNotifier>> _healthNotifiers;
        private readonly ILog _log;
        private readonly ISlackNotificationsSender _slackNotificationsSender;

        public HealthNotifierStorage(ILogFactory logFactory, ISlackNotificationsSender slackNotificationsSender)
        {
            _slackNotificationsSender = slackNotificationsSender;
            _healthNotifiers = new ConcurrentDictionary<HealthNotifierBuilderSettings, Lazy<IHealthNotifier>>();
            _log = logFactory.CreateLog(this);
        }

        public IHealthNotifier GetOrCreateHealthNotifier(string appName, string appVersion, string envInfo, ILogFactory logFactory)
        {
            var builderSettings = new HealthNotifierBuilderSettings(appName, appVersion, envInfo);

            return _healthNotifiers.GetOrAdd(builderSettings, p =>
            {
                return new Lazy<IHealthNotifier>(() =>
                {
                    _log.Info($"Registering health notifier: appName {p.AppName}, appVersion {p.AppVersion}, envInfo {p.EnvInfo}");

                    return BuildHealthNotifier(builderSettings, logFactory);
                }); 
            }).Value;
        }

        private IHealthNotifier BuildHealthNotifier(HealthNotifierBuilderSettings builderSettings, ILogFactory logFactory)
        {
            return new HealthNotifier(builderSettings.AppName, 
                builderSettings.AppVersion, 
                builderSettings.EnvInfo, 
                logFactory, 
                _slackNotificationsSender);
        }

        public void Dispose()
        {
            Parallel.ForEach(_healthNotifiers.Values, p => p.Value.Dispose());
        }
    }
}
