namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class CommandRepository : IServiceCommandRepository, IDisposable
    {
        private readonly AdoContext context;

        public CommandRepository(AdoContext context)
        {
            this.context = context;
            this.context.Connection.Open();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public System.Collections.Generic.IEnumerable<ServiceCommand> Get()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TSource> Get<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ServiceCommand Get(int id)
        {
            throw new NotImplementedException();
        }

        public int Save(ref ServiceCommand serviceCommand)
        {
            var newId = this.context.Add(serviceCommand);
            foreach (var subsciber in serviceCommand.Subscribers)
            {
                subsciber.Id = this.AddSubscriber(newId, subsciber).Id;
            }

            return newId;
        }

        public Task Update(int subscriberId, int serviceCommandId, int subscriberRetryCount, bool subscriberCompleted = false)
        {
            throw new NotImplementedException();
        }

        public Task Log(LogEntry entry)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (this.context.Connection != null)
                {
                    this.context.Connection.Dispose();
                    this.context.Connection = null;
                }
            }
        }

        private Subscriber AddSubscriber(int newId, Subscriber subscriber)
        {
            return this.context.AddSubscriber(newId, subscriber);
        }
    }
}
