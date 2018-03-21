namespace Yahvol.Services
{
    using System;

    public static class ServiceCommandLoggerExtensions
    {
        public static void Log(this ServiceCommandLogger logger, string message, string source = null)
        {
            logger.Log(new LogEntry(logger.ServiceCommandId, logger.SubscriberId, logger.RetryCount, LoggingEventType.Information, message, source, null));
        }

        public static void Log(this ServiceCommandLogger logger, Exception exception, string source = null)
        {
            logger.Log(new LogEntry(logger.ServiceCommandId, logger.SubscriberId, logger.RetryCount, LoggingEventType.Error, exception.Message, source, exception));
        }
    }
}