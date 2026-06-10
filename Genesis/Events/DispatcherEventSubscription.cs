using System;

namespace Genesis.Events
{
    public class DispatcherEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        private readonly IDispatcherFacade _dispatcher;

        public DispatcherEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, IDispatcherFacade dispatcher)
            : base(actionReference, filterReference)
        {
            _dispatcher = dispatcher;
        }

        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            _dispatcher.BeginInvoke(action, argument);
        }
    }
}
