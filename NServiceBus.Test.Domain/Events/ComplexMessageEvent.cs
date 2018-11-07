using System;
using System.Diagnostics;
using SFA.DAS.NServiceBus;

namespace NServiceBus.Test.Domain.Events
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ComplexMessageEvent : Event
    {
        public string Description { get; set; }

        public string UserName { get; set; }

        public int Count { get; set; }

        public ComplexMessageEvent()
        {
        }

        public ComplexMessageEvent(string description, string username, int count)
        {
            Description = description;
            UserName = username;
            Count = count;
            Created = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"'{Description}' count {Count} for user {UserName} sent at {Created:dd/MM/yy HH:mm:ss.ffffff}";
        }

        private string DebuggerDisplay => ToString();

    }
}
