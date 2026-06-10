using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Genesis.UI;
using Genesis.Logging;
using System.Windows;

namespace Genesis.Exceptions
{
    public abstract class ExceptionHandlerBase : IExceptionHandler
    {
        public ILifetimeScope Container { get; set; }
        public ILogger Logger { get; set; }

        public ExceptionHandlerBase() { }

        public ExceptionHandlerBase(ILifetimeScope container, ILogger logger)
            : this()
        {
            Container = container;
            Logger = logger;
        }

        public virtual bool HandleException(object sender, System.Exception exception) { return false; }

        protected void CloseApplication(object payload)
        {
            Application.Current.Shutdown();
        }
    }
}
