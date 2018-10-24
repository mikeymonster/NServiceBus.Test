using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Azure.Functions.V1.NServiceBus
{
    internal class NServiceBusSubscriptionAttributeValueProvider : IValueProvider
    {
        private readonly ParameterInfo _parameterInfo;

        public NServiceBusSubscriptionAttributeValueProvider(ParameterInfo parameterInfo)
        {
            _parameterInfo = parameterInfo;
        }

        public Type Type => _parameterInfo.ParameterType;

        public Task<object> GetValueAsync()
        {
            return Task.FromResult(new object());
        }

        public string ToInvokeString()
        {
            return Type.ToString();
        }
    }
}