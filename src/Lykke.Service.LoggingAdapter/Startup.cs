﻿using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeAzureTable;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Service.LoggingAdapter.Core.Services;
using Lykke.Service.LoggingAdapter.Settings;
using Lykke.Service.LoggingAdapter.Modules;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Lykke.MonitoringServiceApiCaller;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Lykke.Service.LoggingAdapter
{
    public class Startup
    {

        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfiguration Configuration { get; }
        public ILog Log { get; private set; }
        private IHealthNotifier HealthNotifier { get; set; }

        private bool NoSettings { get; set; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            Configuration = configuration;
            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(){CamelCaseText = true});
                    });

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration("v1", "LoggingAdapter API");
                });

                var builder = new ContainerBuilder();

                NoSettings = Configuration.GetSection("NoSettings").Get<bool>();
                if (NoSettings)
                {
                    //TODO - register in DI
                    Log = DirectConsoleLogFactory.Instance.CreateLog(this);
                }
                else
                {
                    var appSettings = Configuration.LoadSettings<AppSettings>();


                    services.AddLykkeHealthNotifications(
                        appSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                        appSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName);

                    services.AddLykkeLogging(logging =>
                    {
                        logging.AddLykkeConsole();
                        logging.AddLykkeAzureTable(appSettings.ConnectionString(x => x.LoggingAdapterService.Db.LogsConnString), "LoggingAdapter" + Program.EnvInfo);
                        logging.AddLykkeEssentialSlackChannels(
                            appSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                            appSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName);
                    });

                }

                builder.Populate(services);
                builder.RegisterModule(new ServiceModule());
                ApplicationContainer = builder.Build();

                Log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(this);
                HealthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();
                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                Log?.Critical(ex);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeForwardedHeaders();
                app.UseLykkeMiddleware(ex => ErrorResponse.Create("Technical problem"));

                app.UseMvc();
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                });
                app.UseSwaggerUI(x =>
                {
                    x.RoutePrefix = "swagger/ui";
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
                app.UseStaticFiles();

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                Log?.Critical(ex);
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                // NOTE: Service not yet recieve and process requests here

                await ApplicationContainer.Resolve<IStartupManager>().StartAsync();

                HealthNotifier.Notify($"Env: {Program.EnvInfo} started");
            }
            catch (Exception ex)
            {
                Log?.Error(nameof(StartApplication), ex);

                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                // NOTE: Service still can recieve and process requests here, so take care about it if you add logic here.

                await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
            }
            catch (Exception ex)
            {
                Log?.Error(nameof(StopApplication), ex);

                throw;
            }
        }

        private async Task CleanUp()
        {
            try
            {
                // NOTE: Service can't recieve and process requests here, so you can destroy all resources

                if (Log != null)
                {
                    HealthNotifier.Notify($"Env: {Program.EnvInfo} Terminating");
                }

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Critical(ex);
                    (Log as IDisposable)?.Dispose();
                }
                throw;
            }
        }
    }
}
