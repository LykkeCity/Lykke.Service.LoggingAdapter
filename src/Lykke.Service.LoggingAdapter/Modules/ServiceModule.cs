using System.Collections.Generic;
using System.Linq;
using Autofac;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Service.LoggingAdapter.Core.Domain.HealthNotification;
using Lykke.Service.LoggingAdapter.Core.Domain.Log;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.Service.LoggingAdapter.Services;
using Lykke.Service.LoggingAdapter.Services.HealthNotification;
using Lykke.Service.LoggingAdapter.Services.Log;
using Lykke.Service.LoggingAdapter.Settings;
using Lykke.SettingsReader;
using Lykke.SlackNotifications;

namespace Lykke.Service.LoggingAdapter.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.LoggingAdapterService.LogInitionMaxDegreeOfParallelism))
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterLoggerFactoryStorage(builder);

            builder.RegisterType<HealthNotificationWriter>()
                .As<IHealthNotificationWriter>();

            builder.RegisterType<LogWriter>()
                .As<ILogWriter>();

            builder.RegisterType<HealthNotifierStorage>()
                .As<IHealthNotifierStorage>()
                .SingleInstance();

            builder.RegisterInstance(CreateSlackNotificationSender(_appSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString, 
                    _appSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName))
                .As<ISlackNotificationsSender>()
                .SingleInstance();
        }

        private void RegisterLoggerFactoryStorage(ContainerBuilder builder)
        {
            var configuredLoggersCount = _appSettings.CurrentValue.LoggingAdapterService.Loggers.Count();

            var builderSettings = new List<LoggerBuilderSettings>();

            for (var i = 0; i < configuredLoggersCount; i++)
            {
                var captured = i;
                var logSettingsReloadingManager = _appSettings.Nested(p => p.LoggingAdapterService.Loggers.Skip(captured).First());
                builderSettings.Add(
                    new LoggerBuilderSettings
                    {
                        AppName = logSettingsReloadingManager.CurrentValue.AppName,
                        ConnectionString = logSettingsReloadingManager.Nested(p => p.ConnString),
                        TableName =  logSettingsReloadingManager.CurrentValue.TableName,
                        SlackNotificationsConnectionString = _appSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                        SlackNotificationsQueueName = _appSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName
                    });
            }

            builder.Register(p => new LogFactoryStorage(builderSettings, p.Resolve<ILogFactory>()))
                .As<ILogFactoryStorage>()
                .SingleInstance();
        }

        private ISlackNotificationsSender CreateSlackNotificationSender(string slackAzureQueueConnectionString,string slackAzureQueuesBaseName)
        {
            var factory = new HealthNotifierSlackSenderFactory();

            return factory.Create(slackAzureQueueConnectionString, slackAzureQueuesBaseName);
        }
    }
}
