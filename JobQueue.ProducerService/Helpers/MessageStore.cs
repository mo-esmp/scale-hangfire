using JobQueue.ProducerService.Models;
using System.Collections.Generic;
using System.Linq;

namespace JobQueue.ProducerService
{
    internal class MessageStore
    {
        private readonly List<MessageModel> _store = new();
        private static readonly MessageStore _instance = new();

        private MessageStore()
        {
        }

        public static MessageStore Instance => _instance;

        public int Count => _store.Count;

        public void AddMessages(IEnumerable<MessageModel> messages)
        {
            _store.AddRange(messages);
        }

        public IEnumerable<MessageModel> GetMessages(int count)
        {
            var message = _store.Take(count).ToList();
            _store.RemoveRange(0, message.Count);

            return message;
        }
    }
}