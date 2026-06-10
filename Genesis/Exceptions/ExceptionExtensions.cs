using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security;
using System.Runtime.InteropServices;

namespace Genesis.Exceptions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 判断异常是否为致命异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsFatal(this System.Exception ex)
        {
            return ex is StackOverflowException ||
                   ex is OutOfMemoryException ||
                   ex is AccessViolationException ||
                   ex is AppDomainUnloadedException ||
                   ex is ThreadAbortException ||
                   ex is SecurityException ||
                   ex is SEHException;
        }
    }
}
