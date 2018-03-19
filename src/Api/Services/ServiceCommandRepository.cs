// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCommandRepository.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Yahvol.Data;

    public class ServiceCommandRepository : IServiceCommandRepository, IDisposable
    {
        private readonly IInstanceFactory<IServiceCommandContext> contextFactory;

        public ServiceCommandRepository(IInstanceFactory<IServiceCommandContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<ServiceCommand> Get()
        {
            using (var context = this.contextFactory.Create())
            {
                return context.ServiceCommands.ToList();
            }
        }

        public IEnumerable<TSource> Get<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            using (var context = this.contextFactory.Create())
            {
                var dbset = context.Set<TSource>();
                return dbset.Where(predicate).ToList();
            }
        }

        public IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate)
        {
            using (var context = this.contextFactory.Create())
            {
                var dbset = context.Set<ServiceCommand>();
                return dbset.Where(predicate).ToList();
            }
        }

        public ServiceCommand Get(int id)
        {
            using (var context = this.contextFactory.Create())
            {
                return context.ServiceCommands.Include(s => s.Subscribers).SingleOrDefault(sc => sc.Id == id);
            }
        }

        public async Task Log(LogEntry entry)
        {
            using (var context = this.contextFactory.Create())
            {
                context.LogEntries.Add(entry);
                context.SaveChanges();
            }
        }

        public int Save(ref ServiceCommand serviceCommand)
        {
            using (var context = this.contextFactory.Create())
            {
                foreach (var subscriber in serviceCommand.Subscribers)
                {
                    context.Subscribers.Add(subscriber);
                }

                context.ServiceCommands.Add(serviceCommand);
                context.SaveChanges();

                return serviceCommand.Id;
            }
        }

        public async Task Update(int subscriberId, int serviceCommandId, int subscriberRetryCount, bool subscriberCompleted = false)
        {
            var now = DateTime.Now;

            using (var context = this.contextFactory.Create())
            {
                var subscriber = context.Subscribers.First(s => s.Id == subscriberId);

                subscriber.LastUpdatedDate = now;
                subscriber.RetryCount = subscriberRetryCount;
                subscriber.Completed = subscriberCompleted;
                context.SaveChanges();

                if (!subscriber.Completed)
                {
                    return;
                }
            }

            using (var context = this.contextFactory.Create())
            {
                var command = context.ServiceCommands.Include(s => s.Subscribers).First(sc => sc.Id == serviceCommandId);
                var incompleteSubscribers = command.Subscribers.Where(s => s.Completed == false);
                if (!incompleteSubscribers.Any())
                {
                    command.Completed = true;
                    command.CompletedDate = now;
                }

                context.SaveChanges();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
        }
    }
}