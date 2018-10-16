using System;
using SFA.DAS.NServiceBus;

namespace NServiceBus.Test.Domain.Events
{
    public class StringMessageEvent : Event
    {
        public string Message { get; set; }

        public StringMessageEvent()
        {
        }

        public StringMessageEvent(string message)
        {
            Message = message;
            Created = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"'{Message}' sent at {Created:dd/MM/yy HH:mm:ss.ffffff}";
        }
    }
}
