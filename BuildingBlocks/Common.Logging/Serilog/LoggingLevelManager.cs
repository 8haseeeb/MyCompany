using Serilog.Core;
using Serilog.Events;

namespace MyCompany.Common.Logging.Serilog
{
    public class LoggingLevelManager
    {
        private readonly LoggingLevelSwitch _levelSwitch;

        public LoggingLevelManager(LoggingLevelSwitch levelSwitch)
        {
            _levelSwitch = levelSwitch;
        }

        public LogEventLevel GetCurrentLevel()
        {
            return _levelSwitch.MinimumLevel;
        }

        public void SetLevel(LogEventLevel level)
        {
            _levelSwitch.MinimumLevel = level;
        }

        public void SetLevel(string level)
        {
            if (Enum.TryParse<LogEventLevel>(level, true, out var logLevel))
            {
                _levelSwitch.MinimumLevel = logLevel;
            }
            else
            {
                throw new ArgumentException($"Invalid log level: {level}. Valid values: Verbose, Debug, Information, Warning, Error, Fatal");
            }
        }
    }
}
