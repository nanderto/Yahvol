namespace Yahvol.Services
{
    using System;

    public class LogEntry
    {
        public readonly Exception Exception;

        public LogEntry()
        {
        }

        public LogEntry(int serviceCommandId, int subscriberId, int retryCount, LoggingEventType severity, string message, string source, Exception exception)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("message");
            }

            this.LoggedDate = DateTime.Now;
            this.ServiceCommandId = serviceCommandId;
            this.SubscriberId = subscriberId;
            this.RetryCount = retryCount;
            this.Severity = severity;
            this.Source = source;
            this.Message = message;
            this.Exception = exception;
            if (exception != null)
            {
                this.Details = exception.ToString();
            }
        }

        public int Id { get; set; }

        public int ServiceCommandId { get; set; }

        public int SubscriberId { get; set; }

        public int RetryCount { get; set; }

        public DateTime? LoggedDate { get; set; }

        public LoggingEventType Severity { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }
    }
}