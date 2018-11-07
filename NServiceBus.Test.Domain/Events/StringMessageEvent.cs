using System;
using System.Diagnostics;
using SFA.DAS.NServiceBus;

namespace NServiceBus.Test.Domain.Events
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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
            return $"'{Message}' created {Created:dd/MM/yy HH:mm:ss.ffffff}";
        }

        private string DebuggerDisplay => ToString();
    }
}
