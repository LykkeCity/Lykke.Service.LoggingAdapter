using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.LoggingAdapter.Client
{
    public static class AutofacExtension
    {
        public static void RegisterLoggingAdapterClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<LoggingAdapterClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ILoggingAdapterClient>()
                .SingleInstance();
        }

        public static void RegisterLoggingAdapterClient(this ContainerBuilder builder, LoggingAdapterServiceClientSettings settings, ILog log)
        {
            builder.RegisterLoggingAdapterClient(settings?.ServiceUrl, log);
        }
    }
}
