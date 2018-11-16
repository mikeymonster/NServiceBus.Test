using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.ServiceBus;
using TokenProvider = Microsoft.Azure.ServiceBus.Primitives.TokenProvider;

namespace Azure.Functions.V1.Extensions
{
    public class NServiceBusConfigurationExtensionConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            Console.WriteLine("Initializing NServiceBusConfigurationExtensionConfigProvider");

            LoadNServiceBusConfiguration();

            //These should come from attribute

        }

        public async Task LoadNServiceBusConfiguration()
        {
            string connectionStringName = "ServiceBusConnectionString";
            string queueName = "das-test-string-messages";

            //Load assemblies
            //var assembly = typeof();
            var assemblies = GetUserAssemblies();
            foreach (var assembly in assemblies)
            {
                //var assembly = assemblies.FirstOrDefault(a => a.Name == "Azure.Functions.V1");
                Type[] types = this.FindTypes(assembly);
                if (types != null)
                {
                    foreach (var type in types.Where<Type>(new Func<Type, bool>(IsJobClass)))
                    {
                        //Look for the attribute on methods
                        foreach (var method in type.GetMethods())
                        {
                            foreach (var attribute in method.GetCustomAttributes())
                            {
                                if (attribute is NServiceBusConfigurationAttribute)
                                {
                                    CreateConfiguration((NServiceBusConfigurationAttribute) attribute);
                                }
                            }

                        }
                    }
                }
            }


            /*
             var eventName = "MyEvent";
var functionName = "Functionname";
var subscriptionDescription = new SubscriptionDescription("bundle-1", functionName) {
    UserMetadata = $"Events {functionName} is subscribed to"
};
var description = await managementClient.CreateSubscriptionAsync(subscriptionDescription);
await managementClient.DeleteRuleAsync(BundleName, description.SubscriptionName, "$Default");
var rule = new RuleDescription();
rule.Name = eventName;
rule.Filter = new SqlFilter($"[NServiceBus.EnclosedMessageTypes] LIKE '%{eventName}%'");
await managementClient.CreateRuleAsync("bundle-1", description.SubscriptionName, rule);
             */
        }

        private async Task CreateConfiguration(NServiceBusConfigurationAttribute attribute)
        {
            var connectionString = Environment.GetEnvironmentVariable(attribute.Connection, EnvironmentVariableTarget.Process);

            //var tokenProvider = GetTokenProvider(attribute.Connection);

            var managementClient = new ManagementClient(connectionString);

            var queues = await managementClient.GetQueuesAsync();

        }

        private static IEnumerable<Assembly> GetUserAssemblies()
        {
            return (IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies();
        }
        
        public static bool IsJobClass(Type type)
        {
            if (type == (Type)null || !type.IsClass || type.IsAbstract && !type.IsSealed || !type.IsPublic)
                return false;
            return !type.ContainsGenericParameters;
        }

        private Type[] FindTypes(Assembly assembly)
        {
            //if (!CustomTypeLocator.AssemblyReferencesSdkOrExtension(assembly, extensionAssemblies))
            //    return (Type[])null;
            Type[] typeArray = (Type[])null;
            try
            {
                typeArray = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                //this._log.WriteLine("Warning: Only got partial types from assembly: {0}", (object)assembly.FullName);
                //this._log.WriteLine("Exception message: {0}", (object)ex.ToString());
                typeArray = ex.Types;
            }
            catch (Exception ex)
            {
                //this._log.WriteLine("Warning: Failed to get types from assembly: {0}", (object)assembly.FullName);
                //this._log.WriteLine("Exception message: {0}", (object)ex.ToString());
            }
            return typeArray;
        }
        private TokenProvider GetTokenProvider(string connectionStringName)
        {
            var connectionString = Environment.GetEnvironmentVariable(connectionStringName, EnvironmentVariableTarget.Process);

            bool endpointAlreadyConfigured = false;

            //ignore case?
            var tokenizerRegex = new Regex("^Endpoint=(?<endpoint>.*);SharedAccessKeyName=(?<sharedAccessKeyName>.*);SharedAccessKey=(?<sharedAccessKey>.*)$");
            var match = tokenizerRegex.Match(connectionString);
            if (!match.Success)
            {
                //throw new ConfigurationErrorsException("Connection string could not be parsed.");
                throw new Exception("Connection string could not be parsed.");
            }

            var endpointAddress = match.Groups["endpoint"].Value;
            var sharedAccessKeyName = match.Groups["sharedAccessKeyName"].Value;
            var sharedAccessKey = match.Groups["sharedAccessKey"].Value;

            Debug.WriteLine($"Found match endpointAddress='{endpointAddress}', sharedAccessKeyName ='{sharedAccessKeyName}', sharedAccessKey='{sharedAccessKey}'");

            //https://github.com/Particular/NServiceBus.AzureServiceBus/issues/667
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sharedAccessKeyName, sharedAccessKey);
            //var namespaceManager = new NamespaceManager(endpointAddress, tokenProvider);

            return tokenProvider;
        }
    }
}
