namespace BrookfieldGrs.Web.Http.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;

    public class MockRepository : IServiceCommandRepository
    {
        public IEnumerable<TSource> Get<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            var commands = new List<TSource>();
            commands.Add(Activator.CreateInstance<TSource>());
            commands.Add(Activator.CreateInstance<TSource>());
            return commands;
        }

        public ServiceCommand Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceCommand> Get()
        {
            var commands = new List<ServiceCommand>();
            commands.Add(new ServiceCommand());
            commands.Add(new ServiceCommand());
            return commands;
        }

        public IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate)
        {
            var commands = new List<ServiceCommand>();
            commands.Add(new ServiceCommand());
            commands.Add(new ServiceCommand());
            return commands;
        }

        public int Save(ref ServiceCommand serviceCommand)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(int id, int serviceCommandId)
        {
            throw new NotImplementedException();
        }

        public Task Update(int subscriberId, int serviceCommandId, int subscriberRetryCount, bool subscriberCompleted = false)
        {
            throw new NotImplementedException();
        }

        public Task Log(LogEntry entry)
        {
            throw new NotImplementedException();
        }

        Task<bool> IServiceCommandRepository.Update(int subscriberId, int serviceCommandId, int subscriberRetryCount, bool subscriberCompleted)
        {
            throw new NotImplementedException();
        }
    }
}
