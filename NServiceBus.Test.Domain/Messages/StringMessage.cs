using System;
using SFA.DAS.NServiceBus;

namespace NServiceBus.Test.Domain.Events
{
    public class StringMessage : IMessage
    {
        public string Text { get; set; }
    }
}
