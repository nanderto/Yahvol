using Yahvol.Web.HttpTests;

namespace Yahvol.Web.HttpTests
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.Services;

    public class MockServiceCommandContext : IServiceCommandContext, IDisposable
    {
        private readonly IDbSet<ServiceCommand> inMemoryServiceCommandDatabase = new InMemoryDbSet<ServiceCommand>();
        
        private readonly IDbSet<Subscriber> inMemoryDatabase2 = new InMemoryDbSetSubscribers<Subscriber>();

        private readonly IDbSet<LogEntry> inMemoryLogEntries = new InMemoryDbSetLogEntries<LogEntry>();

        public MockServiceCommandContext()
        {

        }

        public IDbSet<LogEntry> LogEntries
        {
            get
            {
                return this.inMemoryLogEntries;
            }

            set
            {
                throw new NotImplementedException("this is property is not currently needed");
            }
        }

        public ManualResetEvent ManualResetEvent { get; set; }

        public object ServiceCommand { get; set; }

        public IDbSet<ServiceCommand> ServiceCommands
        {
            get
            {
                return this.inMemoryServiceCommandDatabase;
            }

            set
            {
                throw new NotImplementedException("this is property is not currently needed");
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
                throw new NotImplementedException("this is property is not currently needed");
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int SaveChanges()
        {
            return this.inMemoryServiceCommandDatabase.Count();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return (DbSet<TEntity>) this.inMemoryServiceCommandDatabase;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (ManualResetEvent != null)
                {
                    ManualResetEvent.Dispose();
                }
            }
        }
    }
}