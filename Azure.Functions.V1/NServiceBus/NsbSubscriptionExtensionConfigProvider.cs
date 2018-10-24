using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using NServiceBus;

namespace Azure.Functions.V1.NServiceBus
{
    public class NsbSubscriptionExtensionConfigProvider : IExtensionConfigProvider
    {
        //https://stackoverflow.com/questions/46553687/azure-function-run-code-on-startup
        //If we have Binding attributes, like Inject, the extension code provider should load; otherwise we need to define the path:
        //https://blog.wille-zone.de/post/azure-functions-iextensionconfigprovider-resolution-and-instantiates/

        private readonly NServiceBusSubscriptionAttributeBindingProvider _bindingProvider;

        public NsbSubscriptionExtensionConfigProvider()
        {
            _bindingProvider = new NServiceBusSubscriptionAttributeBindingProvider();
        }

        public void Initialize(ExtensionConfigContext context)
        {
            Console.WriteLine("Initializing NsbSubscriptionExtensionConfigProvider");

            context.Config.RegisterBindingExtensions(_bindingProvider);
        }
    }
}
