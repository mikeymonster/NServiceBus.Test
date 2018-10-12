using System;
using SFA.DAS.NServiceBus;

namespace NServiceBus.Test.Domain.Events
{
    public class StringMessageEvent : Event
    {
        public string Message { get; set; }
    }
}
