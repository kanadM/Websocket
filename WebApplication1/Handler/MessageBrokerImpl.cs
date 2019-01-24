using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Handler
{
    public interface IMessageBroker : IDisposable
    {
        void Publish<T>(object source, T message);
        void Subscribe<T>(Action<MessagePayload<T>> subscription);
        void Unsubscribe<T>(Action<MessagePayload<T>> subscription);
    }

    public class MessagePayload<T>
    {
        public object Who { get; private set; }
        public T What { get; private set; }
        public DateTime When { get; private set; }
        public MessagePayload(T payload, object source)
        {
            Who = source; What = payload; When = DateTime.UtcNow;
        }
    }


    public class MessageBrokerImpl : IMessageBroker
    {
        private static MessageBrokerImpl _instance;
        private readonly Dictionary<Type, List<Delegate>> _subscribers;
        public static MessageBrokerImpl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageBrokerImpl();
                return _instance;
            }
        }

        private MessageBrokerImpl()
        {
            _subscribers = new Dictionary<Type, List<Delegate>>();
        }

        public void Publish<T>(object source, T message)
        {
            if (message == null || source == null)
                return;

            if (!_subscribers.ContainsKey(typeof(T)))
                return;

            var delegates = _subscribers[typeof(T)];

            if (delegates == null || delegates.Count == 0)
                return;

            var payload = new MessagePayload<T>(message, source);

            foreach (var handler in delegates.Select(item => item as Action<MessagePayload<T>>))
            {
                Task.Factory.StartNew(() => handler?.Invoke(payload));
            }
        }

        public void Subscribe<T>(Action<MessagePayload<T>> subscription)
        {
            var delegates = _subscribers.ContainsKey(typeof(T)) ?
                            _subscribers[typeof(T)] : new List<Delegate>();
            if (!delegates.Contains(subscription))
            {
                delegates.Add(subscription);
            }
            _subscribers[typeof(T)] = delegates;
        }

        public void Unsubscribe<T>(Action<MessagePayload<T>> subscription)
        {
            if (!_subscribers.ContainsKey(typeof(T))) return;
            var delegates = _subscribers[typeof(T)];
            if (delegates.Contains(subscription))
                delegates.Remove(subscription);
            if (delegates.Count == 0)
                _subscribers.Remove(typeof(T));
        }

        public void Dispose()
        {
            _subscribers?.Clear();
        }
    }
    public class Test
    {
        IMessageBroker mb = MessageBrokerImpl.Instance;
        public Test()
        {

        }

        public void publishMessage(string user,string message)
        {
            mb.Publish(user,message);
            mb.Subscribe<string>(callBack);
        }

        public void callBack(MessagePayload<string> param)
        {

        }
    }
}
