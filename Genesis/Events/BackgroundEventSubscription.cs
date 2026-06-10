using System;
using System.Threading;

namespace Genesis.Events
{
    public class BackgroundEventSubscription<TPayload> :EventSubscription<TPayload>
    {
        public BackgroundEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
            :base(actionReference, filterReference)
        {
        }

        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            ThreadPool.QueueUserWorkItem((o) => action(argument));
        }
    }
}
