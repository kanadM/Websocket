using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public class WebsocketStore<T, W> where T : class
    {
        private readonly ConcurrentDictionary<T, W> _Bag;
        private readonly int _queueLimit = 10000;
        public ICollection<W> Values { get { return _Bag.Values; } }


        public WebsocketStore()
        {
            _Bag = new ConcurrentDictionary<T, W>();
        }

        public bool CanAdd(T key)
        {
            return !_Bag.TryGetValue(key, out W x);
        }
        public bool Add(T key, W Val)
        {
            if (_Bag.Count < _queueLimit)
            {
                _Bag[key] = Val;
                return true;
            }
            return false;
        }


        public bool Get(T key, out W val)
        {
            val = default(W);
            if (_Bag.Count > 0)
            {
                return _Bag.TryGetValue(key, out val);
            }
            return false;
        }

        public W Remove(T key)
        {
            W x = default(W);
            if (_Bag.Count > 0)
            {
                _Bag.Remove(key, out x);
            }
            return x;
        }

        public int Count { get { return _Bag.Count; } }
    }
}
