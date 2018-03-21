namespace Yahvol.Services.Tests.Mocks
{
    using System.Collections.Generic;

    public class MockLogger : ServiceCommandLogger
    {
        private readonly List<LogEntry> logEntries = new List<LogEntry>();

        public List<LogEntry> LogEntries
        {
            get
            {
                return this.logEntries;
            }
        }

        public override void LogCustom(LogEntry entry)
        {
            this.LogEntries.Add(entry);
        }
    }
}
