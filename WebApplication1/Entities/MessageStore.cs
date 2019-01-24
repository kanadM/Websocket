using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public class MessageStore<T> where T : class
    {
        private readonly ConcurrentBag<T> _Bag;
        private readonly int _queueLimit = 10000;

        public MessageStore()
        {
            _Bag = new ConcurrentBag<T>();
        }

        public bool Enqueue(T hotelChangeInfo)
        {
            if (_Bag.Count < _queueLimit)
            {
                _Bag.Add(hotelChangeInfo);
                return true;
            }
            return false;
        }

        public T Dequeue()
        {
            if (_Bag.Count > 0)
            {
                T hotelChangeInfo;
                return _Bag.TryTake(out hotelChangeInfo) ? hotelChangeInfo : null;
            }
            return null;
        }

        public int Count { get { return _Bag.Count; } }
    }
}
