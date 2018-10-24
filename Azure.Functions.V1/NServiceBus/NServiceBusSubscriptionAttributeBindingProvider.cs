using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Azure.Functions.V1.NServiceBus
{
    public class NServiceBusSubscriptionAttributeBindingProvider : IBindingProvider
    {
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameterInfo = context.Parameter;
            var attribute = parameterInfo.GetCustomAttribute<NServiceBusSubscriptionAttribute>();
            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            return Task.FromResult<IBinding>(new NServiceBusSubscriptionAttributeBinding(parameterInfo, attribute));
        }
    }
}
