using System;

namespace JobQueue.Shared
{
    public class MessageModel
    {
        public DateTime CreateDate { get; set; }

        public Guid MessageId { get; set; }

        public string Entity { get; set; }

        public string Category { get; set; }

        public object Payload { get; set; }
    }
}