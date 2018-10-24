using System;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Azure.Functions.V1.Extensions
{
    public class TestExtensionConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            Console.WriteLine("Initializing TestExtensionConfigProvider");
        }
    }
}
