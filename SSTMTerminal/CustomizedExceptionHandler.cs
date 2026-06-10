using Autofac;
using SSTMTerminal.ViewModels;
using Genesis.Exceptions;
using Genesis.Logging;
using Genesis.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Genesis;
using System.Windows;
using SSTMTerminal.Images;

namespace SSTMTerminal
{
    public class CustomizedExceptionHandler : ExceptionHandlerBase
    {
        public IMsgWithoutCountDownViewModel NotWorkingView { get; set; }

        public CustomizedExceptionHandler(ILifetimeScope container, ILogger logger)
            : base(container, logger)
        {
        }

        public override bool HandleException(object sender, Exception exception)
        {
            Logger.Error(exception, "程序发生未捕获异常");

            var loader = Container.Resolve<ILoader>() as Loader;
            if (loader != null)
			{
                loader.ShowMessageWithoutCountDownView(Visibility.Collapsed,
                    ImagePath.SwipeIDPageTitle,
                    ImagePath.Error,
                    "程序发生异常，请联系管理员。",
                    Visibility.Collapsed);
            }
            return true;
        }
    }
}
