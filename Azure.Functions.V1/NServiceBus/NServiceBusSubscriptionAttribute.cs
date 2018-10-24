using System;
using Microsoft.Azure.WebJobs.Description;

namespace Azure.Functions.V1.NServiceBus
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class NServiceBusSubscriptionAttribute : Attribute
    {
        public string Connection { get; private set; }

        public string Endpoint { get; private set; }

        public string Subscription { get; private set; }

        public NServiceBusSubscriptionAttribute(string endpoint, string subscription, string connection)
        {
            Endpoint = endpoint;
            Subscription = subscription;
            Connection = connection;
        }
    }
}
