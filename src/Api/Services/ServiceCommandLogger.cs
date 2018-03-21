namespace Yahvol.Services
{
	public class ServiceCommandLogger
    {
        public IServiceCommandRepository ServiceCommandRepository { get; set; }

        public int ServiceCommandId { get; set; }

        public int SubscriberId { get; set; }

        public int RetryCount { get; set; }

        public void Log(LogEntry entry)
        {
            this.LogDefault(entry);
            this.LogCustom(entry);
        }

        public void LogDefault(LogEntry entry)
        {
            this.ServiceCommandRepository.Log(entry);
        }

        public virtual void LogCustom(LogEntry entry)
        {
		}
    }
}