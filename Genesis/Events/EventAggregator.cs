using System;
using System.Collections.Generic;

namespace Genesis.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            EventBase existingEvent = null;

            if (!this.events.TryGetValue(typeof(TEventType), out existingEvent))
            {
                TEventType newEvent = new TEventType();
                this.events[typeof(TEventType)] = newEvent;

                return newEvent;
            }
            else
            {
                return (TEventType)existingEvent;
            }
        }
    }
}
