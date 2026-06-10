using System;
using System.Globalization;
using log4net.Core;
using log4net.Util;

namespace Genesis.Logging
{
    public class GenesisLogger : ILogger
    {
        private readonly log4net.Core.ILogger _log4netLogger;
        private static readonly Type _declaringType = typeof(GenesisLogger);

        public GenesisLogger()
        {
            _log4netLogger = LoggerFactory.InitLogger();
        }
        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return _log4netLogger.IsEnabledFor(level: log4net.Core.Level.Debug);
                case LogLevel.Information:
                    return _log4netLogger.IsEnabledFor(level: log4net.Core.Level.Info);
                case LogLevel.Warning:
                    return _log4netLogger.IsEnabledFor(level: log4net.Core.Level.Warn);
                case LogLevel.Error:
                    return _log4netLogger.IsEnabledFor(level: log4net.Core.Level.Error);
                case LogLevel.Fatal:
                    return _log4netLogger.IsEnabledFor(level: log4net.Core.Level.Fatal);
            }

            return false;
        }

        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            if (args == null)
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        _log4netLogger.Log(_declaringType, Level.Debug, format, exception);
                        break;
                    case LogLevel.Information:
                        _log4netLogger.Log(_declaringType, Level.Info, format, exception);
                        break;
                    case LogLevel.Warning:
                        _log4netLogger.Log(_declaringType, Level.Warn, format, exception);
                        break;
                    case LogLevel.Error:
                        _log4netLogger.Log(_declaringType, Level.Error, format, exception);
                        break;
                    case LogLevel.Fatal:
                        _log4netLogger.Log(_declaringType, Level.Fatal, format, exception);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        _log4netLogger.Log(_declaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                        break;
                    case LogLevel.Information:
                        _log4netLogger.Log(_declaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                        break;
                    case LogLevel.Warning:
                        _log4netLogger.Log(_declaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                        break;
                    case LogLevel.Error:
                        _log4netLogger.Log(_declaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                        break;
                    case LogLevel.Fatal:
                        _log4netLogger.Log(_declaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
