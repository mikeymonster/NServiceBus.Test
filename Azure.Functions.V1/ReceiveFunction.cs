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
        [FunctionName("ServiceBusReceiveStringMessageFromQueue")]
        [NServiceBusConfiguration(queue: "das-test-string-messages", subscription: "das-test-subscription", messageType: typeof(StringMessageEvent), connection: "ServiceBusConnectionString")]
        public static void RunStringMessage(
            [ServiceBusTrigger("das-test-string-messages", AccessRights.Manage, Connection = "ServiceBusConnectionString")]
            BrokeredMessage message,
            //[NServiceBusSubscription("das-test-endpoint", "das-test-subscription", "ServiceBusConnectionString")]
            //object subscription,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger receiving message: {message.MessageId}");

            try
            {
                //Debug.WriteLine($"Receiving message - content type '{message.ContentType}', label '{message.Label}', delivery count {message.DeliveryCount}");
                //foreach (var property in message.Properties)
                //{
                //    Debug.WriteLine($"    {property.Key} = {property.Value}");
                //}

                var stringMessageEvent = message.DeserializeJsonMessage<StringMessageEvent>();
                log.Info($"C# ServiceBus queue trigger function processed event: {stringMessageEvent}");
            }
            catch (Exception e)
            {
                log.Error($"Unable to deserialize message body for StringMessage. messageId: {message.MessageId} {{ID={executionContext.InvocationId}}}", e);
                message.Defer();
            }
        }

        [FunctionName("ServiceBusReceiveComplexMessageFromQueue")]
        public static void RunComplexMessage(
            [ServiceBusTrigger("das-test-complex-messages", AccessRights.Manage, Connection = "ServiceBusConnectionString")]
            BrokeredMessage message,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger receiving message: {message.MessageId}");

            try
            {
                var complexMessageEvent = message.DeserializeJsonMessage<ComplexMessageEvent>();
                log.Info($"C# ServiceBus queue trigger function processed event: {complexMessageEvent}");
            }
            catch (Exception e)
            {
                log.Error($"Unable to deserialize message body for StringMessage. messageId: {message.MessageId} {{ID={executionContext.InvocationId}}}", e);
                message.Defer();
            }
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
