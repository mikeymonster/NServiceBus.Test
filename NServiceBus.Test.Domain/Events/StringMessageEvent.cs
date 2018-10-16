using System;
using SFA.DAS.NServiceBus;

namespace NServiceBus.Test.Domain.Events
{
    public class StringMessageEvent : Event
    {
        public string Message { get; private set; }

        public DateTime TimeCreated { get; private set; }

        public StringMessageEvent()
        {
        }

        public StringMessageEvent(string message)
        {
            Message = message;
            TimeCreated = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"'{Message}' sent at {TimeCreated:dd/MM/yy HH:mm}";
        }
    }
}
