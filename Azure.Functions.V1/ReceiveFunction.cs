using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using NServiceBus.Test.Domain.Messages;

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
            try
            {
                log.Info($"C# ServiceBus queue trigger receiving message: {message.MessageId}");
                //var stringMessage = message.GetBody<StringMessage>();

                //TODO: Add a helper or extension to deserialize json
                using (var stream = message.GetBody<Stream>())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        JsonSerializer ser = new JsonSerializer();
                        var stringMessage =  ser.Deserialize<StringMessage>(jsonReader);
                        log.Info($"C# ServiceBus queue trigger function processed message: {stringMessage}");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error($"Unable to deserialize message body for message queue sent_transfer_connection_invitation. messageId: {message.MessageId} {{ID={executionContext.InvocationId}}}", e);
                //message.Defer();
            }
        }
    }
}
