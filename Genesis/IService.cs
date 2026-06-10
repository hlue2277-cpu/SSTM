using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesis.Logging;
using Genesis.DynamicProxy;

namespace Genesis
{
    /// <summary>
    /// service manager接口
    /// </summary>
    [Intercept(typeof(IServiceProxyInterceptor))]
    public interface IService : IDependency
    {
        ILogger Logger { get; set; }
    }
}
