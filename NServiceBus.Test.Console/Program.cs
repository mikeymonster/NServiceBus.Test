using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Test.Domain.Configuration;
using NServiceBus.Test.Domain.Events;
using SFA.DAS.NServiceBus.AzureServiceBus;

namespace NServiceBus.Test.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var configuration = builder.Build();

                var nServiceBusSettings = new NServiceBusConfiguration();
                configuration.GetSection("NServiceBusConfiguration").Bind(nServiceBusSettings);
                var useLearningTransport = configuration.GetValue<bool>("UseLearningTransport");

                System.Console.WriteLine($"NServiceBus Settings:");
                System.Console.WriteLine($"    UseLearningTransport {useLearningTransport}");
                System.Console.WriteLine($"    Local Endpoint {nServiceBusSettings.LocalEndpoint}");
                System.Console.WriteLine($"    Endpoint {nServiceBusSettings.Endpoint}");
                System.Console.WriteLine($"    Service bus {nServiceBusSettings.ServiceBusConnectionString}");
                System.Console.WriteLine($"    License bus {nServiceBusSettings.LicenceText}");
                
                //var connectionStrings = configuration.GetSection("ConnectionStrings");
                //var connectionString = configuration.GetConnectionString("RemoteConnection");
                //var repository = new DataLockRepository(connectionString);

                await SendMessage("Hello World", nServiceBusSettings, useLearningTransport);
            }
            catch (Exception ex)
            {
                Debug.Print($"An exception has occurred. {ex}");
                System.Console.WriteLine($"An exception has occurred. {ex}");
            }

            System.Console.WriteLine("...");
            System.Console.ReadKey();
        }

        public static async Task SendMessage(string messageText, NServiceBusConfiguration nServiceBusSettings, bool useLearningTransport = false)
        {
            var endpointConfiguration = new EndpointConfiguration(nServiceBusSettings.LocalEndpoint);
                
            endpointConfiguration.License(nServiceBusSettings.LicenceText);

            if (useLearningTransport)
            {
                endpointConfiguration.UseTransport<LearningTransport>();
            }

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            var options = new SendOptions();
            options.SetDestination(nServiceBusSettings.Endpoint);

            var message = new StringMessage { Text = messageText };
            await endpointInstance.Send(message, options)
                .ConfigureAwait(false);

            //var message = new StringMessageEvent { Message = messageText };
            //await endpointInstance.Publish(message)
            //    .ConfigureAwait(false);
        }
    }
}
