using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Test.Domain.Events;

namespace Azure.Functions.V1.NServiceBus
{
    public class StringMessageEventHandler : IHandleMessages<StringMessageEvent>
    {
        public Task Handle(StringMessageEvent message, IMessageHandlerContext context)
        {
            //Get logger;
            return Task.CompletedTask;
        }
    }
}
