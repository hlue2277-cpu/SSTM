using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InterceptSpareAttribute : Attribute
    {
    }
}
