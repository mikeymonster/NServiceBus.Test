using System;

namespace NServiceBus.Test.Domain.Messages
{
    public class StringMessage : IMessage
    {
        public string Text { get; set; }

        public DateTime TimeCreated { get; set; }

        public StringMessage()
        {
        }

        public StringMessage(string message)
        {
            Text = message;
            TimeCreated = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"'{Text}' sent at {TimeCreated:dd/MM/yy HH:mm:ss.ffffff}";
        }
    }
}
