using System.Reflection;

namespace Genesis.Logging
{
    public class LoggerFactory
    {
        public static log4net.Core.ILogger InitLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
            return log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType).Logger;
        }
    }
}
