namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IServiceCommandRepository
    {
        IEnumerable<ServiceCommand> Get();

        ServiceCommand Get(int id);

        IEnumerable<TSource> Get<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class;

        IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate);

        int Save(ref ServiceCommand serviceCommand);

        Task Update(int subscriberId, int serviceCommandId, int subscriberRetryCount, bool subscriberCompleted = false);

        Task Log(LogEntry entry);
    }
}