namespace Yahvol.Services.Tests.Mocks
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;

    using Yahvol.Services;

    public class MockServiceCommandContext : IServiceCommandContext
    {
        private readonly IDbSet<ServiceCommand> inMemoryDatabase = new InMemoryDbSet<ServiceCommand>();
        
        private readonly IDbSet<Subscriber> inMemoryDatabase2 = new InMemoryDbSetSubscribers<Subscriber>();

        private readonly IDbSet<LogEntry> inMemoryDatabase3 = new InMemoryDbSetLogEntries<LogEntry>();

        public ManualResetEvent ManualResetEvent { get; set; }

        public object ServiceCommand { get; set; }

        public IDbSet<ServiceCommand> ServiceCommands
        {
            get
            {
                return this.inMemoryDatabase;
            }

            set
            {
                var databaseSet = value;
            } 
        }

        public IDbSet<Subscriber> Subscribers
        {
            get
            {
                return this.inMemoryDatabase2;
            }

            set
            {
                var databaseSet = value;
            }
        }

        public IDbSet<LogEntry> LogEntries
        {
            get
            {
                return this.inMemoryDatabase3;
            }

            set
            {
                var databaseSet = value;
            }
        }

        public void Dispose()
        {
        }

        public int SaveChanges()
        {
            foreach (var subscriber in this.Subscribers.Where(subscriber => subscriber.ServiceCommandId == 0))
            {
                subscriber.ServiceCommandId = this.ServiceCommands.OrderByDescending(u => u.Id).First().Id;
            }

            return this.inMemoryDatabase.Count();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {           
            throw new System.NotImplementedException();
        }
    }
}