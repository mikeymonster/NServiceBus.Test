using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Test.Domain.Configuration;
using NServiceBus.Test.Domain.Events;
using NServiceBus.Test.Domain.Messages;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using StructureMap.TypeRules;

namespace NServiceBus.Test.Console
{
    class Program
    {
        static async Task Main(string[] args)
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
            System.Console.WriteLine($"    License {nServiceBusSettings.LicenceText}");

            var runOptionKey = GetRunOption();
            while (runOptionKey != ConsoleKey.Q && runOptionKey != ConsoleKey.Escape)
            {
                try
                {
                    switch (runOptionKey)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            await SendMessage("Hello World", nServiceBusSettings, useLearningTransport);
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                        case ConsoleKey.Enter: //Default
                            await SendMessageToAzure("Hello World", nServiceBusSettings, false);
                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            await PublishEventToAzure("Hello World", nServiceBusSettings, false);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print($"An exception has occurred. {ex}");
                    System.Console.WriteLine($"An exception has occurred. {ex}");
                }

                System.Console.WriteLine("....");
                runOptionKey = GetRunOption();
            }
        }

        private static ConsoleKey GetRunOption()
        {
            System.Console.WriteLine("Options:");
            System.Console.WriteLine("    1 to send message to local folder");
            System.Console.WriteLine("    2 to send message to Azure:");
            System.Console.WriteLine("    3 to send event to Azure:");
            System.Console.WriteLine("    Esc or Q to exit.");

            return System.Console.ReadKey().Key;
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

            var message = new StringMessage(messageText);
            await endpointInstance.Send(message, options)
                .ConfigureAwait(false);

            System.Console.WriteLine($"Sent message: {message}");
        }

        public static async Task SendMessageToAzure(string messageText, NServiceBusConfiguration nServiceBusSettings, bool useLearningTransport = false)
        {
            var endpointConfiguration = new EndpointConfiguration("ClientEndpoint")
                .UseLicense(nServiceBusSettings.LicenceText)
                .UseAzureServiceBusTransport(false,
                    () => nServiceBusSettings.ServiceBusConnectionString,
                    r => r.RouteToEndpoint(typeof(StringMessage).GetAssembly(), nServiceBusSettings.Endpoint))
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.SendOnly();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            var message = new StringMessage(messageText);
            await endpointInstance.Send(message) //, options)
                .ConfigureAwait(false);

            System.Console.WriteLine($"Sent message: {message}");
        }

        public static async Task PublishEventToAzure(string messageText, NServiceBusConfiguration nServiceBusSettings,
            bool useLearningTransport = false)
        {
            var endpointConfiguration = new EndpointConfiguration(nServiceBusSettings.Endpoint)
                    .UseLicense(nServiceBusSettings.LicenceText)
                    .UseAzureServiceBusTransport(false, () => nServiceBusSettings.ServiceBusConnectionString,
                    r =>
                    {
                        r.RouteToEndpoint(typeof(StringMessageEvent), nServiceBusSettings.Endpoint + "/my-topic");
                    })
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.SendOnly();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            //var options = new PublishOptions();
            
            var message = new StringMessageEvent(messageText);
            
            await endpointInstance.Publish(message) //, options)
                .ConfigureAwait(false);

            System.Console.WriteLine($"Published event: {message}");
        }
    }
}
