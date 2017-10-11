using BrookfieldGrs.Web.Http.Tests.Core;

namespace BrookfieldGrs.Web.Http.Tests.Mocks
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;

    using BrookfieldGrs.Services;

    public class MockServiceCommandContext : IServiceCommandContext, IDisposable
    {
        private readonly IDbSet<ServiceCommand> inMemoryDatabase = new InMemoryDbSet<ServiceCommand>();
        
        private readonly IDbSet<Subscriber> inMemoryDatabase2 = new InMemoryDbSetSubscribers<Subscriber>();

        public IDbSet<LogEntry> LogEntries
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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
                var dbSet = value;
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
                var dbSet = value;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int SaveChanges()
        {
            return this.inMemoryDatabase.Count();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            throw new System.NotImplementedException();
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