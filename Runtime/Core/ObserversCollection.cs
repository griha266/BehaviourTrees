using System;
using System.Collections.Generic;

namespace Shipico.BehaviourTrees
{
    internal class ObserversCollection
    {
        private readonly Dictionary<Type, List<object>> _observers;

        public void PushEvent<T>(T eventData)
        {
            var type = typeof(T);
            if (!_observers.ContainsKey(type))
            {
                return;
            }
            
            var observers = _observers[type];
            for (var i = 0; i < observers.Count; i++)
            {
                var observer = observers[i] as IBehaviourTreeEventsObserver<T>;
                observer?.OnNextEvent(eventData);
            }
        }

        public IDisposable AddObserver<T>(IBehaviourTreeEventsObserver<T> observer)
        {
            var eventType = typeof(T);
            if (!_observers.ContainsKey(eventType))
            {
                _observers.Add(eventType, new List<object>());
            }
            
            _observers[eventType].Add(observer);

            return new Subscription(this, eventType, observer);
        }
        
        private readonly struct Subscription : IDisposable
        {
            private readonly Type _eventType;
            private readonly object _observer;
            private readonly ObserversCollection _collection;

            public Subscription(ObserversCollection collection, Type eventType, object observer)
            {
                _collection = collection;
                _eventType = eventType;
                _observer = observer;
            }

            public void Dispose()
            {
                _collection._observers[_eventType].Remove(_observer);
            }
        }
    }
}