using System;
using System.Diagnostics;
using Azure.Functions.V1.Extensions;
using Azure.Functions.V1.NServiceBus;
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
            [ServiceBusTrigger("das-test-endpoint", AccessRights.Manage, Connection = "ServiceBusConnectionString")]
            BrokeredMessage message,
            [NServiceBusSubscription("das-test-endpoint", "das-test-subscription", "ServiceBusConnectionString")]
            object subscription,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger receiving message: {message.MessageId}");

            //try
            //{
            //    var stringMessage = message.DeserializeJsonMessage<StringMessage>();
            //    log.Info($"C# ServiceBus queue trigger function processed message: {stringMessage}");
            //}
            //catch
            //{
            try
            {
                Debug.WriteLine($"Receiving message - content type '{message.ContentType}', label '{message.Label}', delivery count {message.DeliveryCount}");
                foreach (var property in message.Properties)
                {
                    Debug.WriteLine($"    {property.Key} = {property.Value}");
                }

                var stringMessageEvent = message.DeserializeJsonMessage<StringMessageEvent>();
                log.Info($"C# ServiceBus queue trigger function processed event: {stringMessageEvent}");
            }
            catch (Exception e)
            {
                log.Error($"Unable to deserialize message body for StringMessage. messageId: {message.MessageId} {{ID={executionContext.InvocationId}}}", e);
                message.Defer();
            }
            //}
        }

        /*
        [FunctionName("ServiceBusReceiveTopic")]
        public static void RunTopic(
            [ServiceBusTrigger("bundle-1", "test-subscription", AccessRights.Manage, Connection = "ServiceBusConnectionString")]BrokeredMessage message,
            ExecutionContext executionContext,
            TraceWriter log
            //, [Inject]ISomething something
            )
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
        */
    }
}
