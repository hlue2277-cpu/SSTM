using System;

namespace Genesis.Events
{
    public interface IDispatcherFacade
    {
        void BeginInvoke(Delegate method, object arg);
    }
}
