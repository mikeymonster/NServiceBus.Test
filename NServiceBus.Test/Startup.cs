﻿using System;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NServiceBus.Test.Application;
using NServiceBus.Test.Domain.Configuration;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox;
//using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.UnitOfWork;
using SFA.DAS.UnitOfWork.Mvc;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.UnitOfWork.NServiceBus;
using SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox;
using StructureMap;

namespace NServiceBus.Test
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles()
                .UseUnitOfWork()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<DbConnection>(provider => 
                new SqlConnection(Configuration.GetConnectionString("DatabaseConnectionString")));

            //Need the serviceprovider so we can get the DBConnection back 
            var sp = services.BuildServiceProvider();

            services.AddMvc();

            var container = ConfigureIOC(services);

            var nServiceBusSettings = new NServiceBusConfiguration();
            Configuration.GetSection("NServiceBusConfiguration").Bind(nServiceBusSettings);

            var useLearningTransport = Configuration.GetValue<bool>("UseLearningTransport");

            var endpointConfiguration = new EndpointConfiguration(nServiceBusSettings.Endpoint)
                //.UseAzureServiceBusTransport(useLearningTransport, () => nServiceBusSettings.ServiceBusConnectionString, r => { })
                .UseAzureServiceBusTransport(useLearningTransport,
                    () => nServiceBusSettings.ServiceBusConnectionString,
                    r =>
                    {
                        //r.RouteToEndpoint(typeof(StringMessage),
                        //    nServiceBusSettings.Endpoint);
                        //r.RouteToEndpoint(typeof(StringMessageEvent),
                        //    nServiceBusSettings.Endpoint + "/my-topic");
                    })
                .UseLicense(nServiceBusSettings.LicenceText)
                .UseInstallers()
                .UseSqlServerPersistence(() => sp.GetService<DbConnection>())
                .UseStructureMapBuilder(container)
                .UseNewtonsoftJsonSerializer()
                //.UseErrorQueue()
                //.UseNLogFactory()
                .UseOutbox()
                .UseUnitOfWork();

            services.AddNServiceBus(endpointConfiguration);

            container.Populate(services);

            return container.GetInstance<IServiceProvider>();
        }
        
        private IContainer ConfigureIOC(IServiceCollection services)
        {
            var container = new global::StructureMap.Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.WithDefaultConventions();
                    _.SingleImplementationsOfInterface();
                });


                config.For<IConfiguration>().Use(Configuration);
                
                //config.Scan(scanner =>
                //{
                //});

                config.For<ISender>().Use<Sender>();

                config.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
                config.AddRegistry<UnitOfWorkRegistry>();
                
                config
                    .For<DbTransaction>()
                    .Use(c => GetSqlSessionFromContext(c).Transaction);
            });

            return container;
        }

        private ISqlStorageSession GetSqlSessionFromContext(IContext iocContext)
        {
            var unitOfWorkContext = iocContext.GetInstance<IUnitOfWorkContext>();

            //var maybeClientSession = unitOfWorkContext.TryGet<IClientOutboxTransaction>();
            var maybeClientSession = unitOfWorkContext.Get<IClientOutboxTransaction>();

            if (maybeClientSession != null)
                return maybeClientSession.GetSqlSession();

            return
                unitOfWorkContext
                    .Get<SynchronizedStorageSession>()
                    .GetSqlSession();
        }
    }
}
