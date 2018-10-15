using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;

namespace Azure.Functions.V1
{
    public static class ReceiveFunction
    {

        [FunctionName("ServiceBusReceiveFunctionV2")]
        public static void Run(
            [ServiceBusTrigger("das-test-endpoint", AccessRights.Manage, Connection = "ServiceBusConnectionString")]BrokeredMessage message,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {message}");
        }
    }
}
