using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using NServiceBus;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;

namespace Azure.Functions.V1.NServiceBus
{
    public class NServiceBusSubscriptionAttributeBinding : IBinding
    {
        private readonly ParameterInfo _parameterInfo;
        private NServiceBusSubscriptionAttribute _attribute;

        //public string Connection => _attribute.Connection;

        //public string Endpoint => _attribute.Endpoint;

        //public string Subscription => _attribute.Subscription;

        public NServiceBusSubscriptionAttributeBinding(ParameterInfo parameterInfo, NServiceBusSubscriptionAttribute attribute)
        {
            _parameterInfo = parameterInfo;
            _attribute = attribute;

            StartSubscription(attribute.Connection, attribute.Endpoint).GetAwaiter().GetResult();
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult<IValueProvider>(new NServiceBusSubscriptionAttributeValueProvider(_parameterInfo));
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            return Task.FromResult<IValueProvider>(new NServiceBusSubscriptionAttributeValueProvider(_parameterInfo));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameterInfo.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = "NServiceBus subscription service",
                    DefaultValue = "NServiceBus subscription services",
                    Prompt = "NServiceBus subscription services"
                }
            };
        }

        private async Task StartSubscription(string connectionStringName, string endPointName)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable(connectionStringName, EnvironmentVariableTarget.Process);

                bool endpointAlreadyConfigured = false; //TODO: Get this from Azure somehow
                if (!endpointAlreadyConfigured)
                {
                    var endpointConfiguration = new EndpointConfiguration(endPointName)
                        .UseInstallers()
                        .UseAzureServiceBusTransport(false, () => connectionString, r => { })
                        .UseNewtonsoftJsonSerializer();

                    var endpointInstance
                        = await
                            Endpoint
                                .Start(endpointConfiguration)
                                .ConfigureAwait(false);

                    Console.WriteLine($"Started endpoint {endPointName}");

                    var stop = true;
                    if (stop)
                    {
                        await
                            endpointInstance
                                .Stop()
                                .ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
        }
    }
}