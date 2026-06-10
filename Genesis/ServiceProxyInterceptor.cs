using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Genesis.Logging;

namespace Genesis
{
    public interface IServiceProxyInterceptor : IInterceptor, IDependency
    {
        // Empty.
    }

    public abstract class ServiceProxyInterceptor : IServiceProxyInterceptor
    {
        public ILogger Logger { get; set; }

        public ServiceProxyInterceptor(ILogger logger)
        {
            Logger = logger;
        }

        public virtual void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}
