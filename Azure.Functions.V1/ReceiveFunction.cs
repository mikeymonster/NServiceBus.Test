using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using NServiceBus.Test.Domain.Events;
using NServiceBus.Test.Domain.Messages;

namespace Azure.Functions.V1
{
    public static class ReceiveFunction
    {
        [FunctionName("ServiceBusReceiveQueue")]
        public static void Run(
            [ServiceBusTrigger("das-test-endpoint", AccessRights.Manage, Connection = "ServiceBusConnectionString")]BrokeredMessage message,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            try
            {
                log.Info($"C# ServiceBus queue trigger receiving message: {message.MessageId}");

                var stringMessage =  message.DeserializeJsonMessage<StringMessage>();

                log.Info($"C# ServiceBus queue trigger function processed message: {stringMessage}");
            }
            catch (Exception e)
            {
                log.Error($"Unable to deserialize message body for StringMessage. messageId: {message.MessageId} {{ID={executionContext.InvocationId}}}", e);
                message.Defer();
            }
        }

        [FunctionName("ServiceBusReceiveTopic")]
        public static void RunTopic(
            [ServiceBusTrigger("bundle-1", "test-subscription", AccessRights.Manage, Connection = "ServiceBusConnectionString")]BrokeredMessage message,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            try
            {
                log.Info($"C# ServiceBus topic trigger receiving message: {message.MessageId}");

                var stringMessageEvent = message.DeserializeJsonMessage<StringMessageEvent>();

                log.Info($"C# ServiceBus topic trigger function processed event: {stringMessageEvent}");
            }
            catch (Exception e)
            {
                log.Error($"Unable to deserialize message body for StringMessageEvent. messageId: {message.MessageId} {{ID={executionContext.InvocationId}}}", e);
                message.Defer();
            }
        }
    }
}
