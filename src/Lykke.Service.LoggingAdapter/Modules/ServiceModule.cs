﻿using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.Service.LoggingAdapter.Services;
using Lykke.Service.LoggingAdapter.Services.Log;
using Lykke.Service.LoggingAdapter.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.LoggingAdapter.Modules
{
    public class ServiceModule : Module
    {
        private readonly IServiceCollection _services;
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterLoggerFactoryStorage(builder);

            builder.RegisterType<LoggerSelector>()
                .As<ILoggerSelector>();
            
            builder.Populate(_services);
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

            var storageInstance = new LogFactoryStorage(builderSettings);

            builder.RegisterInstance(storageInstance).As<ILogFactoryStorage>();
        }
    }
}
