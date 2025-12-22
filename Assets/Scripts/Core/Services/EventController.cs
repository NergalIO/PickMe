using System;
using System.Collections.Generic;
using PickMe.Core.Infrastructure;
using UnityEngine;

namespace PickMe.Core.Services
{
    /// <summary>
    /// Central event bus. All game events should pass through here.
    /// Supports strongly typed publish/subscribe per event type.
    /// </summary>
    public class EventController : PersistentSingleton<EventController>
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        /// <summary>
        /// Subscribe to events of type T.
        /// </summary>
        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }

            if (!list.Contains(handler))
            {
                list.Add(handler);
            }
        }

        /// <summary>
        /// Unsubscribe from events of type T.
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    _handlers.Remove(type);
                }
            }
        }

        /// <summary>
        /// Publish an event instance to all subscribers of its type.
        /// </summary>
        public void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                // iterate on copy to avoid modification during dispatch
                var snapshot = list.ToArray();
                foreach (var del in snapshot)
                {
                    if (del is Action<T> callback)
                    {
                        callback.Invoke(evt);
                    }
                }
            }
        }
    }
}

