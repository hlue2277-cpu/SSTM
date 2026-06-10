using Castle.DynamicProxy;
using SSTMTerminal.Controls;
using SSTMTerminal.Helpers;
using Genesis;
using Genesis.DynamicProxy;
using Genesis.Exceptions;
using Genesis.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SSTMTerminal
{
    public class CustomizedInterceptor : ServiceProxyInterceptor
    {
        public CustomizedInterceptor(ILogger logger)
            : base(logger)
        {
        }

        public override void Intercept(IInvocation invocation)
        {
            //if (NetworkHelper.CheckNetworkStatus())
            {
                var dispatcher = Application.Current.Dispatcher;
                var isInDispatcherThread = (dispatcher.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId);

                var completed = false;
                Exception exception = null;

                Action action = () =>
                {
                    try
                    {
                        base.Intercept(invocation);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        exception = ex;
                    }
                    finally
                    {
                        completed = true;
                    }

                };

                if (isInDispatcherThread)
                {
                    Logger.Debug("start --> " + invocation.Method.DeclaringType.Name.Remove(0, 1) + "--" + invocation.Method.Name);
                    var spare = false;

                    var attrs = invocation.Method.CustomAttributes;
                    if (attrs != null && attrs.Count() > 0)
                    {
                        var name = attrs.FirstOrDefault().AttributeType.Name;
                        if (name is nameof(InterceptSpareAttribute))
                        {
                            spare = true;
                        }
                    }

                    if (!spare)
                    {
                        LoadingControlManager.Instance.IsBusy = true;
                        LoadingControlManager.Instance.Message = "加载数据中...";
                    }

                    Task.Factory.StartNew(action);

                    while (!completed)
                    {
                        dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                    }

                    if (!spare)
                    {
                        LoadingControlManager.Instance.IsBusy = false;
                        LoadingControlManager.Instance.Message = string.Empty;
                    }

                    Logger.Debug("end --> " + invocation.Method.DeclaringType.Name.Remove(0, 1) + "--" + invocation.Method.Name);

                    if (null != exception)
                    {
                        //throw exception;
                    }
                }
                else
                {
                    action.Invoke();

                    if (null != exception)
                    {
                        throw exception;
                    }
                }
            }
            //else
            //{
            //    throw new NetworkException();
            //}
        }
    }
}
