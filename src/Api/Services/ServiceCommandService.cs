// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCommandService.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using Akka.Actor;

    using Yahvol.Data;
    using Yahvol.Linq.Expressions;
    using Yahvol.Services.Commands;

    public class ServiceCommandService : IServiceCommandService
    {
        private readonly IInstanceFactory<IServiceCommandContext> contextFactory;

        private readonly IInstanceFactory<ServiceCommandLogger> loggerFactory;

		private readonly ActorSelection throttle;

		private ServiceCommand serviceCommand;

		public ServiceCommandService()
			: this(new ServiceCommandContextInstanceFactory(), new ServiceCommandLoggerInstanceFactory())
		{
		}

		public ServiceCommandService(ActorSelection throttle)
            : this(new ServiceCommandContextInstanceFactory(), new ServiceCommandLoggerInstanceFactory(), throttle)
        {
        }

        public ServiceCommandService(IInstanceFactory<IServiceCommandContext> contextFactory, ActorSelection throttle = null)
            : this(contextFactory, new ServiceCommandLoggerInstanceFactory(), throttle)
        {
        }

        public ServiceCommandService(IInstanceFactory<ServiceCommandLogger> loggerFactory, ActorSelection throttle = null)
            : this(new ServiceCommandContextInstanceFactory(), loggerFactory, throttle)
        {
        }

        public ServiceCommandService(IInstanceFactory<IServiceCommandContext> contextFactory, IInstanceFactory<ServiceCommandLogger> loggerFactory, ActorSelection throttle = null)
        {
            this.ServiceCommandRepository = new ServiceCommandRepository(this.contextFactory = contextFactory);
            this.loggerFactory = loggerFactory;
			this.throttle = throttle;

            this.Subscribers = new List<Subscriber>();
            this.Workloads = new List<IWorkload>();
        }

        public string EndpointId { get; set; }

        public IServiceCommandRepository ServiceCommandRepository { get; set; }

        public List<Subscriber> Subscribers { get; set; }

        public IEnumerable<IWorkload> Workloads { get; set; }

        protected ICommand Command { get; set; }

        public ServiceCommand Add(ServiceCommand servicecommand)
        {
            servicecommand.Id = this.ServiceCommandRepository.Save(ref servicecommand);
            return servicecommand;
        }

        public IServiceCommandService AddCommand<T>(T command) where T : ICommand
        {
            if (this.serviceCommand != null)
            {
                throw new ApplicationException("Only one command can be added to the CommandService");
            }

            if (string.IsNullOrEmpty(command.UniqueKey))
            {
                command.UniqueKey = Guid.NewGuid().ToString();
            }

            this.Command = command;

            this.serviceCommand = new ServiceCommand(Serializer.Serialize(command), DateTime.Now, command.CreatedBy, command.GetType().FullName, Thread.CurrentPrincipal.Identity.Name, command.UniqueKey, this.EndpointId);
            return this;
        }

        public IServiceCommandService AddSubscribers(Subscriber subscriber)
        {
            this.Subscribers.Add(subscriber);
            return this;
        }

        public IEnumerable<ServiceCommand> Get()
        {
            return this.ServiceCommandRepository.Get();
        }

        public IEnumerable<TSource> Get<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            return this.ServiceCommandRepository.Get(predicate);
        }

        public IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate, Expression<Func<ServiceCommand, bool>> userPredicate)
        {
            return this.ServiceCommandRepository.Get(predicate.And(userPredicate));
        }

        public IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate)
        {
            return this.ServiceCommandRepository.Get(predicate);
        }

        public ServiceCommand Get(int id)
        {
            return this.ServiceCommandRepository.Get(id);
        }

        public virtual void RaiseServiceCommandsCompletedEvent()
        {
        }

        public virtual void RaiseWorkloadCompletedEvent(string name)
        {
        }

        public T Run<T>() where T : ICommand
        {
            if (this.serviceCommand == null)
            {
                throw new NullReferenceException("The command is Null. A command must be added before Run() can be called");
            }

            if (this.Subscribers.Count == 0)
            {
                throw new NullReferenceException("At least one workload must be added prior to calling the Run() method");
            }

            foreach (var subscriber in this.Subscribers)
            {
                this.serviceCommand.AddSubscriber(subscriber);
                subscriber.ServiceCommand = this.serviceCommand;
                subscriber.ServiceCommandRepository = new ServiceCommandRepository(this.contextFactory);
                subscriber.Logger = this.loggerFactory.Create();
                subscriber.Workload.Logger += () => subscriber.Logger;
                subscriber.Workload.OnCompletedAction = () => this.RaiseWorkloadCompletedEvent(subscriber.Workload.GetType().Name);
            }

            var returnedServiceCommand = this.Add(this.serviceCommand);
            var returnCommand = (T)this.Command;
            returnCommand.Id = returnedServiceCommand.Id;
            var asyncTaskRunner = new AsyncTaskRunner(this.Subscribers, this.throttle);
            asyncTaskRunner.StartTasks();
            return returnCommand;
        }

        public IServiceCommandService WithWorkload(IWorkload workload, TimeSpan timeToExpire)
        {
            if (this.serviceCommand == null)
            {
                throw new NullReferenceException("The command is Null. A command must be added before WithWorkload() can be called");
            }

            workload.SetCommand(this.Command);

            return this.AddSubscribers(new Subscriber(timeToExpire, workload, workload.GetType().ToString()));
        }
    }
}