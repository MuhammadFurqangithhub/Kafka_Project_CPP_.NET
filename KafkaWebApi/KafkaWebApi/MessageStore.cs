using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KafkaWebApi
{
    public class MessageStore
    {
        private readonly ConcurrentQueue<string> _messages = new();

        public void AddMessage(string message)
        {
            _messages.Enqueue(message);
            while (_messages.Count > 100) _messages.TryDequeue(out _); // Keep latest 100 messages
        }

        public IEnumerable<string> GetMessages()
        {
            return _messages.ToArray();
        }
    }
}
