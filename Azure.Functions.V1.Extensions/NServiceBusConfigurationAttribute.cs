using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Functions.V1.Extensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NServiceBusConfigurationAttribute : Attribute
    {
        public string Connection { get; private set; }

        public string Queue { get; private set; }

        public string Subscription { get; private set; }

        public Type MessageType { get; private set; }

        public NServiceBusConfigurationAttribute(string queue, string subscription, Type messageType, string connection)
        {
            Connection = connection;
            Queue = queue;
            Subscription = subscription;
            MessageType = messageType;
        }
    }
}
